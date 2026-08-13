using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class GravityBeam : MonoBehaviour
{
    [Header("Settings Gravity Beam")]
    [Min(1)] public int lineCount = 3;
    [Min(2)] public int segments = 32;
    [Min(0f)] public float beamWidth = 0.025f;
    [Min(0f)] public float glowWidth = 0.1f;
    [Min(0f)] public float waveAmplitude = 0.06f;
    [Min(0f)] public float waveFrequency = 3f;
    [Min(0f)] public float waveSpeed = 1.5f;

    [ColorUsage(true, true)]
    public Color beamColor = new(0.2f, 1.2f, 2.5f, 1f);

    [ColorUsage(true, true)]
    public Color glowColor = new(0.1f, 0.6f, 2f, 0.15f);

    public Material beamMaterial;
    public Material glowMaterial;

    [Header("Debug")]
    public bool isVisible;
    public Vector3 startPosition;
    public Vector3 endPosition;
    public List<LineRenderer> beamRenderers = new();
    public List<LineRenderer> glowRenderers = new();

    void Awake()
    {
        SyncRenderers();
        Configure();
        SetVisible(false);
    }

    void OnValidate()
    {
        if (!Application.isPlaying) return;

        SyncRenderers();
        Configure();
    }

    void OnDisable() => SetVisible(false);

    void LateUpdate()
    {
        if (!isVisible) return;
        DrawInternal(startPosition, endPosition);
    }

    public void Draw(Vector3 start, Vector3 end)
    {
        startPosition = start;
        endPosition = end;

        SyncRenderers();
        SetVisible(true);
        DrawInternal(start, end);
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;

        int count = Mathf.Min(beamRenderers.Count, glowRenderers.Count);

        for (int i = 0; i < count; i++)
        {
            bool activeLine = i < lineCount;

            if (beamRenderers[i] != null) beamRenderers[i].enabled = visible && activeLine;
            if (glowRenderers[i] != null) glowRenderers[i].enabled = visible && activeLine;
        }
    }

    void DrawInternal(Vector3 start, Vector3 end)
    {
        int count = Mathf.Max(segments, 2);
        int activeLineCount = Mathf.Max(lineCount, 1);

        Vector3 direction = end - start;
        float distance = direction.magnitude;

        if (distance <= 0.0001f)
        {
            SetVisible(false);
            return;
        }

        Vector3 forward = direction / distance;
        Vector3 right = Vector3.Cross(forward, Vector3.up);

        if (right.sqrMagnitude < 0.001f) right = Vector3.Cross(forward, Vector3.right);

        right.Normalize();

        Vector3 up = Vector3.Cross(right, forward).normalized;
        float time = Time.time * waveSpeed * Mathf.PI * 2f;

        for (int lineIndex = 0; lineIndex < activeLineCount; lineIndex++)
        {
            LineRenderer beam = beamRenderers[lineIndex];
            LineRenderer glow = glowRenderers[lineIndex];

            beam.positionCount = count;
            glow.positionCount = count;

            float linePhase = lineIndex / (float)activeLineCount * Mathf.PI * 2f;

            for (int i = 0; i < count; i++)
            {
                float t = i / (float)(count - 1);
                Vector3 position = Vector3.Lerp(start, end, t);
                float envelope = Mathf.Sin(t * Mathf.PI);
                float angle = t * waveFrequency * Mathf.PI * 2f + time + linePhase;

                Vector3 wave =
                    right * Mathf.Sin(angle) +
                    up * Mathf.Cos(angle);

                position += wave * waveAmplitude * envelope;

                beam.SetPosition(i, position);
                glow.SetPosition(i, position);
            }
        }
    }

    void SyncRenderers()
    {
        int required = Mathf.Max(lineCount, 1);

        while (beamRenderers.Count < required)
        {
            int index = beamRenderers.Count;

            beamRenderers.Add(CreateRenderer($"Gravity Beam {index + 1}"));
            glowRenderers.Add(CreateRenderer($"Gravity Beam Glow {index + 1}"));
        }

        for (int i = 0; i < beamRenderers.Count; i++)
        {
            bool active = i < required;

            if (beamRenderers[i] != null) beamRenderers[i].gameObject.SetActive(active);
            if (i < glowRenderers.Count && glowRenderers[i] != null) glowRenderers[i].gameObject.SetActive(active);
        }

        Configure();
    }

    LineRenderer CreateRenderer(string objectName)
    {
        GameObject rendererObject = new(objectName);
        rendererObject.transform.SetParent(transform, false);

        LineRenderer renderer = rendererObject.AddComponent<LineRenderer>();
        renderer.enabled = false;

        return renderer;
    }

    void Configure()
    {
        int count = Mathf.Min(beamRenderers.Count, glowRenderers.Count);

        for (int i = 0; i < count; i++)
        {
            ConfigureRenderer(beamRenderers[i], beamWidth, beamColor, beamMaterial);
            ConfigureRenderer(glowRenderers[i], glowWidth, glowColor, glowMaterial);
        }
    }

    void ConfigureRenderer(LineRenderer renderer, float width, Color color, Material material)
    {
        if (renderer == null) return;

        renderer.useWorldSpace = true;
        renderer.loop = false;
        renderer.startWidth = width;
        renderer.endWidth = width * 0.25f;
        renderer.startColor = color;
        renderer.endColor = new Color(color.r, color.g, color.b, 0f);
        renderer.numCapVertices = 4;
        renderer.numCornerVertices = 4;

        if (material != null) renderer.sharedMaterial = material;
    }
}
