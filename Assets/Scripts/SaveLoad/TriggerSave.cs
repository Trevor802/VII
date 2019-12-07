using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSave : Tile
{
    private Player player;
    public void SavePlayer(Player player)
    {

        SaveSystem.SavePlayer(player);
    }

    public void LoadPlayer(Player player)
    {
        SavePlayerData data = SaveSystem.LoadPlayer();
        //player.PlayerData.respawnPosition = data.respawnPoint;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            print("save player");
            SaveSystem.SavePlayer(player);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SavePlayerData data = SaveSystem.LoadPlayer();
            player.PlayerData.respawnPosition.x = data.position[0];
            player.PlayerData.respawnPosition.y = data.position[1];
            player.PlayerData.respawnPosition.z = data.position[2];

        }
    }
}
