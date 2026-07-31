using System;
using Lucy.Features.Missions.Runtime;
using UnityEngine;

public class BaseGameManager : MonoBehaviour {
    [Header("Missions")]
    public BaseMission[] missions;

    [Header("Runtime")]
    public BaseMission currentMission;
    public int currentMissionIndex = -1;

    public string CurrentMissionName => currentMission != null ? currentMission.missionName : string.Empty;
    public BaseMission CurrentMission => currentMission;

    protected virtual void Start() => StartGame();

    #region Game Flow

    public virtual void StartGame() {
        currentMissionIndex = -1;
        StartNextMission();
    }

    public virtual void PauseGame() =>
        Debug.Log("=== Game paused ===");

    public virtual void MissionFailed() {
        Debug.Log($"[GameManager] Mission failed: {CurrentMissionName}");
        OnMissionFailed();
    }

    #endregion

    #region Missions

    protected void StartNextMission() {
        currentMissionIndex++;

        if (!TryGetMission(currentMissionIndex, out currentMission)) {
            OnAllMissionsCompleted();
            return;
        }

        RegisterMissionEvents(currentMission);
        Debug.Log($"=== Start mission {CurrentMissionName} ===");
        currentMission.StartMission();
    }

    public virtual void RestartMission() {
        if (currentMission == null) return;

        UnregisterMissionEvents(currentMission);
        currentMission.StartMission();
    }

    protected virtual bool TryGetMission(int index, out BaseMission mission) {
        if (missions == null || missions.Length == 0 || index >= missions.Length) {
            mission = null;
            return false;
        }

        mission = missions[index];
        return true;
    }

    #endregion

    #region Mission Events

    protected void RegisterMissionEvents(BaseMission mission) {
        mission.OnMissionCompleted += HandleMissionCompleted;
        mission.OnMissionFailed += HandleMissionFailed;
    }

    protected void UnregisterMissionEvents(BaseMission mission) {
        mission.OnMissionCompleted -= HandleMissionCompleted;
        mission.OnMissionFailed -= HandleMissionFailed;
    }

    protected virtual void HandleMissionCompleted(BaseMission mission) {
        Debug.Log($"[GameManager] Completed mission: {mission.missionName}");
        UnregisterMissionEvents(mission);
        StartNextMission();
    }

    protected virtual void HandleMissionFailed(BaseMission mission) {
        Debug.Log($"[GameManager] Failed mission: {mission.missionName}");
        UnregisterMissionEvents(mission);
        MissionFailed();
    }

    #endregion

    #region Hooks (for subclasses)

    protected virtual void OnAllMissionsCompleted()
    {
        Debug.Log("=== All missions completed ===");
    }
    protected virtual void OnMissionFailed() { }

    #endregion
}
