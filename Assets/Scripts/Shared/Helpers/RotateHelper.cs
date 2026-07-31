using System.Collections;
using UnityEngine;

namespace Lucy.Shared.Helpers
{
    public class RotateHelper : MonoBehaviour
    {
        bool reverse = false;
        Coroutine rotateRoutine;

        bool cachedStart;
        Vector3 startRot;

        public void RotateTo(Vector3 axis, float angle, float duration)
        {
            if (!cachedStart)
            {
                startRot = transform.localEulerAngles;
                cachedStart = true;
            }

            Vector3 targetRot = reverse ? startRot : startRot + axis * angle;

            reverse = !reverse;

            if (rotateRoutine != null) StopCoroutine(rotateRoutine);
            rotateRoutine = StartCoroutine(Animate(transform.localEulerAngles, targetRot, duration));
        }

        IEnumerator Animate(Vector3 start, Vector3 end, float duration)
        {
            float time = 0f;

            Quaternion startQ = Quaternion.Euler(start);
            Quaternion endQ = Quaternion.Euler(end);

            while (time < duration)
            {
                time += Time.deltaTime;

                float t = time / duration;

                t = Mathf.Sin(t * Mathf.PI * 0.5f);
                t = t * t;

                transform.localRotation = Quaternion.Lerp(startQ, endQ, t);

                yield return null;
            }

            transform.localRotation = endQ;
        }
    }
}