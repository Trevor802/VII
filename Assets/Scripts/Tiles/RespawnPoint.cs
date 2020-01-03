using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : Tile
{
    public ByteSheep.Events.AdvancedEvent OnPlayerRespawnEndEvent;
    public int bestLifeCost = VII.GameData.PLAYER_DEFAULT_LIVES;

    protected override void OnPlayerRespawnEnd(Player player)
    {
        base.OnPlayerRespawnEnd(player);
        if (playerInside)
            OnPlayerRespawnEndEvent.Invoke();
    }
}
