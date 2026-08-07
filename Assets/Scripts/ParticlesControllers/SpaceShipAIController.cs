using Lucy.Features.Projectiles.Runtime;
using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GravityBody))]
    public class SpaceShipAIController : MonoBehaviour
    {
        public enum State
        {
            AttackRun,
            Retreat
        }

        #region Inspector

        [Header("References")]
        public SpaceShipEngine engine;
        public AISpaceShipWeapon weapon;
        public Rigidbody rb;
        public GravityBody gravityBody;
        public Transform target;

        [Header("Settings SpaceShip AI Controller Combat")]
        [Min(0f)] public float fireDistance = 50f;
        [Min(0f)] public float retreatDistance = 12f;
        [Min(0f)] public float returnDistance = 40f;

        [Range(0f, 180f)] public float fireAngle = 5f;
        [Range(0f, 180f)] public float thrustAngle = 25f;
        [Range(0f, 180f)] public float retreatThrustAngle = 20f;

        [Min(0f)] public float fireInterval = 0.25f;
        [Min(0f)] public float turnPredictionTime = 1.5f;

        [Header("Settings SpaceShip AI Controller Stabilization")]
        [Min(1f)] public float fullSteeringAngle = 30f;
        [Min(0f)] public float angularVelocityDamping = 0.6f;

        [Range(0f, 1.5f)]
        public float gravityCompensation = 1f;

        public bool compensateDefaultGravity = true;

        [Min(0.01f)]
        public float lateralStabilizationTime = 0.5f;

        [Min(0f)]
        public float maxLateralStabilizationAcceleration = 20f;

        [Header("Debug")]
        public bool gizmosEnabled;
        public State state;
        public float distanceToTarget;
        public float angleToTarget;
        public float nextFireTime;
        public float calculatedRetreatDistance;

        public Vector3 desiredFlightDirection;
        public Vector3 gravityAcceleration;
        public Vector3 lateralVelocity;
        public Vector3 stabilizationAcceleration;
        public Vector3 localAngularVelocity;

        #endregion

        #region Unity messages

#if UNITY_EDITOR
        void OnValidate()
        {
            engine = engine != null
                ? engine
                : GetComponent<SpaceShipEngine>();

            rb = rb != null
                ? rb
                : GetComponent<Rigidbody>();

            gravityBody = gravityBody != null
                ? gravityBody
                : GetComponent<GravityBody>();
        }
#endif

        void Update()
        {
            if (target == null)
            {
                desiredFlightDirection = Vector3.zero;

                engine.moveDirection = Vector3.zero;
                engine.Roll(0f);
                engine.SetThrusting(false);
                engine.SetBraking(false);

                return;
            }

            Vector3 toTarget = target.position - transform.position;

            distanceToTarget = toTarget.magnitude;

            if (distanceToTarget < 0.001f)
                return;

            Vector3 directionToTarget = toTarget / distanceToTarget;

            angleToTarget = Vector3.Angle(
                transform.forward,
                directionToTarget
            );

            switch (state)
            {
                case State.AttackRun:
                    AttackRun(directionToTarget);
                    break;

                case State.Retreat:
                    Retreat(directionToTarget);
                    break;
            }
        }

        void FixedUpdate()
        {
            ApplyGravityCompensation();
            ApplyLateralStabilization();
        }

        #endregion

        #region AI

        void AttackRun(Vector3 directionToTarget)
        {
            desiredFlightDirection = directionToTarget;

            float closingSpeed = Mathf.Max( 0f, Vector3.Dot(rb.linearVelocity, directionToTarget) );

            calculatedRetreatDistance = retreatDistance + closingSpeed * turnPredictionTime;

            if (distanceToTarget <= calculatedRetreatDistance)
            {
                state = State.Retreat;

                engine.SetThrusting(false);
                engine.SetBraking(true);

                return;
            }

            Aim(directionToTarget);

            engine.SetBraking(false);
            engine.SetThrusting(angleToTarget <= thrustAngle);

            if (distanceToTarget <= fireDistance && angleToTarget <= fireAngle && Time.time >= nextFireTime)
            {
                weapon.Fire();
                nextFireTime = Time.time + fireInterval;
            }
        }

        void Retreat(Vector3 directionToTarget)
        {
            Vector3 retreatDirection = -directionToTarget;

            desiredFlightDirection = retreatDirection;

            Aim(retreatDirection);

            float angleFromTarget = Vector3.Angle(
                transform.forward,
                retreatDirection
            );

            if (angleFromTarget > retreatThrustAngle)
            {
                engine.SetThrusting(false);
                engine.SetBraking(true);

                return;
            }

            engine.SetBraking(false);
            engine.SetThrusting(true);

            if (distanceToTarget >= returnDistance)
            {
                state = State.AttackRun;

                engine.SetThrusting(false);
            }
        }

        void Aim(Vector3 worldDirection)
        {
            Vector3 localDirection =
                transform.InverseTransformDirection(
                    worldDirection.normalized
                );

            localAngularVelocity =
                transform.InverseTransformDirection(
                    rb.angularVelocity
                );

            float yawError =
                Mathf.Atan2(
                    localDirection.x,
                    localDirection.z
                ) * Mathf.Rad2Deg;

            float pitchError =
                Mathf.Atan2(
                    localDirection.y,
                    new Vector2(
                        localDirection.x,
                        localDirection.z
                    ).magnitude
                ) * Mathf.Rad2Deg;

            float yaw =
                yawError / fullSteeringAngle -
                localAngularVelocity.y *
                angularVelocityDamping;

            float pitch =
                pitchError / fullSteeringAngle +
                localAngularVelocity.x *
                angularVelocityDamping;

            float roll =
                -localAngularVelocity.z *
                angularVelocityDamping;

            engine.moveDirection = new Vector3(
                Mathf.Clamp(yaw, -1f, 1f),
                0f,
                Mathf.Clamp(pitch, -1f, 1f)
            );

            engine.Roll(
                Mathf.Clamp(roll, -1f, 1f)
            );
        }

        #endregion

        #region Stabilization

        void ApplyGravityCompensation()
        {
            gravityAcceleration =
                gravityBody.currentAcceleration;

            if (compensateDefaultGravity &&
                rb.useGravity)
            {
                gravityAcceleration +=
                    Physics.gravity;
            }

            rb.AddForce(
                -gravityAcceleration *
                gravityCompensation,
                ForceMode.Acceleration
            );
        }

        void ApplyLateralStabilization()
        {
            stabilizationAcceleration =
                Vector3.zero;

            lateralVelocity =
                Vector3.zero;

            if (desiredFlightDirection.sqrMagnitude < 0.001f)
                return;

            Vector3 direction =
                desiredFlightDirection.normalized;

            lateralVelocity =
                rb.linearVelocity -
                Vector3.Project(
                    rb.linearVelocity,
                    direction
                );

            stabilizationAcceleration =
                -lateralVelocity /
                lateralStabilizationTime;

            stabilizationAcceleration =
                Vector3.ClampMagnitude(
                    stabilizationAcceleration,
                    maxLateralStabilizationAcceleration
                );

            rb.AddForce(
                stabilizationAcceleration,
                ForceMode.Acceleration
            );
        }

        #endregion

        #region Public methods

        public void SetTarget(Transform value)
        {
            target = value;
            state = State.AttackRun;
        }

        #endregion

        #if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (!gizmosEnabled) return;

            Vector3 position = transform.position;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, retreatDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, fireDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, returnDistance);

            if (target != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(position, target.position);
            }

            if (desiredFlightDirection.sqrMagnitude > 0.001f)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay( position, desiredFlightDirection.normalized * 10f );
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawRay( position, transform.forward * 10f );

            if (rb != null && rb.linearVelocity.sqrMagnitude > 0.001f)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawRay( position, rb.linearVelocity );
            }
        }
        #endif
    }
}