#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Lucy.Features.Audio.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace Lucy.Features.Audio.Editor
{
    public static class AudioSystemInstaller
    {
        const string RootFolder = "Assets/Lucy/Features/Audio";
        const string MixerFolder = RootFolder + "/Mixer";
        const string PrefabFolder = RootFolder + "/Prefabs";
        const string MixerPath = MixerFolder + "/MainAudioMixer.mixer";
        const string PrefabPath = PrefabFolder + "/AudioSystem.prefab";

        static readonly BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        static readonly BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        [MenuItem("Tools/Lucy/Audio/Create Audio System")]
        public static void CreateAudioSystem()
        {
            EnsureFolders();

            AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
            object controller;

            if (mixer == null)
            {
                controller = CreateMixerController(MixerPath);
                mixer = controller as AudioMixer;
            }
            else
            {
                controller = mixer;
            }

            if (mixer == null)
                throw new InvalidOperationException("Failed to create or load AudioMixer.");

            object masterGroupController = GetMemberValue(controller, "masterGroup");
            if (masterGroupController == null)
                throw new InvalidOperationException("AudioMixer master group was not found.");

            object musicController = EnsureGroup(controller, masterGroupController, mixer, "Music");
            object sfxController = EnsureGroup(controller, masterGroupController, mixer, "SFX");
            object uiController = EnsureGroup(controller, masterGroupController, mixer, "UI");
            object ambienceController = EnsureGroup(controller, masterGroupController, mixer, "Ambience");

            EnsureExposedVolume(controller, masterGroupController, "MasterVolume");
            EnsureExposedVolume(controller, musicController, "MusicVolume");
            EnsureExposedVolume(controller, sfxController, "SFXVolume");
            EnsureExposedVolume(controller, uiController, "UIVolume");
            EnsureExposedVolume(controller, ambienceController, "AmbienceVolume");

            EditorUtility.SetDirty((Object)controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AudioMixerGroup music = FindExactGroup(mixer, "Music");
            AudioMixerGroup sfx = FindExactGroup(mixer, "SFX");
            AudioMixerGroup ui = FindExactGroup(mixer, "UI");
            AudioMixerGroup ambience = FindExactGroup(mixer, "Ambience");

            Runtime.AudioSystem system = Object.FindAnyObjectByType<Runtime.AudioSystem>();
            if (system == null)
            {
                GameObject root = new GameObject("[AudioSystem]");
                Undo.RegisterCreatedObjectUndo(root, "Create Audio System");
                system = root.AddComponent<Runtime.AudioSystem>();
            }

            system.mixer = mixer;
            system.musicGroup = music;
            system.sfxGroup = sfx;
            system.uiGroup = ui;
            system.ambienceGroup = ambience;

            EditorUtility.SetDirty(system);

            GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(
                system.gameObject,
                PrefabPath,
                InteractionMode.UserAction);

            Selection.activeObject = prefab != null ? prefab : system.gameObject;

            Debug.Log(
                "Audio system created. Mixer groups: Master, Music, SFX, UI, Ambience. " +
                "Exposed parameters: MasterVolume, MusicVolume, SFXVolume, UIVolume, AmbienceVolume.",
                system);
        }

        static object CreateMixerController(string path)
        {
            Type controllerType = FindEditorType("UnityEditor.Audio.AudioMixerController");
            MethodInfo createMethod = controllerType.GetMethod("CreateMixerControllerAtPath", StaticFlags);

            if (createMethod == null)
                throw new MissingMethodException(controllerType.FullName, "CreateMixerControllerAtPath");

            return createMethod.Invoke(null, new object[] { path });
        }

        static object EnsureGroup(object controller, object masterGroup, AudioMixer mixer, string groupName)
        {
            AudioMixerGroup existing = FindExactGroup(mixer, groupName);
            if (existing != null)
                return existing;

            MethodInfo createGroup = controller.GetType().GetMethod("CreateNewGroup", InstanceFlags);
            MethodInfo addChild = controller.GetType().GetMethod("AddChildToParent", InstanceFlags);

            if (createGroup == null || addChild == null)
                throw new MissingMethodException("Unity internal AudioMixer group API was not found.");

            object group = createGroup.Invoke(controller, new object[] { groupName, false });
            addChild.Invoke(controller, new[] { group, masterGroup });

            MethodInfo addToView = controller.GetType().GetMethod("AddGroupToCurrentView", InstanceFlags);
            addToView?.Invoke(controller, new[] { group });

            return group;
        }

        static AudioMixerGroup FindExactGroup(AudioMixer mixer, string groupName)
        {
            return mixer.FindMatchingGroups(groupName).FirstOrDefault(x => x != null && x.name == groupName);
        }

        static void EnsureExposedVolume(object controller, object group, string exposedName)
        {
            if (HasExposedParameter(controller, exposedName))
                return;

            MethodInfo getVolumeGuid = group.GetType().GetMethod("GetGUIDForVolume", InstanceFlags);
            if (getVolumeGuid == null)
                throw new MissingMethodException(group.GetType().FullName, "GetGUIDForVolume");

            object guid = getVolumeGuid.Invoke(group, null);
            Type pathType = FindEditorType("UnityEditor.Audio.AudioGroupParameterPath");

            ConstructorInfo pathConstructor = pathType
                .GetConstructors(InstanceFlags)
                .FirstOrDefault(x => x.GetParameters().Length == 2);

            if (pathConstructor == null)
                throw new MissingMethodException(pathType.FullName, ".ctor");

            object path = pathConstructor.Invoke(new[] { group, guid });

            MethodInfo addExposed = controller.GetType().GetMethod("AddExposedParameter", InstanceFlags);
            if (addExposed == null)
                throw new MissingMethodException(controller.GetType().FullName, "AddExposedParameter");

            addExposed.Invoke(controller, new[] { path });
            RenameExposedParameter(controller, guid, exposedName);
        }

        static bool HasExposedParameter(object controller, string parameterName)
        {
            Array parameters = GetExposedParameters(controller);
            if (parameters == null)
                return false;

            foreach (object parameter in parameters)
            {
                string name = GetMemberValue(parameter, "name") as string;
                if (name == parameterName)
                    return true;
            }

            return false;
        }

        static void RenameExposedParameter(object controller, object guid, string exposedName)
        {
            PropertyInfo exposedProperty = controller.GetType().GetProperty("exposedParameters", InstanceFlags);
            if (exposedProperty == null)
                throw new MissingMemberException(controller.GetType().FullName, "exposedParameters");

            Array parameters = exposedProperty.GetValue(controller) as Array;
            if (parameters == null)
                return;

            for (int i = 0; i < parameters.Length; i++)
            {
                object parameter = parameters.GetValue(i);
                object parameterGuid = GetMemberValue(parameter, "guid");

                if (parameterGuid == null || !parameterGuid.Equals(guid))
                    continue;

                SetMemberValue(parameter, "name", exposedName);
                parameters.SetValue(parameter, i);
                exposedProperty.SetValue(controller, parameters);
                return;
            }
        }

        static Array GetExposedParameters(object controller)
        {
            PropertyInfo exposedProperty = controller.GetType().GetProperty("exposedParameters", InstanceFlags);
            return exposedProperty?.GetValue(controller) as Array;
        }

        static object GetMemberValue(object target, string memberName)
        {
            if (target == null)
                return null;

            Type type = target.GetType();

            PropertyInfo property = type.GetProperty(memberName, InstanceFlags);
            if (property != null)
                return property.GetValue(target);

            FieldInfo field = type.GetField(memberName, InstanceFlags);
            if (field != null)
                return field.GetValue(target);

            return null;
        }

        static void SetMemberValue(object target, string memberName, object value)
        {
            if (target == null)
                return;

            Type type = target.GetType();

            PropertyInfo property = type.GetProperty(memberName, InstanceFlags);
            if (property != null)
            {
                property.SetValue(target, value);
                return;
            }

            FieldInfo field = type.GetField(memberName, InstanceFlags);
            if (field != null)
            {
                field.SetValue(target, value);
                return;
            }

            throw new MissingMemberException(type.FullName, memberName);
        }

        static Type FindEditorType(string fullName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName);
                if (type != null)
                    return type;
            }

            throw new TypeLoadException($"Unity editor type '{fullName}' was not found.");
        }

        static void EnsureFolders()
        {
            EnsureFolder("Assets", "Lucy");
            EnsureFolder("Assets/Lucy", "Features");
            EnsureFolder("Assets/Lucy/Features", "Audio");
            EnsureFolder(RootFolder, "Mixer");
            EnsureFolder(RootFolder, "Prefabs");
        }

        static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
#endif
