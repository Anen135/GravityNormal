using Lucy.Features.Projectiles.Runtime;
using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    public sealed class AISpaceShipWeapon : SpaceShipWeapon
    {
        #region Inspector

        [Header("References AI SpaceShip Weapon")]
        public Transform weaponPivot;

        [Header("Settings AI SpaceShip Weapon")]
        [Min(0f)] public float rotationSpeed = 180f;
        [Range(0f, 20f)] public float fireAngle = 1f;

        [Min(1)] public int predictionIterations = 8;
        [Min(0.1f)] public float maxPredictionTime = 10f;

        [Header("Debug")]
        public Rigidbody targetRigidbody;
        public Rigidbody projectileRigidbody;
        public GameObject target;

        public bool isAimed;

        public float projectileSpeed;
        public float predictionTime;
        public float aimError;

        public Vector3 aimDirection;
        public Vector3 predictedTargetPosition;
        public Vector3 ballisticAimPoint;

        #endregion

        #region Unity messages

        void Awake()
        {
            if (launcher != null && launcher.projectilePrefab != null) projectileRigidbody = launcher.projectilePrefab.rb;
            if (target != null) targetRigidbody = target.GetComponent<Rigidbody>();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (launcher != null && launcher.projectilePrefab != null) projectileRigidbody = launcher.projectilePrefab.rb;
            if (target != null) targetRigidbody = target.GetComponent<Rigidbody>();
        }
#endif

        void Update()
        {
            if (target == null)
            {
                isAimed = false;
                return;
            }

            projectileSpeed = launcher.launchPower / projectileRigidbody.mass;

            if (projectileSpeed <= 0f)
            {
                isAimed = false;
                return;
            }

            Vector3 firePosition = launcher.firePoint.position;
            Vector3 shooterVelocity = launcher.ownerRigidbody.GetPointVelocity( firePosition );
            predictionTime = Mathf.Min( Vector3.Distance( firePosition, target.transform.position ) / projectileSpeed, maxPredictionTime );

            for (int i = 0; i < predictionIterations; i++)
            {
                PredictTargetPosition(predictionTime);
                Vector3 requiredProjectileMovement = predictedTargetPosition - firePosition - shooterVelocity * predictionTime;
                predictionTime = Mathf.Min( requiredProjectileMovement.magnitude / projectileSpeed, maxPredictionTime
                );
            }

            PredictTargetPosition(predictionTime);
            Vector3 requiredDisplacement = predictedTargetPosition - firePosition - shooterVelocity * predictionTime;
            aimDirection = requiredDisplacement.normalized;
            ballisticAimPoint = firePosition + requiredDisplacement;
            Quaternion targetRotation = Quaternion.LookRotation( aimDirection, weaponPivot.up );
            weaponPivot.rotation = Quaternion.RotateTowards( weaponPivot.rotation, targetRotation, rotationSpeed * Time.deltaTime );
            aimError = Vector3.Angle( weaponPivot.forward, aimDirection );
            isAimed = aimError <= fireAngle;
        }

        #endregion

        #region Prediction

        void PredictTargetPosition(float time)
        {
            if (targetRigidbody == null)
            {
                predictedTargetPosition = target.transform.position;
                return;
            }

            Vector3 center = targetRigidbody.worldCenterOfMass + targetRigidbody.linearVelocity * time;
            Vector3 offset = target.transform.position - targetRigidbody.worldCenterOfMass;
            Vector3 angularVelocity = targetRigidbody.angularVelocity;
            if (angularVelocity.sqrMagnitude > 0.0001f) offset = Quaternion.AngleAxis(angularVelocity.magnitude * Mathf.Rad2Deg * time, angularVelocity.normalized) * offset;
            predictedTargetPosition = center + offset;
        }

        #endregion

        #region Public methods

        public override void Fire()
        {
            launcher.Launch();
        }

        public override void SetTarget(GameObject value)
        {
            target = value;
            targetRigidbody = target != null ? target.GetComponentInParent<Rigidbody>() : null;
        }

        #endregion
    }
}