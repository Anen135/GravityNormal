using UnityEngine;

namespace Lucy.Features.Highlightable
{
    public class Highlightable : MonoBehaviour
    {
        public Renderer rend;
        public Color highlightColor = Color.yellow;
        public float intensity = 2f;

        private Color originalColor;

        void Start()
        {
            if (rend == null)
                rend = GetComponentInChildren<Renderer>(); 

            if (rend == null)
            {
                Debug.LogWarning("Renderer не найден на объекте " + gameObject.name);
                return;
            }

            originalColor = rend.material.color;
        }

        public void HighlightOn()
        {
            if (rend != null)
                rend.material.color = originalColor * intensity;
        }

        public void HighlightOff()
        {
            if (rend != null)
                rend.material.color = originalColor;
        }
    }
}
