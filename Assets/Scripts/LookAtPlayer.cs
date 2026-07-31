using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Следит за игроком глазами и шеей.
///
/// НАСТРОЙКА ГЛАЗ:
///   1. Вручную поверни кости глаз так чтобы они смотрели прямо вперёд.
///   2. Нажми "Capture Rest Pose" в инспекторе.
///   3. Укажи Eye Right Axis и Eye Up Axis — те локальные оси кости
///      которые в нейтральной позе смотрят вправо и вверх персонажа.
///      (Включи Debug Bone Axes и смотри стрелки на костях в SceneView)
/// </summary>
public class LookAtPlayer : MonoBehaviour
{
    // ─── Кости ───────────────────────────────────────────────────────────────
    [Header("Bones")]
    public Transform neckBone;
    public Transform leftEyeBone;
    public Transform rightEyeBone;

    // ─── Поза покоя ──────────────────────────────────────────────────────────
    [Header("Rest Pose  ←  нажми Capture Rest Pose")]
    public Quaternion neckRestPose     = Quaternion.identity;
    public Quaternion leftEyeRestPose  = Quaternion.identity;
    public Quaternion rightEyeRestPose = Quaternion.identity;
    public bool restPoseCaptured       = false;

    // ─── Цель ────────────────────────────────────────────────────────────────
    [Header("Target")]
    public Transform target;
    public Vector3   targetOffset = new Vector3(0f, 1.6f, 0f);

    // ─── Лимиты шеи ──────────────────────────────────────────────────────────
    [Header("Neck Limits (degrees)")]
    public float neckYawLimit   = 60f;
    public float neckPitchLimit = 30f;

    // ─── Лимиты глаз по осям ──────────────────────────────────────────────────
    [Header("Eye Rotation Axes & Limits")]
    [Tooltip("Лимит по локальной оси X кости глаза (degrees). 0 = отключено.")]
    public float eyeLimitX = 0f;
    [Tooltip("Лимит по локальной оси Y кости глаза (degrees). 0 = отключено.")]
    public float eyeLimitY = 20f;
    [Tooltip("Лимит по локальной оси Z кости глаза (degrees). 0 = отключено.")]
    public float eyeLimitZ = 0f;

    [Tooltip("Инвертировать направление по X")]
    public bool eyeInvertX = false;
    [Tooltip("Инвертировать направление по Y")]
    public bool eyeInvertY = false;
    [Tooltip("Инвертировать направление по Z")]
    public bool eyeInvertZ = false;

    // ─── Ось шеи ─────────────────────────────────────────────────────────────
    [Header("Neck Bone Axes")]
    [Tooltip("Локальная ось шеи смотрящая ВПЕРЁД.\nBlender DEF: (0,1,0)  Standard: (0,0,1)")]
    public Vector3 neckForwardAxis = Vector3.forward;
    public Vector3 neckUpAxis      = Vector3.up;

    // ─── Веса и скорости ─────────────────────────────────────────────────────
    [Header("Weights & Speed")]
    [Range(0f, 1f)] public float neckWeight  = 0.6f;
    [Range(0f, 1f)] public float eyeWeight   = 1f;
    public float trackSpeed      = 80f;
    public float returnSpeed     = 40f;
    public float visionConeAngle = 90f;

    // ─── Debug ───────────────────────────────────────────────────────────────
    [Header("Debug")]
    public bool showGizmos    = true;
    public bool debugBoneAxes = false;

    // ─── Приватное ───────────────────────────────────────────────────────────
    private Quaternion _neckCurrent;
    private Quaternion _leftEyeCurrent;
    private Quaternion _rightEyeCurrent;
    private bool       _initialized;
    private bool       _inRange;

    // ─────────────────────────────────────────────────────────────────────────

    void OnEnable() { _initialized = false; }

