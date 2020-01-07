using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SavePlayerData
{
    public float[] position;
    public int savelives;
    public int cameraIndex;
    public int saveMapId;
    public int saveLevelId;
    public int savePPIndex;
    public int savFogIndex;
    public bool saveListInit;
    public bool savePlayedLevel17;
    public List<int> saveLeastLives;
    //public int cameraIndex;
    //public string playerName;

    public SavePlayerData(Player player)
    {
        saveMapId = VII.SceneDataManager.Instance.GetCurrentMapData().GetMapID();
        saveLevelId = VII.SceneDataManager.Instance.GetCurrentLevelData().GetLevelID();
        cameraIndex = CameraManager.Instance.level_index;
        savePPIndex = CameraManager.Instance.pp_index;
        savFogIndex = CameraManager.Instance.fog_index;
        saveListInit = SteamAchievements.listInit;
        savePlayedLevel17 = player.playedLevel17;
        saveLeastLives = SteamAchievements.leastLives;
        /*position[0] = player.PlayerData.respawnPosition.x;
        position[1] = player.PlayerData.respawnPosition.y;
        position[2] = player.PlayerData.respawnPosition.z;
        savelives = player.GetLives();*/
        
        
    }
}
