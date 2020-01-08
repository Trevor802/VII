using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : Tile
{
    public ByteSheep.Events.AdvancedEvent OnPlayerRespawnEndEvent;
    public int bestLifeCost = VII.GameData.PLAYER_DEFAULT_LIVES;

    private bool invokeEvents = false;

    protected override void OnPlayerRespawnEnd(Player player)
    {
        base.OnPlayerRespawnEnd(player);
        if (playerInside && !invokeEvents)
        {
            OnPlayerRespawnEndEvent.Invoke();
            invokeEvents = true;
        }
        //text stuff
        if (player.mapIndex == 0 && player.levelIndex == 1 && player.diedInLevel1 == true)
        {
            player.makeSentence.EnableLevel1_Sentence2();
        }
    }
}
