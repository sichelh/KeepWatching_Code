using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : SceneOnlySingleton<AchievementManager>
{
    public List<AchievementData> Achievements = new List<AchievementData>();
    public AchievementSaveData AchievementSaveData;
    public event Action<string> OnAchievementUnlocked;

    protected override void Awake()
    {
        base.Awake();
        LoadAchievements();
    }

    void Start()
    {
        CheckAchievementById("t0", 100);
    }

    public void CheckAchievementById(string achievementId, int currentValue)
    {
        var achievement = Achievements.Find(a => a.Id == achievementId);
        if (currentValue >= achievement.TargetValue)
        {
            var achievementSave = AchievementSaveData.Achievements.Find(a => a.Id == achievementId);
            UnlockAchievement(achievementSave);
        }
    }

    private void UnlockAchievement(AchievementStatus achievement)
    {
        achievement.IsAchieved = true;
        OnAchievementUnlocked?.Invoke(achievement.Id);
        SaveManager.Instance.SaveAchievements(AchievementSaveData);
    }

    private void LoadAchievements()
    {
        AchievementSaveData = SaveManager.Instance.LoadAchievements(AchievementSaveData);
    }
}