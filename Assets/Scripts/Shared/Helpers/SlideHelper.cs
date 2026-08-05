using System.Collections;
using UnityEngine;

namespace Lucy.Core.Helpers
{
    public class SlideHelper : MonoBehaviour 
    {
        Coroutine moveRoutine;

        bool isForward;
        bool cachedStart;
        Vector3 startPos;

        public void MoveTo(Vector3 direction, float distance, float duration)
        {
            if (!cachedStart)
            {
                startPos = transform.localPosition;
                cachedStart = true;
            }

            Vector3 endPos = isForward ? startPos : startPos + direction.normalized * distance;
            isForward = !isForward;

            if (moveRoutine != null) StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(Animate(transform.localPosition, endPos, duration));
        }

        IEnumerator Animate(Vector3 start, Vector3 end, float duration)
        {
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;

                float t = time / duration;

                t = Mathf.Sin(t * Mathf.PI * 0.5f);
                t = t * t;

                transform.localPosition = Vector3.Lerp(start, end, t);

                yield return null;
            }

            transform.localPosition = end;
        }
    }
}