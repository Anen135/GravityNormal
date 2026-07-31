using System;
using UnityEngine;

public class Gravitation : MonoBehaviour
{
    [Header("Raycast Grab Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float grabRange = 8f;
    [SerializeField] private float holdDistance = 3f;
    [SerializeField] private float smoothFactor = 12f;
    [SerializeField] private float throwForce = 20f;
    [SerializeField] private LayerMask grabMask = ~0;
    [SerializeField] private bool debugRay = false;
    
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float rotationSmooth = 10f;
    
    [Header("Debug Settings")]
    [SerializeField] private bool showDirection = true;
    [SerializeField] private Color LineColor = Color.cyan;
    [SerializeField] private Color SphereColor = Color.cyan;
    [SerializeField] private float LineLength = 2.0f;
    [SerializeField] private float LineHeight = 1f;

    [Header("References")]
    [SerializeField] private Transform holdAnchor;

    private Rigidbody grabbedBody;
    private bool isHolding;
    private Quaternion targetRotation;

    public bool IsHolding => isHolding;
    public event Action<Rigidbody> OnGrabbed;
    public event Action<Rigidbody> OnReleased;
    public event Action<Rigidbody> OnThrown;

    private void Awake()
    {
        if (playerCamera == null)
            Debug.LogWarning($"{nameof(Gravitation)}: No camera assigned. Please assign in inspector.", this);

        if (holdAnchor == null)
        {
            holdAnchor = new GameObject($"{name}_HoldAnchor").transform;
            holdAnchor.SetParent(transform);
            holdAnchor.localPosition = new Vector3(0, 1.5f, 1.5f);
        }
    }

    private void FixedUpdate()
    {
        if (!isHolding || grabbedBody == null)
            return;

        // Обновляем позицию точки удержания перед камерой
        Vector3 targetPos = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
        holdAnchor.position = Vector3.Lerp(holdAnchor.position, targetPos, Time.fixedDeltaTime * smoothFactor);

        // Перемещаем объект к якорю
        Vector3 moveDir = holdAnchor.position - grabbedBody.position;
        grabbedBody.linearVelocity = moveDir * smoothFactor;

        // Плавное вращение к цели
        grabbedBody.MoveRotation(Quaternion.Slerp(grabbedBody.rotation, targetRotation, Time.fixedDeltaTime * rotationSmooth));
    }

    private void Update()
    {
        if (debugRay && playerCamera != null)
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * grabRange, Color.yellow);
    }

    public GameObject GetHeldObject() => isHolding ? grabbedBody.gameObject : null;

    /// <summary>
    /// Пытается захватить ближайший Rigidbody в пределах дистанции.
    /// </summary>
    public void TryGrab()
    {
        if (isHolding || playerCamera == null)
            return;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, grabRange, grabMask))
        {
            if (hit.rigidbody != null)
            {
                grabbedBody = hit.rigidbody;
                grabbedBody.useGravity = false;
                grabbedBody.linearDamping = 10f;

                isHolding = true;
                targetRotation = grabbedBody.rotation;
                OnGrabbed?.Invoke(grabbedBody);
            }
        }
    }

    /// <summary>
    /// Отпускает удерживаемый объект без импульса.
    /// </summary>
    public void Release()
    {
        if (!isHolding || grabbedBody == null)
            return;

        grabbedBody.useGravity = true;
        grabbedBody.linearDamping = 0f;
        OnReleased?.Invoke(grabbedBody);

        grabbedBody = null;
        isHolding = false;
    }

    /// <summary>
    /// Отпускает объект, придавая ему импульс от камеры.
    /// </summary>
    public void Throw()
    {
        if (!isHolding || grabbedBody == null || playerCamera == null)
            return;

        grabbedBody.useGravity = true;
        grabbedBody.linearDamping = 0f;
        grabbedBody.linearVelocity = playerCamera.transform.forward * throwForce;
        OnThrown?.Invoke(grabbedBody);

        grabbedBody = null;
        isHolding = false;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && playerCamera == null)
            return; // Без камеры в редакторе нечего рисовать

        // Используем текущую камеру в редакторе, если не в игре
        Transform camTransform = Application.isPlaying
            ? playerCamera?.transform
            : Camera.current?.transform;

        if (camTransform == null)
            return;

        if (showDirection)
        {
            Vector3 origin = camTransform.position;
            Vector3 forward = camTransform.forward;
            Vector3 up = camTransform.up;

            // Линия направления взгляда
            Gizmos.color = LineColor;
            Gizmos.DrawLine(origin, origin + forward * LineLength);

            // Дополнительные линии для визуализации "высоты" (например, крест)
            Vector3 right = camTransform.right;
            Gizmos.DrawLine(origin + up * LineHeight, origin + up * LineHeight + forward * LineLength);
            Gizmos.DrawLine(origin - right * 0.1f, origin + right * 0.1f); // маленький крестик у origin
        }

        // Визуализация точки удержания (holdAnchor), если она существует
        if (holdAnchor != null)
        {
            Gizmos.color = SphereColor;
            Gizmos.DrawWireSphere(holdAnchor.position, 0.1f);
        }

        // Визуализация grabRange как луча (опционально)
        if (debugRay)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(camTransform.position, camTransform.position + camTransform.forward * grabRange);
        }
    }

    public void RotateHeldObject(Vector2 input)
    {
        if (!isHolding || grabbedBody == null)
            return;
        
        // Вращаем объект вокруг локальных осей камеры
        Quaternion yaw = Quaternion.AngleAxis(input.x * rotationSpeed * Time.deltaTime, playerCamera.transform.up);
        Quaternion pitch = Quaternion.AngleAxis(-input.y * rotationSpeed * Time.deltaTime, playerCamera.transform.right);
        targetRotation = yaw * pitch * targetRotation;
    }
}