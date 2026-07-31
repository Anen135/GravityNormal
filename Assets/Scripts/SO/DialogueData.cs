using UnityEngine;

namespace Lucy.SO
{
    [System.Serializable]
    public class SubtitleEntry
    {
        public string speaker;
        public GameObject speakerObject; // Проверь, что это именно GameObject и он public
        [TextArea(2, 4)] public string text;
        public string audioKey;
        public float delayAfter = 1.5f;
        public bool changeQuest;
        public string newQuestText;
    }

    [CreateAssetMenu(fileName = "DialogueData", menuName = "IMD/Dialogue/Data")]
    public class DialogueData : ScriptableObject
    {
        public SubtitleEntry[] subtitles;
    }
}