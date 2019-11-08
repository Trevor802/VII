using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Tile
{
    public GameObject respawnObject;

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (player.GetPlayerData().HasKey())
        {
            // Reset respawn position and respawn player
            player.GetPlayerData().respawnPosition = respawnObject.transform.position;
            player.Respawn(false);
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
    }
}
