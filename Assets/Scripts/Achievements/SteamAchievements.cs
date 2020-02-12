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
        if (saveForAchievement !=  null)
        {
            listInit = saveForAchievement.saveListInit;
            player.playedLevel17 = saveForAchievement.savePlayedLevel17;
            leastLives = saveForAchievement.saveLeastLives;
        }
        if (!listInit)
        {
            leastLives = new List<int> 
            { 
                1, 1, 1, //Map0
                2, 2, 3, //Map1 +3
                2, 2, //Map2 +6
                5, 3, //Map3 +8
                1, 2, 1, //Map4 +10
                1, //Map5 +13
                1, 1, //Map6 +14
                2, //Map7 +16
                3, 2, //Map8 +17
                1, //Map9 +19
                1, //Map10 +20
                2, 1, //Map11 +21
                3, //Map12 +23
                3 //Map13 +24
            }; 

            listInit = true;
        }
        //unlockAchievement("achievement_00");

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            unlockAchievement("achievement_00");
            print("unlocked");
        }
        /*
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
        */
        //print(player.DiedInLevel5 + ", " + player.DiedInTrapInLevel5);
        //print(player.mapIndex + ", " + player.levelIndex);
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
                    if (player.livesLeft == leastLives[player.levelIndex + 3])
                    {
                        leastLives[player.levelIndex + 3] = 0;
                    }
                }
                else if (player.mapIndex == 2)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 6])
                    {
                        leastLives[player.levelIndex + 6] = 0;
                    }
                }
                else if (player.mapIndex == 3)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 8])
                    {
                        leastLives[player.levelIndex + 8] = 0;
                    }
                }
                else if (player.mapIndex == 4)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 10])
                    {
                        leastLives[player.levelIndex + 10] = 0;
                    }
                }
                else if (player.mapIndex == 5)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 13])
                    {
                        leastLives[player.levelIndex + 13] = 0;
                    }
                }
                else if (player.mapIndex == 6)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 14])
                    {
                        leastLives[player.levelIndex + 14] = 0;
                    }
                }
                else if (player.mapIndex == 7)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 16])
                    {
                        leastLives[player.levelIndex + 16] = 0;
                    }
                }
                else if (player.mapIndex == 8)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 17])
                    {
                        leastLives[player.levelIndex + 17] = 0;
                    }
                }
                else if (player.mapIndex == 9)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 19])
                    {
                        leastLives[player.levelIndex + 19] = 0;
                    }
                }
                else if (player.mapIndex == 10)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 20])
                    {
                        leastLives[player.levelIndex + 20] = 0;
                    }
                }
                else if (player.mapIndex == 11)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 21])
                    {
                        leastLives[player.levelIndex + 21] = 0;
                    }
                }
                else if (player.mapIndex == 12)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 23])
                    {
                        leastLives[player.levelIndex + 23] = 0;
                    }
                }
                else if (player.mapIndex == 13)
                {
                    if (player.livesLeft == leastLives[player.levelIndex + 24])
                    {
                        leastLives[player.levelIndex + 24] = 0;
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

                if (j == 25) //if completes all 26 levels with least lives spent
                {
                    unlockAchievement("achievement_09");
                }
            }

            if (!player.playedLevel17 && player.mapIndex == 9 && player.levelIndex == 0) //TODO: needs to be modified
            {
                if (leastLives[18] == 0)
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
