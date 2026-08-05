using System.Collections;
using UnityEngine;
namespace Lucy.Core.Extensions
{
    internal class StaticCoroutine : MonoBehaviour
    {
        private static StaticCoroutine @in;
        public static Coroutine Start(IEnumerator routine)
        {
            if (@in == null)
            {
                @in = new GameObject("[StaticCoroutine]").AddComponent<StaticCoroutine>();
                DontDestroyOnLoad(@in.gameObject);
            }
            return @in.StartCoroutine(routine);
        }

        public static void Stop(Coroutine routine)
        {
            if (@in != null && routine != null) @in.StopCoroutine(routine);
        }

        public static void StopAll()
        {
            @in?.StopAllCoroutines();
        }
    }
}