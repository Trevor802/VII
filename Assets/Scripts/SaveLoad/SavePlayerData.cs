using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavePlayerData
{
    public float[] position;
    //public string playerName;
    
    public SavePlayerData(Player player)
    {
        position = new float[3];
        position[0] = player.PlayerData.respawnPosition.x;
        position[1] = player.PlayerData.respawnPosition.y;
        position[2] = player.PlayerData.respawnPosition.z;
    }
}
