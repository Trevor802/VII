using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavePlayerData
{
    public Vector3 respawnPoint;
    //public string playerName;
    
    public SavePlayerData(Player player)
    {
        respawnPoint = player.PlayerData.respawnPosition;
    }
}
