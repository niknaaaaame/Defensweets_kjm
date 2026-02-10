using UnityEngine;

public static class ProgressionSave
{
    private const string ClearedStagePrefix = "progress.clearedStage.";
    private const string LegacyClearedStagePrefix = "Stage_";
    private const string TowerUnlockPrefix = "progress.unlockedTower.";

    public static void MarkStageCleared(int stageNumber)
    {
        if (stageNumber <= 0)
            return;

        PlayerPrefs.SetInt(ClearedStagePrefix + stageNumber, 1);
        PlayerPrefs.SetInt(GetLegacyStageClearKey(stageNumber), 1);
        PlayerPrefs.Save();
    }

    public static bool HasClearedStage(int stageNumber)
    {
        if (stageNumber <= 0)
            return false;

        return PlayerPrefs.GetInt(ClearedStagePrefix + stageNumber, 0) == 1
               || PlayerPrefs.GetInt(GetLegacyStageClearKey(stageNumber), 0) == 1;
    }

    public static string GetLegacyStageClearKey(int stageNumber)
    {
        return $"{LegacyClearedStagePrefix}{stageNumber}_Cleared";
    }

    public static void UnlockTower(string towerKey)
    {
        if (string.IsNullOrWhiteSpace(towerKey))
            return;

        PlayerPrefs.SetInt(TowerUnlockPrefix + towerKey, 1);
        PlayerPrefs.Save();
        Debug.Log($"[ProgressionSave] Tower {towerKey} unlocked!");
    }

    public static bool IsTowerUnlocked(string towerKey)
    {
        if (string.IsNullOrWhiteSpace(towerKey))
            return false;

        return PlayerPrefs.GetInt(TowerUnlockPrefix + towerKey, 0) == 1;
    }
}