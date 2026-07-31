using UnityEngine;
using System;
using System.Collections;
using Lucy.Shared.Extensions;
namespace Lucy.Shared.Helpers
{
    public static class Magnet
    {
        /// <summary>
        /// Запускает процесс приfтягивания объекта к точке.
        /// </summary>
        /// <param name="obj">Объект с Rigidbody</param>
        /// <param name="target">Куда тянуть</param>
        /// <param name="force">Сила/Ускорение</param>
        /// <param name="stopDistance">Радиус захвата</param>
        public static Coroutine ToPoint(GameObject obj, Transform target, float force = 20f, float stopDistance = 0.2f, Action onComplete = null)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning($"У объекта {obj.name} нет Rigidbody!");
                return null;
            }

            return StaticCoroutine.Start(PullRoutine(rb, target, force, stopDistance, onComplete));
        }

        public static Coroutine ToPoint(GameObject obj, Vector3 targetPosition, Transform parent = null, float force = 20f, float stopDistance = 0.2f, Action onComplete = null)
        {
            GameObject tempTargetObj = new GameObject("[Temp_Pull_Target]");
            Transform tempTarget = tempTargetObj.transform;

            if (parent != null)
            {
                tempTarget.SetParent(parent);
                tempTarget.localPosition = targetPosition;
            }
            else
            {
                tempTarget.position = targetPosition;
            }

            return ToPoint(obj, tempTarget, force, stopDistance, () =>
            {
                onComplete?.Invoke();
                if (tempTargetObj != null) UnityEngine.Object.Destroy(tempTargetObj);
            });
        }

        static IEnumerator PullRoutine(Rigidbody rb, Transform target, float force, float stopDistance, Action onComplete)
        {
            rb.isKinematic = false;
            rb.useGravity = false;

            while (rb != null && target != null)
            {
                float distance = Vector3.Distance(rb.position, target.position);

                if (distance <= stopDistance)
                {
                    rb.isKinematic = true;
                    rb.transform.position = target.position;
                    rb.linearVelocity = Vector3.zero;
                    
                    onComplete?.Invoke();
                    yield break; 
                }

                Vector3 direction = (target.position - rb.position).normalized;
                rb.AddForce(direction * force, ForceMode.Acceleration);

                yield return new WaitForFixedUpdate();
            }
        }
    }
}
