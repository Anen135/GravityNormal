using UnityEngine;

namespace Lucy.SO
{
    [CreateAssetMenu(menuName = "Lucy/Audio/Settings", fileName = "AudioSettings")]
    public class AudioSettings : ScriptableObject
    {
        [Header("General")]
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
    }
}