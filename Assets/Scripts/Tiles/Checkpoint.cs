using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Tile
{
    // TODO Remove respawnObject reference
    public GameObject respawnObject;
    public Item requiredItem;
    public ByteSheep.Events.AdvancedEvent OnPlayerEnterEvent;

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (player.PlayerData.Inventory.ContainItem(requiredItem))
        {
            // Reset respawn position and respawn player
            player.PlayerData.Inventory.RemoveItem(requiredItem);
            player.SetRespawnPosition(1);
            player.SetInitLives();
            player.Respawn(false);
            OnPlayerEnterEvent.Invoke();
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
    }
}
