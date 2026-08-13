#if UNITY_EDITOR

using System.IO;
using Lucy.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lucy.Editor.UI
{
    public static class SpaceMenuPrefabBuilder
    {
        #region Constants

        const string PrefabPath = "Assets/SpaceMenu/Prefabs/SpaceMenu.prefab";
        const string SpriteRootPath = "Assets/SpaceMenu/Sprites";

        #endregion

        #region Menu items

        [MenuItem("Tools/Lucy/Space Menu/Create Prefab")]
        public static void CreatePrefab()
        {
            EnsureFolder("Assets/SpaceMenu");
            EnsureFolder("Assets/SpaceMenu/Prefabs");

            GameObject root = BuildMenuRoot();
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);

            Debug.Log($"Space Menu prefab created: {PrefabPath}");
        }

        [MenuItem("Tools/Lucy/Space Menu/Create In Current Scene")]
        public static void CreateInCurrentScene()
        {
            GameObject root = BuildMenuRoot();
            root.name = "SpaceMenu";
            Undo.RegisterCreatedObjectUndo(root, "Create Space Menu");
            EnsureEventSystem();

            Selection.activeGameObject = root;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("Space Menu created in current scene.");
        }

        #endregion

        #region Build

        static GameObject BuildMenuRoot()
        {
            GameObject root = new GameObject(
                "SpaceMenu",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster),
                typeof(SpaceMenuView)
            );

            RectTransform rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            SpaceMenuView view = root.GetComponent<SpaceMenuView>();
            view.canvas = canvas;
            view.sunScale = 1f;

            Image backdrop = CreateSolidImage(
                root.transform,
                "Backdrop",
                new Color(0.02f, 0.02f, 0.04f, 1f),
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Vector2(0.5f, 0.5f)
            );
            view.backgroundImage = backdrop;

            Image starfieldImage = CreateImage(
                root.transform,
                "Starfield",
                "Backgrounds/starfield_clean_top_right.png",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Vector2(0.5f, 0.5f),
                preserveAspect: false
            );
            Color starfieldColor = starfieldImage.color;
            starfieldColor.a = 0.45f;
            starfieldImage.color = starfieldColor;
            view.starfieldImage = starfieldImage;

            Image sunImage = CreateImage(
                root.transform,
                "Sun",
                "Backgrounds/sun_large.png",
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-280f, 20f),
                new Vector2(840f, 590f),
                new Vector2(0.5f, 0.5f)
            );
            view.sunImage = sunImage;

            Image rightGlow = CreateImage(
                root.transform,
                "RightGlow",
                "Backgrounds/space_right_side.png",
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-120f, 0f),
                new Vector2(980f, 1010f),
                new Vector2(0.5f, 0.5f),
                preserveAspect: true
            );
            Color glowColor = rightGlow.color;
            glowColor.a = 0.7f;
            rightGlow.color = glowColor;

            Image panelImage = CreateImage(
                root.transform,
                "MenuPanel",
                "Panels/menu_panel_full.png",
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(420f, 0f),
                new Vector2(750f, 855f),
                new Vector2(0.5f, 0.5f)
            );
            view.panelImage = panelImage;

            CreateButtonHotspot(root.transform, view, "PlayButton", new Vector2(420f, 160f), new Vector2(520f, 125f), ButtonType.Play);
            CreateButtonHotspot(root.transform, view, "SettingsButton", new Vector2(420f, 15f), new Vector2(520f, 125f), ButtonType.Settings);
            CreateButtonHotspot(root.transform, view, "ExitButton", new Vector2(420f, -135f), new Vector2(520f, 125f), ButtonType.Exit);

            return root;
        }

        static void CreateButtonHotspot(Transform parent, SpaceMenuView view, string objectName, Vector2 anchoredPosition, Vector2 size, ButtonType buttonType)
        {
            GameObject gameObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image),
                typeof(Button)
            );

            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = gameObject.GetComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.001f);
            image.raycastTarget = true;

            Button button = gameObject.GetComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(1f, 1f, 1f, 0.001f);
            colors.highlightedColor = new Color(1f, 0.8f, 0.2f, 0.12f);
            colors.pressedColor = new Color(1f, 0.6f, 0.1f, 0.20f);
            colors.selectedColor = new Color(1f, 0.8f, 0.2f, 0.12f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.05f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            button.transition = Selectable.Transition.ColorTint;

            switch (buttonType)
            {
                case ButtonType.Play:
                    view.playButton = button;
                    break;
                case ButtonType.Settings:
                    view.settingsButton = button;
                    break;
                case ButtonType.Exit:
                    view.exitButton = button;
                    break;
            }
        }

        #endregion

        #region Factory methods

        static Image CreateSolidImage(
            Transform parent,
            string objectName,
            Color color,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 sizeDelta,
            Vector2 pivot)
        {
            GameObject gameObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;

            Image image = gameObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        static Image CreateImage(
            Transform parent,
            string objectName,
            string spriteRelativePath,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 size,
            Vector2 pivot,
            bool preserveAspect = true)
        {
            GameObject gameObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = gameObject.GetComponent<Image>();
            image.sprite = LoadSprite(spriteRelativePath);
            image.preserveAspect = preserveAspect;
            image.raycastTarget = false;
            return image;
        }

        static Sprite LoadSprite(string relativePath)
        {
            string path = $"{SpriteRootPath}/{relativePath}";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite != null)
                return sprite;

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            if (sprite == null)
                Debug.LogError($"Space Menu sprite not found: {path}");

            return sprite;
        }

        static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
                return;

            GameObject eventSystem = new GameObject(
                "EventSystem",
                typeof(EventSystem),
                typeof(StandaloneInputModule)
            );
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
            string folder = Path.GetFileName(path);

            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
                EnsureFolder(parent);

            AssetDatabase.CreateFolder(parent, folder);
        }

        enum ButtonType
        {
            Play,
            Settings,
            Exit
        }

        #endregion
    }
}

#endif
