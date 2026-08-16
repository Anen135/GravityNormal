#if UNITY_EDITOR
using System.IO;
using Lucy.Features.UI.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lucy.Features.UI.Editor
{
    public static class PauseMenuPrefabBuilder
    {
        const string PrefabDirectory = "Assets/Lucy/Features/UI/Prefabs";
        const string PrefabPath = PrefabDirectory + "/PauseMenu.prefab";

        [MenuItem("Tools/Lucy/UI/Create Pause Menu Prefab")]
        public static void CreatePrefab()
        {
            EnsureDirectory(PrefabDirectory);

            GameObject root = new GameObject("PauseMenu");
            PauseMenu pauseMenu = root.AddComponent<PauseMenu>();

            GameObject canvasObject = new GameObject(
                "PauseCanvas",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );
            canvasObject.transform.SetParent(root.transform, false);

            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject overlay = CreateImage(
                "PauseRoot",
                canvasObject.transform,
                new Color(0f, 0f, 0f, 0.72f)
            );
            StretchFull(overlay.GetComponent<RectTransform>());

            GameObject mainPanel = CreatePanel("MainPanel", overlay.transform, new Vector2(560f, 440f));
            CreateText("Title", mainPanel.transform, "PAUSED", 44, new Vector2(0f, 145f), new Vector2(430f, 70f));

            Button resumeButton = CreateButton(
                "ResumeButton",
                mainPanel.transform,
                "RESUME",
                new Vector2(0f, 55f)
            );

            Button settingsButton = CreateButton(
                "SettingsButton",
                mainPanel.transform,
                "SETTINGS",
                new Vector2(0f, -35f)
            );

            Button mainMenuButton = CreateButton(
                "MainMenuButton",
                mainPanel.transform,
                "MAIN MENU",
                new Vector2(0f, -125f)
            );

            GameObject settingsPanel = CreatePanel("SettingsPanel", overlay.transform, new Vector2(560f, 440f));
            CreateText("Title", settingsPanel.transform, "SETTINGS", 42, new Vector2(0f, 140f), new Vector2(430f, 70f));
            CreateText(
                "Hint",
                settingsPanel.transform,
                "Replace this panel with your Settings UI\nor add controls here.",
                24,
                new Vector2(0f, 30f),
                new Vector2(430f, 100f)
            );

            Button closeSettingsButton = CreateButton(
                "BackButton",
                settingsPanel.transform,
                "BACK",
                new Vector2(0f, -120f)
            );

            settingsPanel.SetActive(false);

            pauseMenu.pauseRoot = overlay;
            pauseMenu.mainPanel = mainPanel;
            pauseMenu.settingsPanel = settingsPanel;
            pauseMenu.resumeButton = resumeButton;
            pauseMenu.settingsButton = settingsButton;
            pauseMenu.mainMenuButton = mainMenuButton;
            pauseMenu.closeSettingsButton = closeSettingsButton;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);

            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static GameObject CreatePanel(string name, Transform parent, Vector2 size)
        {
            GameObject panel = CreateImage(name, parent, new Color(0.055f, 0.075f, 0.11f, 0.98f));
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            return panel;
        }

        static Button CreateButton(string name, Transform parent, string label, Vector2 position)
        {
            GameObject buttonObject = CreateImage(name, parent, new Color(0.12f, 0.17f, 0.24f, 1f));
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(370f, 68f);
            rect.anchoredPosition = position;

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = buttonObject.GetComponent<Image>();

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.78f, 0.9f, 1f, 1f);
            colors.pressedColor = new Color(0.55f, 0.75f, 0.95f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;

            CreateText("Label", buttonObject.transform, label, 28, Vector2.zero, new Vector2(350f, 60f));
            return button;
        }

        static GameObject CreateImage(string name, Transform parent, Color color)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            gameObject.transform.SetParent(parent, false);

            Image image = gameObject.GetComponent<Image>();
            image.color = color;

            return gameObject;
        }

        static void CreateText(
            string name,
            Transform parent,
            string value,
            int fontSize,
            Vector2 position,
            Vector2 size
        )
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;

            Text text = textObject.GetComponent<Text>();
            text.text = value;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        static void StretchFull(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static void EnsureDirectory(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
                return;

            string[] parts = assetPath.Split('/');
            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];

                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);

                current = next;
            }
        }
    }
}
#endif