    void LateUpdate()
    {
        if (neckBone == null) return;

        Quaternion neckNeutral     = restPoseCaptured ? neckRestPose     : neckBone.localRotation;
        Quaternion leftEyeNeutral  = restPoseCaptured ? leftEyeRestPose  : (leftEyeBone  != null ? leftEyeBone.localRotation  : Quaternion.identity);
        Quaternion rightEyeNeutral = restPoseCaptured ? rightEyeRestPose : (rightEyeBone != null ? rightEyeBone.localRotation : Quaternion.identity);

        if (!_initialized)
        {
            _neckCurrent     = neckNeutral;
            _leftEyeCurrent  = leftEyeNeutral;
            _rightEyeCurrent = rightEyeNeutral;
            _initialized     = true;
        }

        if (target == null)
        {
            RotateTowards(ref _neckCurrent,     neckNeutral,     returnSpeed);
            RotateTowards(ref _leftEyeCurrent,  leftEyeNeutral,  returnSpeed);
            RotateTowards(ref _rightEyeCurrent, rightEyeNeutral, returnSpeed);
            ApplyBones();
            return;
        }

        Vector3 targetPoint  = target.position + targetOffset;
        Vector3 neckWorldFwd = neckBone.TransformDirection(neckForwardAxis);
        Vector3 toTarget     = targetPoint - neckBone.position;
        _inRange = Vector3.Angle(neckWorldFwd, toTarget) < visionConeAngle;

        // ── Шея ──────────────────────────────────────────────────────────────
        Quaternion neckGoal = neckNeutral;
        if (_inRange)
        {
            Quaternion lookLocal = WorldLookToLocal(neckBone, toTarget, neckForwardAxis, neckUpAxis);
            lookLocal = ClampEulerSimple(lookLocal, neckYawLimit, neckPitchLimit);
            neckGoal  = Quaternion.Slerp(neckNeutral, lookLocal, neckWeight);
        }
        RotateTowards(ref _neckCurrent, neckGoal, _inRange ? trackSpeed : returnSpeed);

        // ── Глаза ────────────────────────────────────────────────────────────
        if (leftEyeBone != null)
        {
            Quaternion goal = _inRange
                ? CalcEyeGoal(leftEyeBone,  leftEyeNeutral,  targetPoint)
                : leftEyeNeutral;
            RotateTowards(ref _leftEyeCurrent, goal, _inRange ? trackSpeed * 1.5f : returnSpeed);
        }

        if (rightEyeBone != null)
        {
            Quaternion goal = _inRange
                ? CalcEyeGoal(rightEyeBone, rightEyeNeutral, targetPoint)
                : rightEyeNeutral;
            RotateTowards(ref _rightEyeCurrent, goal, _inRange ? trackSpeed * 1.5f : returnSpeed);
        }

        ApplyBones();
    }

    void ApplyBones()
    {
        neckBone.localRotation = _neckCurrent;
        if (leftEyeBone  != null) leftEyeBone.localRotation  = _leftEyeCurrent;
        if (rightEyeBone != null) rightEyeBone.localRotation = _rightEyeCurrent;
    }

    // ─── Расчёт поворота глаза ───────────────────────────────────────────────
    // Для каждой из трёх локальных осей (X/Y/Z) вычисляем угол к цели
    // и применяем его если лимит > 0. Инвертировать можно отдельно каждую.

