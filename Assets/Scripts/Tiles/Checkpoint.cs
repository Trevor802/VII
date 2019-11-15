using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Tile
{
    public GameObject respawnObject;
    public Item requiredItem;

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (player.PlayerData.Inventory.ContainItem(requiredItem))
        {
            // Reset respawn position and respawn player
            player.PlayerData.Inventory.RemoveItem(requiredItem);
            var value = player.PlayerData.Inventory.items;
            player.PlayerData.respawnPosition = respawnObject.transform.position;
            player.Respawn(false);
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
    }
}
