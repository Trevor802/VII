using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievements : MonoBehaviour
{
    public bool achievementUnlocked;
    public Player player;
    public Level8TriggerBoard Level8TriggerBoard;
    public static List<int> leastLives; //TODO: This needs to be saved
    public static bool listInit; //TODO: This also needs to be saved
    private void Start()
    {
        SavePlayerData saveForAchievement = SaveSystem.LoadPlayer();
        listInit = saveForAchievement.saveListInit;
        player.playedLevel17 = saveForAchievement.savePlayedLevel17;
        leastLives = saveForAchievement.saveLeastLives;
        //unlockAchievement("achievement_00");
        if (!listInit)
        {
            leastLives = new List<int> {1, 2, 1, 1, 1, 3, 1, 2, 5, 4, //Dungeon Levels
                                    1, 2, 1, 1, 1, 1, 2, 3, 2, //Ice Levels
                                    2, 1, 1, 3, 1, 3, 3}; //Lava Levels

            listInit = true;
        }
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
            lockAchievement("achievement_04");
            print("locked");
        }

        if (player)
        {
            if (player.mapIndex == 0 && player.levelIndex == 0)
            {
                if (player.DiedInLevel0 == true)
                {
                    unlockAchievement("achievement_01");
                }
            }

            if (player.mapIndex == 2 && player.levelIndex == 0)
            {
                if (player.DiedInLevel5 == false && player.DiedInTrapInLevel5 == true)
                {
                    unlockAchievement("achievement_02");
                }
            }

            if (player.mapIndex == 3 && player.levelIndex == 1)
            {
                if (player.FinishLevel7 == true && player.DiedInTrapInLevel7 == false)
                {
                    unlockAchievement("achievement_03");
                }
            }

            if (player.mapIndex == 3 && player.levelIndex == 1)
            {
                if (Level8TriggerBoard)
                {
                    if (player.HasKeyInLevel8 == true && Level8TriggerBoard.TriggerBoardDown == false)
                    {
                        unlockAchievement("achievement_04");
                    }
                }
            }

            if (player.completeDungeon == true)
            {
                unlockAchievement("achievement_05");
            }


            if (player.completeIce == true)
            {
                unlockAchievement("achievement_06");
            }

            if (player.completeLava == true)
            {
                unlockAchievement("achievement_07");
            }

            if (player.summonGreatOne == true)
            {
                unlockAchievement("achievement_08");
            }

            if (player.checkLeastLives == true)
            {
                player.checkLeastLives = false;

                if (player.mapIndex == 0)
                {
                    if (player.livesLeft == leastLives[player.levelIndex])
                    {
                        leastLives[player.levelIndex] = 0;
                    }
                }
                else if (player.mapIndex == 1)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 9])
                    {
                        leastLives[player.levelIndex + 9] = 0;
                    }
                }
                else if (player.mapIndex == 2)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 17])
                    {
                        leastLives[player.levelIndex + 17] = 0;
                    }
                }

                int i = 0;
                int j = 0;
                while (i < leastLives.Count)
                {
                    if (leastLives[i] == 0)
                    {
                        j++;
                    }
                    i++;
                }

                if (j == 26) //if completes all 26 levels with least lives spent
                {
                    unlockAchievement("achievement_09");
                }
            }

            if (!player.playedLevel17 && player.mapIndex == 9 && player.levelIndex == 0) //TODO: needs to be modified
            {
                if (leastLives[4 + 17] == 0)
                {
                    unlockAchievement("achievement_10");
                }
                player.playedLevel17 = true;
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