    Quaternion CalcEyeGoal(Transform eye, Quaternion neutral, Vector3 targetPoint)
    {
        if (eye == null) return neutral;

        Quaternion parentRot       = eye.parent != null ? eye.parent.rotation : Quaternion.identity;
        Quaternion eyeWorldNeutral = parentRot * neutral;

        // Вектор к цели в локальном пространстве нейтральной позы кости
        Vector3 toTargetWorld = (targetPoint - eye.position).normalized;
        Vector3 toTargetLocal = Quaternion.Inverse(eyeWorldNeutral) * toTargetWorld;

        // Углы по каждой локальной оси — просто atan2 проекций
        // Ось X: поворот в плоскости YZ → угол между Z и проекцией на YZ
        float angleX = Mathf.Atan2(-toTargetLocal.y, toTargetLocal.z) * Mathf.Rad2Deg;
        // Ось Y: поворот в плоскости XZ → угол между Z и проекцией на XZ
        float angleY = Mathf.Atan2( toTargetLocal.x, toTargetLocal.z) * Mathf.Rad2Deg;
        // Ось Z: поворот в плоскости XY
        float angleZ = Mathf.Atan2( toTargetLocal.y, toTargetLocal.x) * Mathf.Rad2Deg;

        // Инверсия
        if (eyeInvertX) angleX = -angleX;
        if (eyeInvertY) angleY = -angleY;
        if (eyeInvertZ) angleZ = -angleZ;

        // Клампим только активные оси (лимит > 0)
        float rx = eyeLimitX > 0f ? Mathf.Clamp(angleX, -eyeLimitX, eyeLimitX) : 0f;
        float ry = eyeLimitY > 0f ? Mathf.Clamp(angleY, -eyeLimitY, eyeLimitY) : 0f;
        float rz = eyeLimitZ > 0f ? Mathf.Clamp(angleZ, -eyeLimitZ, eyeLimitZ) : 0f;

        // Применяем как локальный поворот относительно нейтрали
        Quaternion localDelta = Quaternion.Euler(rx, ry, rz);
        Quaternion localGoal  = neutral * localDelta;

        return Quaternion.Slerp(neutral, localGoal, eyeWeight);
    }

    // ─── Вспомогательные ─────────────────────────────────────────────────────

    static Quaternion WorldLookToLocal(Transform bone, Vector3 worldDir,
                                        Vector3 fwdAxis, Vector3 upAxis)
    {
        if (worldDir.sqrMagnitude < 0.0001f) return bone.localRotation;

        Vector3 worldUp   = bone.TransformDirection(upAxis);
        Quaternion look   = Quaternion.LookRotation(worldDir.normalized, worldUp);
        Quaternion corr   = Quaternion.FromToRotation(Vector3.forward, fwdAxis);
        look              = look * Quaternion.Inverse(corr);

        Quaternion parentInv = bone.parent != null
            ? Quaternion.Inverse(bone.parent.rotation)
            : Quaternion.identity;

        return parentInv * look;
    }

    static Quaternion ClampEulerSimple(Quaternion q, float yawLim, float pitchLim)
    {
        Vector3 e = q.eulerAngles;
        e.x = Mathf.Clamp(Wrap(e.x), -pitchLim, pitchLim);
        e.y = Mathf.Clamp(Wrap(e.y), -yawLim,   yawLim);
        return Quaternion.Euler(e);
    }

    static float Wrap(float a)
    {
        a %= 360f;
        if (a >  180f) a -= 360f;
        if (a < -180f) a += 360f;
        return a;
    }

    static void RotateTowards(ref Quaternion cur, Quaternion target, float speed)
    {
        cur = Quaternion.RotateTowards(cur, target, speed * Time.deltaTime);
    }

    // ─── Rest Pose ───────────────────────────────────────────────────────────

    public void CaptureRestPose()
    {
        if (neckBone     != null) neckRestPose     = neckBone.localRotation;
        if (leftEyeBone  != null) leftEyeRestPose  = leftEyeBone.localRotation;
        if (rightEyeBone != null) rightEyeRestPose = rightEyeBone.localRotation;
        restPoseCaptured = true;
        Debug.Log($"[LookAtPlayer] Rest pose captured.\n" +
                  $"  Neck:      {neckRestPose.eulerAngles:F2}\n" +
                  $"  Left Eye:  {leftEyeRestPose.eulerAngles:F2}\n" +
                  $"  Right Eye: {rightEyeRestPose.eulerAngles:F2}");
    }

    public void ClearRestPose()
    {
        restPoseCaptured = false;
        Debug.Log("[LookAtPlayer] Rest pose cleared.");
    }

    // ─── Gizmos ──────────────────────────────────────────────────────────────

