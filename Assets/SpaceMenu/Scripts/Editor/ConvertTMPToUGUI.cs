#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;

public static class ConvertTMPToUGUI
{
    [MenuItem("Tools/Lucy/UI/Convert TextMeshPro to UGUI")]
    public static void Convert()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Select a root GameObject first.");
            return;
        }

        TextMeshPro[] texts =
            Selection.activeGameObject.GetComponentsInChildren<TextMeshPro>(true);

        foreach (TextMeshPro source in texts)
            ConvertText(source);

        Debug.Log($"Converted {texts.Length} TextMeshPro objects to TextMeshProUGUI.");
    }

    static void ConvertText(TextMeshPro source)
    {
        Transform oldTransform = source.transform;
        Transform parent = oldTransform.parent;

        // Учитываем масштаб старого 3D TMP.
        float scale = Mathf.Abs(oldTransform.localScale.y);
        float approximateFontSize = source.fontSize * scale;

        // Защита от странных/нулевых scale.
        if (approximateFontSize <= 0.01f)
            approximateFontSize = source.fontSize;

        // Сохраняем положение до удаления объекта.
        Vector3 localPosition = oldTransform.localPosition;
        Quaternion localRotation = oldTransform.localRotation;
        int siblingIndex = oldTransform.GetSiblingIndex();
        bool active = source.gameObject.activeSelf;

        GameObject newObject = new GameObject(
            oldTransform.name,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(TextMeshProUGUI)
        );

        Undo.RegisterCreatedObjectUndo(newObject, "Convert TMP to UGUI");

        RectTransform rect = newObject.GetComponent<RectTransform>();
        TextMeshProUGUI target = newObject.GetComponent<TextMeshProUGUI>();

        rect.SetParent(parent, false);
        rect.SetSiblingIndex(siblingIndex);

        rect.localPosition = localPosition;
        rect.localRotation = localRotation;

        // Масштаб уже перенесли в размер шрифта.
        rect.localScale = Vector3.one;

        // ---------------------------------------------------------
        // Text
        // ---------------------------------------------------------

        target.text = source.text;

        target.font = source.font;
        target.fontSharedMaterial = source.fontSharedMaterial;

        target.fontSize = approximateFontSize;
        target.fontStyle = source.fontStyle;
        target.color = source.color;

        target.alignment = source.alignment;

        target.enableWordWrapping = source.enableWordWrapping;
        target.overflowMode = source.overflowMode;

        target.characterSpacing = source.characterSpacing;
        target.wordSpacing = source.wordSpacing;
        target.lineSpacing = source.lineSpacing;
        target.paragraphSpacing = source.paragraphSpacing;

        target.richText = source.richText;

        // ---------------------------------------------------------
        // Auto Size
        // ---------------------------------------------------------

        target.enableAutoSizing = source.enableAutoSizing;

        if (source.enableAutoSizing)
        {
            target.fontSizeMin = source.fontSizeMin * scale;
            target.fontSizeMax = source.fontSizeMax * scale;
        }

        target.margin = source.margin;

        // ---------------------------------------------------------
        // Rect size
        // ---------------------------------------------------------

        Vector2 preferredSize = target.GetPreferredValues(
            target.text,
            10000f,
            10000f
        );

        rect.sizeDelta = new Vector2(
            Mathf.Max(preferredSize.x, 10f),
            Mathf.Max(preferredSize.y, 10f)
        );

        newObject.SetActive(active);

        Undo.DestroyObjectImmediate(source.gameObject);
    }
}
#endif