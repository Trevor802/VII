using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievements : MonoBehaviour
{
    public bool achievementUnlocked;
    public Player player;
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

        //lock all achievements
        if (Input.GetKeyDown(KeyCode.F2))
        {
            lockAchievement("achievement_00");
            lockAchievement("achievement_01");
            lockAchievement("achievement_02");
            lockAchievement("achievement_03");
            print("locked");
        }

        if (player)
        {
            if (player.m_RespawnPosIndex == 0)
            {
                if(player.DiedInLevel0 == true)
                {
                    unlockAchievement("achievement_01");
                }
            }

            if(player.m_RespawnPosIndex == 5)
            {
                if(player.DiedInLevel5 == false && player.DiedInTrapInLevel5 == true)
                {
                    unlockAchievement("achievement_02");
                }
            }

            if(player.m_RespawnPosIndex == 8)
            {
                if(player.FinishLevel7 == true && player.DiedInTrapInLevel7 == false)
                {
                    unlockAchievement("achievement_03");
                }
            }

            if(player.m_RespawnPosIndex == 8)
            {

            }

            if(player.m_RespawnPosIndex == 17)
            {

            }
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
