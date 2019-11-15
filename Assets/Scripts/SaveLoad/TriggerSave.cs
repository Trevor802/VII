using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSave : Tile
{
    
    public void SavePlayer(Player player)
    {

        SaveSystem.SavePlayer(player);
    }

    public void LoadPlayer(Player player)
    {
        SavePlayerData data = SaveSystem.LoadPlayer();
        player.PlayerData.respawnPosition = data.respawnPoint;
    }
    private void Update()
    {
        
    }
}
