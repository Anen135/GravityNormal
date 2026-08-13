using System;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lucy.Features.UI.Editor
{
    public static class AudioSettingsPrefabCreator
    {
        const string RootFolder = "Assets/Lucy";
        const string FeaturesFolder = "Assets/Lucy/Features";
        const string UiFolder = "Assets/Lucy/Features/UI";
        const string PrefabsFolder = "Assets/Lucy/Features/UI/Prefabs";
        const string PrefabPath = "Assets/Lucy/Features/UI/Prefabs/AudioSettingsWindow.prefab";

        [MenuItem("Tools/Lucy/UI/Create Audio Settings Prefab")]
        public static void CreateAudioSettingsPrefab()
        {
            EnsureFolders();

            GameObject root = CreateRoot();
            try
            {
                RectTransform rootRect = root.GetComponent<RectTransform>();

                GameObject dimBackground = CreateDimBackground(rootRect);
                GameObject revealArea = CreateRevealArea(rootRect);
                GameObject panel = CreatePanel(revealArea.GetComponent<RectTransform>());

                GameObject title = CreateTitle(panel.GetComponent<RectTransform>());
                GameObject rowsContainer = CreateRowsContainer(panel.GetComponent<RectTransform>());

                Slider globalSlider = CreateSliderRow(rowsContainer.transform, "Global", "GlobalSlider");
                Slider musicSlider = CreateSliderRow(rowsContainer.transform, "Music", "MusicSlider");
                Slider soundsSlider = CreateSliderRow(rowsContainer.transform, "Sounds", "SoundsSlider");

                Button closeButton = CreateCloseButton(panel.GetComponent<RectTransform>());

                CanvasGroup canvasGroup = root.GetComponent<CanvasGroup>();
                RectTransform revealRect = revealArea.GetComponent<RectTransform>();
                Button dimButton = dimBackground.GetComponent<Button>();

                TryAttachRuntimeComponents(
                    root,
                    canvasGroup,
                    revealRect,
                    dimButton,
                    closeButton,
                    globalSlider,
                    musicSlider,
                    soundsSlider);

                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
                EditorGUIUtility.PingObject(Selection.activeObject);

                Debug.Log($"Audio settings prefab created: {PrefabPath}");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(root);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static void EnsureFolders()
        {
            EnsureFolder("Assets", "Lucy");
            EnsureFolder(RootFolder, "Features");
            EnsureFolder(FeaturesFolder, "UI");
            EnsureFolder(UiFolder, "Prefabs");
        }

        static void EnsureFolder(string parent, string child)
        {
            string path = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(parent, child);
        }

        static GameObject CreateRoot()
        {
            GameObject root = new GameObject("AudioSettingsWindow", typeof(RectTransform), typeof(CanvasGroup));
            RectTransform rect = root.GetComponent<RectTransform>();

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            CanvasGroup canvasGroup = root.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            return root;
        }

        static GameObject CreateDimBackground(Transform parent)
        {
            GameObject go = new GameObject("DimBackground", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);

            Image image = go.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.55f);

            Button button = go.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.disabledColor = Color.white;
            button.colors = colors;

            return go;
        }

        static GameObject CreateRevealArea(Transform parent)
        {
            GameObject go = new GameObject("RevealArea", typeof(RectTransform), typeof(RectMask2D));
            go.transform.SetParent(parent, false);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(980f, 0f);
            rect.anchoredPosition = new Vector2(0f, -40f);

            return go;
        }

        static GameObject CreatePanel(Transform parent)
        {
            GameObject go = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(980f, 340f);
            rect.anchoredPosition = Vector2.zero;

            Image image = go.GetComponent<Image>();
            image.color = new Color(0.16f, 0.17f, 0.20f, 0.96f);

            Outline outline = go.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.08f);
            outline.effectDistance = new Vector2(1f, -1f);

            return go;
        }

        static GameObject CreateTitle(Transform parent)
        {
            GameObject go = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(520f, 56f);
            rect.anchoredPosition = new Vector2(0f, -24f);

            TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
            text.text = "Audio Settings";
            text.fontSize = 34f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            return go;
        }

        static GameObject CreateRowsContainer(Transform parent)
        {
            GameObject go = new GameObject("Rows", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            go.transform.SetParent(parent, false);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = new Vector2(72f, 88f);
            rect.offsetMax = new Vector2(-72f, -90f);

            VerticalLayoutGroup layout = go.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 22f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = true;

            ContentSizeFitter fitter = go.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            return go;
        }

        static Slider CreateSliderRow(Transform parent, string labelText, string objectName)
        {
            GameObject row = new GameObject(objectName, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            row.transform.SetParent(parent, false);

            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(0f, 56f);

            HorizontalLayoutGroup rowLayout = row.GetComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 20f;
            rowLayout.childAlignment = TextAnchor.MiddleLeft;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = true;

            LayoutElement rowElement = row.GetComponent<LayoutElement>();
            rowElement.preferredHeight = 56f;

            GameObject label = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            label.transform.SetParent(row.transform, false);

            LayoutElement labelLayout = label.GetComponent<LayoutElement>();
            labelLayout.preferredWidth = 180f;
            labelLayout.minWidth = 180f;

            TextMeshProUGUI labelTextMesh = label.GetComponent<TextMeshProUGUI>();
            labelTextMesh.text = labelText;
            labelTextMesh.fontSize = 24f;
            labelTextMesh.alignment = TextAlignmentOptions.Left;
            labelTextMesh.color = Color.white;
            labelTextMesh.margin = new Vector4(6f, 0f, 0f, 0f);

            GameObject sliderRoot = new GameObject("Slider", typeof(RectTransform), typeof(Slider), typeof(LayoutElement));
            sliderRoot.transform.SetParent(row.transform, false);

            LayoutElement sliderLayout = sliderRoot.GetComponent<LayoutElement>();
            sliderLayout.flexibleWidth = 1f;
            sliderLayout.preferredHeight = 32f;

            RectTransform sliderRect = sliderRoot.GetComponent<RectTransform>();
            sliderRect.sizeDelta = new Vector2(640f, 32f);

            Slider slider = sliderRoot.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;
            slider.direction = Slider.Direction.LeftToRight;
            slider.transition = Selectable.Transition.ColorTint;

            GameObject background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(sliderRoot.transform, false);

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0f, 0.5f);
            backgroundRect.anchorMax = new Vector2(1f, 0.5f);
            backgroundRect.pivot = new Vector2(0.5f, 0.5f);
            backgroundRect.sizeDelta = new Vector2(0f, 20f);
            backgroundRect.anchoredPosition = Vector2.zero;

            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.color = new Color(0.82f, 0.82f, 0.82f, 1f);

            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(sliderRoot.transform, false);

            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0f, 0f);
            fillAreaRect.anchorMax = new Vector2(1f, 1f);
            fillAreaRect.offsetMin = new Vector2(10f, 6f);
            fillAreaRect.offsetMax = new Vector2(-16f, -6f);

            GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fill.transform.SetParent(fillArea.transform, false);

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            Image fillImage = fill.GetComponent<Image>();
            fillImage.color = new Color(0.98f, 0.71f, 0.18f, 1f);

            GameObject handleSlideArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleSlideArea.transform.SetParent(sliderRoot.transform, false);

            RectTransform handleAreaRect = handleSlideArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = new Vector2(0f, 0f);
            handleAreaRect.anchorMax = new Vector2(1f, 1f);
            handleAreaRect.offsetMin = new Vector2(10f, 0f);
            handleAreaRect.offsetMax = new Vector2(-10f, 0f);

            GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handle.transform.SetParent(handleSlideArea.transform, false);

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(24f, 42f);

            Image handleImage = handle.GetComponent<Image>();
            handleImage.color = Color.white;

            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.targetGraphic = handleImage;

            return slider;
        }

        static Button CreateCloseButton(Transform parent)
        {
            GameObject go = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(180f, 42f);
            rect.anchoredPosition = new Vector2(0f, 24f);

            Image image = go.GetComponent<Image>();
            image.color = new Color(0.25f, 0.27f, 0.32f, 1f);

            Button button = go.GetComponent<Button>();

            GameObject label = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            label.transform.SetParent(go.transform, false);

            RectTransform labelRect = label.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            TextMeshProUGUI text = label.GetComponent<TextMeshProUGUI>();
            text.text = "Close";
            text.fontSize = 22f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            return button;
        }

        static void TryAttachRuntimeComponents(
            GameObject root,
            CanvasGroup canvasGroup,
            RectTransform revealArea,
            Button dimButton,
            Button closeButton,
            Slider globalSlider,
            Slider musicSlider,
            Slider soundsSlider)
        {
            Type windowType = FindType("Lucy.Features.UI.Runtime.AudioSettingsWindow");
            if (windowType != null)
            {
                Component windowComponent = root.AddComponent(windowType);
                SerializedObject serializedWindow = new SerializedObject(windowComponent);

                SetObjectReference(serializedWindow, "windowCanvasGroup", canvasGroup);
                SetObjectReference(serializedWindow, "revealArea", revealArea);
                SetObjectReference(serializedWindow, "closeButton", closeButton);
                SetObjectReference(serializedWindow, "backgroundButton", dimButton);
                SetFloat(serializedWindow, "openedHeight", 340f);
                SetFloat(serializedWindow, "animationDuration", 0.25f);
                SetBool(serializedWindow, "startOpened", false);

                serializedWindow.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Debug.LogWarning("AudioSettingsWindow script not found. Prefab created without animation component.");
            }

            Type binderType = FindType("Lucy.Features.UI.Runtime.AudioSettingsBinder");
            if (binderType != null)
            {
                Component binderComponent = root.AddComponent(binderType);
                SerializedObject serializedBinder = new SerializedObject(binderComponent);

                SetObjectReference(serializedBinder, "globalSlider", globalSlider);
                SetObjectReference(serializedBinder, "musicSlider", musicSlider);
                SetObjectReference(serializedBinder, "soundsSlider", soundsSlider);
                SetBool(serializedBinder, "applySoundsToUI", true);
                SetBool(serializedBinder, "refreshValuesOnEnable", true);

                serializedBinder.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Debug.LogWarning("AudioSettingsBinder script not found. Prefab created without mixer binding component.");
            }
        }

        static void SetObjectReference(SerializedObject serializedObject, string propertyName, UnityEngine.Object value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
                property.objectReferenceValue = value;
        }

        static void SetFloat(SerializedObject serializedObject, string propertyName, float value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
                property.floatValue = value;
        }

        static void SetBool(SerializedObject serializedObject, string propertyName, bool value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
                property.boolValue = value;
        }

        static Type FindType(string fullName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(GetTypesSafely)
                .FirstOrDefault(x => x.FullName == fullName);
        }

        static Type[] GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }
    }
}