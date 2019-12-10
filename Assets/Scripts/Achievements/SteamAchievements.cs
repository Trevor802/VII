using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievements : MonoBehaviour
{
    public bool achievementUnlocked;

    private void Start()
    {
        //unlockAchievement("achievement_00");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            unlockAchievement("achievement_00");
            print("unlocked");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            lockAchievement("achievement_00");
            print("locked");
        }
        SteamAPI.RunCallbacks();
    }

    public void unlockAchievement(string achievementID)
    {
        getAchievementStatus(achievementID);
        if (!achievementUnlocked)
        {
            SteamUserStats.SetAchievement(achievementID);
            SteamUserStats.StoreStats();
        }
    }

    void lockAchievement(string achievementID)
    {
        getAchievementStatus(achievementID);
        if (achievementUnlocked)
        {
            SteamUserStats.ClearAchievement(achievementID);
            SteamUserStats.StoreStats();
        }
    }

    void getAchievementStatus(string achievementID)
    {
        SteamUserStats.GetAchievement(achievementID, out achievementUnlocked);
    }
}
