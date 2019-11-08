using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Tile
{
    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
        Debug.Log(player.name + " leaves " + name);
    }
}
