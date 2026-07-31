using UnityEngine;

namespace Lucy.Shared.Helpers
{
    public enum TimeMode
    {
        Normal,
        Unscaled,
        Frozen 
    }

    public class GameTime
    {
        public TimeMode Mode = TimeMode.Normal;
        public float DeltaTime
        {
            get
            {
                switch (Mode)
                {
                    case TimeMode.Normal:
                        return Time.deltaTime;
                    case TimeMode.Unscaled:
                        return Time.unscaledDeltaTime;
                    case TimeMode.Frozen:
                        return 0f;
                    default:
                        return Time.deltaTime;
                }
            }
        }
    }
}