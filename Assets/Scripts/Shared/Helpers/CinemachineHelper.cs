using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;

namespace Lucy.Core.Helpers
{
    public class CinemachineHelper: MonoBehaviour
    {
        public CinemachineCamera vcam;
        public CinemachineRotationComposer composer;
        public void SetCameraPriority(int priority) => vcam.Priority = priority;
        public void SetComposerOffsetX(float x) { if (composer != null) composer.TargetOffset.x = x; }
        public void SetComposerOffsetY(float y) { if (composer != null) composer.TargetOffset.y = y; }
        public void SetComposerOffsetZ(float z) { if (composer != null) composer.TargetOffset.z = z; }

    }
}