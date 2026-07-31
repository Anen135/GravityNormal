using Lucy.Features.Missions.Runtime;

public class CyclicGameManager : BaseGameManager {

    protected override bool TryGetMission(int index, out BaseMission mission) {
        if (missions == null || missions.Length == 0) {
            mission = null;
            return false;
        }

        int wrappedIndex = index % missions.Length;
        mission = missions[wrappedIndex];
        return true;
    }

    protected override void OnAllMissionsCompleted() {
        // Никогда не вызывается, но оставляем на будущее
    }
}