    void OnDrawGizmos()
    {
        if (!showGizmos || neckBone == null) return;

        Vector3 fwd = neckBone.TransformDirection(
            Application.isPlaying ? neckForwardAxis : Vector3.forward);

        Gizmos.color = _inRange ? Color.green : Color.red;
        DrawCone(neckBone.position, fwd, visionConeAngle, 1.2f);

        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(neckBone.position, target.position + targetOffset);
            Gizmos.DrawWireSphere(target.position + targetOffset, 0.04f);
        }

        if (debugBoneAxes)
        {
            DrawAxes(neckBone,     0.15f);
            DrawAxes(leftEyeBone,  0.08f);
            DrawAxes(rightEyeBone, 0.08f);
        }
    }

    static void DrawAxes(Transform t, float s)
    {
        if (t == null) return;
        Gizmos.color = Color.red;   Gizmos.DrawRay(t.position, t.right   * s);
        Gizmos.color = Color.green; Gizmos.DrawRay(t.position, t.up      * s);
        Gizmos.color = Color.blue;  Gizmos.DrawRay(t.position, t.forward * s);
    }

    static void DrawCone(Vector3 o, Vector3 fwd, float half, float len)
    {
        Vector3 r = Vector3.Cross(fwd, Vector3.up).normalized;
        if (r.sqrMagnitude < 0.01f) r = Vector3.right;
        Vector3 u = Vector3.Cross(r, fwd).normalized;
        float   rad = half * Mathf.Deg2Rad;
        Vector3 prev = Vector3.zero;
        for (int i = 0; i <= 16; i++)
        {
            float   t = (float)i / 16 * Mathf.PI * 2f;
            Vector3 d = fwd * Mathf.Cos(rad) + (r * Mathf.Cos(t) + u * Mathf.Sin(t)) * Mathf.Sin(rad);
            Vector3 p = o + d.normalized * len;
            if (i > 0)      Gizmos.DrawLine(prev, p);
            if (i % 4 == 0) Gizmos.DrawLine(o,    p);
            prev = p;
        }
    }
}

// ─── Кастомный инспектор ──────────────────────────────────────────────────────
#if UNITY_EDITOR
[CustomEditor(typeof(LookAtPlayer))]
public class LookAtPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LookAtPlayer s = (LookAtPlayer)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Rest Pose Tools", EditorStyles.boldLabel);

        // Подсказка по осям
        EditorGUILayout.HelpBox(
            "Eye Limit X/Y/Z — лимиты по локальным осям кости глаза.\n" +
            "Включи Debug Bone Axes чтобы увидеть оси на кости в SceneView:\n" +
            "  🔴 красная = X    🟢 зелёная = Y    🔵 синяя = Z\n" +
            "Выставь лимит только на нужных осях, остальные оставь 0.\n" +
            "Если глаз крутится в обратную сторону — включи Invert для этой оси.",
            MessageType.None);

        EditorGUILayout.Space(4);

        GUI.backgroundColor = s.restPoseCaptured
            ? new Color(0.4f, 1f, 0.5f)
            : new Color(1f, 0.75f, 0.2f);

        if (GUILayout.Button(
            s.restPoseCaptured
                ? "✔  Rest Pose захвачена  (нажми чтобы перезахватить)"
                : "⚠  Capture Rest Pose",
            GUILayout.Height(34)))
        {
            Undo.RecordObject(s, "Capture Rest Pose");
            s.CaptureRestPose();
            EditorUtility.SetDirty(s);
        }
        GUI.backgroundColor = Color.white;

        if (s.restPoseCaptured)
        {
            if (GUILayout.Button("✖  Clear Rest Pose", GUILayout.Height(22)))
            {
                Undo.RecordObject(s, "Clear Rest Pose");
                s.ClearRestPose();
                EditorUtility.SetDirty(s);
            }

            EditorGUILayout.HelpBox(
                $"Neck:       {s.neckRestPose.eulerAngles:F1}\n" +
                $"Left Eye:   {s.leftEyeRestPose.eulerAngles:F1}\n" +
                $"Right Eye:  {s.rightEyeRestPose.eulerAngles:F1}",
                MessageType.Info);
        }
    }
}
#endif
