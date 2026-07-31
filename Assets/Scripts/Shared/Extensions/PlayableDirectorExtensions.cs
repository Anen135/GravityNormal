using UnityEngine;
using UnityEngine.Playables;

namespace Lucy.Shared.Extensions
{
    public static class PlayableDirectorExtensions 
    {
        public static bool IsPlaying(this PlayableDirector playableDirector) => playableDirector.state == PlayState.Playing;
        public static bool IsPaused(this PlayableDirector playableDirector) => playableDirector.state == PlayState.Paused;
    }
}