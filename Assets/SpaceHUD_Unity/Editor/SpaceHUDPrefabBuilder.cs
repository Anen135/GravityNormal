#if UNITY_EDITOR

using System.IO;
using Lucy.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lucy.Editor.UI
{
    public static class SpaceHUDPrefabBuilder
    {
        #region Constants

        const string HudRootPath = "Assets/SpaceHUD";
        const string PrefabPath = "Assets/SpaceHUD/SpaceHUD.prefab";

        #endregion

        #region Menu

        [MenuItem("Tools/Lucy/Space HUD/Create Prefab")]
        public static void CreatePrefab()
        {
            EnsureFolder(HudRootPath);

            GameObject root = CreateCanvasRoot();
            SpaceHUDView view = root.AddComponent<SpaceHUDView>();

            CreateLeftPanel(root.transform, view);
            CreateRightPanel(root.transform, view);
            CreateCenterReticle(root.transform);

            view.Refresh();

            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);

            Debug.Log($"Space HUD prefab created: {PrefabPath}");
        }

        #endregion

        #region Root

        static GameObject CreateCanvasRoot()
        {
            GameObject root = new GameObject(
                "SpaceHUD",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return root;
        }

        #endregion

        #region Left panel

        static void CreateLeftPanel(Transform parent, SpaceHUDView view)
        {
            RectTransform panel = CreateImage(
                parent,
                "LeftPanel",
                "panel_left_460x220.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(24f, -24f),
                new Vector2(460f, 220f),
                new Vector2(0f, 1f)
            );

            CreateImage(
                panel,
                "SpeedIcon",
                "icon_speed.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(34f, -32f),
                new Vector2(68f, 68f),
                new Vector2(0f, 1f)
            );

            TMP_Text speedLabel = CreateText(
                panel,
                "SpeedLabel",
                "SPEED",
                25f,
                TextAlignmentOptions.MidlineLeft,
                new Vector2(122f, -30f),
                new Vector2(120f, 40f),
                new Vector2(0f, 1f)
            );
            speedLabel.fontStyle = FontStyles.Bold;

            view.speedText = CreateText(
                panel,
                "SpeedValue",
                "0 m/s",
                28f,
                TextAlignmentOptions.MidlineRight,
                new Vector2(242f, -30f),
                new Vector2(170f, 40f),
                new Vector2(0f, 1f)
            );
            view.speedText.color = new Color32(72, 230, 240, 255);
            view.speedText.fontStyle = FontStyles.Bold;

            CreateImage(
                panel,
                "SpeedBarBackground",
                "bar_speed_segmented_background.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(122f, -78f),
                new Vector2(290f, 18f),
                new Vector2(0f, 1f)
            );

            RectTransform speedFillRect = CreateImage(
                panel,
                "SpeedBarFill",
                "bar_speed_segmented_fill.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(122f, -78f),
                new Vector2(290f, 18f),
                new Vector2(0f, 1f)
            );

            view.speedFill = speedFillRect.GetComponent<Image>();
            view.speedFill.type = Image.Type.Filled;
            view.speedFill.fillMethod = Image.FillMethod.Horizontal;
            view.speedFill.fillOrigin = 0;
            view.speedFill.fillAmount = 0f;

            CreateImage(
                panel,
                "Divider",
                "divider_400x8.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(30f, -116f),
                new Vector2(400f, 8f),
                new Vector2(0f, 1f)
            );

            CreateImage(
                panel,
                "HealthIcon",
                "icon_health.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(34f, -138f),
                new Vector2(68f, 68f),
                new Vector2(0f, 1f)
            );

            TMP_Text healthLabel = CreateText(
                panel,
                "HealthLabel",
                "HEALTH",
                25f,
                TextAlignmentOptions.MidlineLeft,
                new Vector2(122f, -138f),
                new Vector2(120f, 40f),
                new Vector2(0f, 1f)
            );
            healthLabel.fontStyle = FontStyles.Bold;

            view.healthText = CreateText(
                panel,
                "HealthValue",
                "100%",
                28f,
                TextAlignmentOptions.MidlineRight,
                new Vector2(282f, -138f),
                new Vector2(130f, 40f),
                new Vector2(0f, 1f)
            );
            view.healthText.color = new Color32(0, 246, 94, 255);
            view.healthText.fontStyle = FontStyles.Bold;

            CreateImage(
                panel,
                "HealthBarBackground",
                "bar_health_background.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(122f, -184f),
                new Vector2(290f, 22f),
                new Vector2(0f, 1f)
            );

            RectTransform healthFillRect = CreateImage(
                panel,
                "HealthBarFill",
                "bar_health_fill.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(122f, -184f),
                new Vector2(290f, 22f),
                new Vector2(0f, 1f)
            );

            view.healthFill = healthFillRect.GetComponent<Image>();
            view.healthFill.type = Image.Type.Filled;
            view.healthFill.fillMethod = Image.FillMethod.Horizontal;
            view.healthFill.fillOrigin = 0;
            view.healthFill.fillAmount = 1f;
        }

        #endregion

        #region Right panel

        static void CreateRightPanel(Transform parent, SpaceHUDView view)
        {
            RectTransform panel = CreateImage(
                parent,
                "RightPanel",
                "panel_right_460x220.png",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-24f, -24f),
                new Vector2(460f, 220f),
                new Vector2(1f, 1f)
            );

            CreateImage(
                panel,
                "DistanceIcon",
                "icon_distance.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(34f, -32f),
                new Vector2(68f, 68f),
                new Vector2(0f, 1f)
            );

            TMP_Text distanceLabel = CreateText(
                panel,
                "DistanceLabel",
                "CLOSEST DISTANCE",
                24f,
                TextAlignmentOptions.MidlineLeft,
                new Vector2(122f, -30f),
                new Vector2(220f, 40f),
                new Vector2(0f, 1f)
            );
            distanceLabel.fontStyle = FontStyles.Bold;

            view.distanceText = CreateText(
                panel,
                "DistanceValue",
                "0 m",
                28f,
                TextAlignmentOptions.MidlineRight,
                new Vector2(340f, -30f),
                new Vector2(72f, 40f),
                new Vector2(0f, 1f)
            );
            view.distanceText.color = new Color32(72, 230, 240, 255);
            view.distanceText.fontStyle = FontStyles.Bold;

            CreateImage(
                panel,
                "Divider",
                "divider_400x8.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(30f, -116f),
                new Vector2(400f, 8f),
                new Vector2(0f, 1f)
            );

            CreateImage(
                panel,
                "CoinIcon",
                "icon_coin.png",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(34f, -138f),
                new Vector2(68f, 68f),
                new Vector2(0f, 1f)
            );

            TMP_Text coinsLabel = CreateText(
                panel,
                "CoinsLabel",
                "COINS",
                25f,
                TextAlignmentOptions.MidlineLeft,
                new Vector2(122f, -138f),
                new Vector2(120f, 40f),
                new Vector2(0f, 1f)
            );
            coinsLabel.fontStyle = FontStyles.Bold;

            view.coinsText = CreateText(
                panel,
                "CoinsValue",
                "0 / 5",
                28f,
                TextAlignmentOptions.MidlineRight,
                new Vector2(282f, -138f),
                new Vector2(130f, 40f),
                new Vector2(0f, 1f)
            );
            view.coinsText.color = new Color32(255, 211, 60, 255);
            view.coinsText.fontStyle = FontStyles.Bold;
        }

        #endregion

        #region Reticle

        static void CreateCenterReticle(Transform parent)
        {
            CreateImage(
                parent,
                "CenterReticle",
                "reticle_center.png",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -145f),
                new Vector2(72f, 72f),
                new Vector2(0.5f, 0.5f)
            );

            CreateImage(
                parent,
                "BottomReticle",
                "reticle_bottom_arc.png",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 44f),
                new Vector2(760f, 150f),
                new Vector2(0.5f, 0f)
            );
        }

        #endregion

        #region Factory methods

        static RectTransform CreateImage(
            Transform parent,
            string objectName,
            string spriteFile,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Vector2 size,
            Vector2 pivot)
        {
            GameObject gameObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = gameObject.GetComponent<Image>();
            image.sprite = LoadSprite(spriteFile);
            image.preserveAspect = false;
            image.raycastTarget = false;

            return rect;
        }

        static TMP_Text CreateText(
            Transform parent,
            string objectName,
            string text,
            float fontSize,
            TextAlignmentOptions alignment,
            Vector2 anchoredPosition,
            Vector2 size,
            Vector2 pivot)
        {
            GameObject gameObject = new GameObject(
                objectName,
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );

            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = pivot;
            rect.anchorMax = pivot;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            TextMeshProUGUI tmp = gameObject.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = alignment;
            tmp.color = Color.white;
            tmp.raycastTarget = false;
            tmp.enableWordWrapping = false;

            return tmp;
        }

        static Sprite LoadSprite(string fileName)
        {
            string path = $"{HudRootPath}/{fileName}";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite != null)
                return sprite;

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();

                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }

            if (sprite == null)
                Debug.LogError($"Space HUD sprite not found: {path}");

            return sprite;
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

        #endregion
    }
}

#endif
