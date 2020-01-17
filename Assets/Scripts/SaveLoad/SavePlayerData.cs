using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SavePlayerData
{
    public float[] position;
    public int savelives;
    public int saveLevelIndex;
    public int saveMapId;
    public int saveLevelId;
    public int savePPIndex;
    public int saveFogIndex;
    public float saveMusicVolume;
    public float saveSoundVolume;
    public bool saveListInit;
    public bool savePlayedLevel17;
    public List<int> saveLeastLives;

    public SavePlayerData(Player player)
    {
        saveMapId = VII.SceneDataManager.Instance.GetCurrentMapData().GetMapID();
        saveLevelId = VII.SceneDataManager.Instance.GetCurrentLevelData().GetLevelID();
        saveLevelIndex = CameraManager.Instance.level_index;
        savePPIndex = CameraManager.Instance.pp_index;
        saveFogIndex = CameraManager.Instance.fog_index;
        saveMusicVolume = AudioManager.instance.GetMusicVolume();
        saveSoundVolume = AudioManager.instance.GetSoundVolume();
        saveListInit = SteamAchievements.listInit;
        savePlayedLevel17 = player.playedLevel17;
        saveLeastLives = SteamAchievements.leastLives;   
    }
}
