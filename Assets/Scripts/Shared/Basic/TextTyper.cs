using System.Collections;
using TMPro;
using UnityEngine;

namespace Lucy.Shared.Basic
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextTyper : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI textMesh;

        [Header("Settings")]
        [Tooltip("Задержка между появлением символов")]
        [SerializeField] private float charDelay = 0.03f;

        private Coroutine typingCoroutine;
        private WaitForSeconds cachedDelay;
        private string fullText = string.Empty;
        
        public bool IsTyping { get; private set; }

        private void Awake()
        {
            if (textMesh == null) textMesh = GetComponent<TextMeshProUGUI>();
            cachedDelay = new WaitForSeconds(charDelay);
        }

        public void StartTyping(string text)
        {
            if (textMesh == null)
            {
                Debug.LogWarning($"[{nameof(TextTyper)}] TextMeshPro reference is missing!", this);
                return;
            }

            StopTyping();
            typingCoroutine = StartCoroutine(TypeText(text));
        }

        public void StopTyping()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            IsTyping = false;
        }

        public void Clear()
        {
            StopTyping();
            if (textMesh != null) textMesh.text = string.Empty;
        }

        public void Skip()
        {
            if (!IsTyping) return;

            StopTyping();
            textMesh.maxVisibleCharacters = fullText.Length;
        }

        private IEnumerator TypeText(string text)
        {
            fullText = text;
            IsTyping = true;

            textMesh.text = fullText;
            textMesh.maxVisibleCharacters = 0;

            int totalCharacters = fullText.Length;

            for (int i = 0; i <= totalCharacters; i++)
            {
                textMesh.maxVisibleCharacters = i;
                yield return cachedDelay;
            }

            IsTyping = false;
            typingCoroutine = null;
        }
    }
}