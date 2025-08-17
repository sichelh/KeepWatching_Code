using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AchievementSaveData
{
    public List<AchievementStatus> Achievements = new List<AchievementStatus>();
}

[System.Serializable]
public class AchievementStatus
{
    public string Id;
    public bool IsAchieved;
}

public class SaveManager : Singleton<SaveManager>
{
    private string savePath;

    protected override void Awake()
    {
        base.Awake();
        savePath = Path.Combine(Application.persistentDataPath, "achievements.json");
    }

    public void SaveAchievements(AchievementSaveData achievements)
    {
        AchievementSaveData saveData = new AchievementSaveData();
        saveData = achievements;
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
    }

    public AchievementSaveData LoadAchievements(AchievementSaveData achievements)
    {
        AchievementSaveData saveData = new AchievementSaveData();
        if (File.Exists(savePath))
        {
            string json = "";
            json = File.ReadAllText(savePath) ?? "";
            saveData = JsonUtility.FromJson<AchievementSaveData>(json);
        }
        else
        {
            foreach (var achievement in AchievementManager.Instance.Achievements)
            {
                AchievementStatus @status = new AchievementStatus();
                @status.Id = achievement.Id;
                @status.IsAchieved = false;
                saveData.Achievements.Add(@status);
            }

            if (saveData.Achievements.Count != AchievementManager.Instance.Achievements.Count)
            {
                for (int i = saveData.Achievements.Count - 1; i < AchievementManager.Instance.Achievements.Count; i++)
                {
                    AchievementStatus @status = new AchievementStatus();
                    @status.Id = AchievementManager.Instance.Achievements[i].Id;
                    @status.IsAchieved = false;
                    saveData.Achievements.Add(@status);
                }
            }
        }

        return saveData;
    }
}