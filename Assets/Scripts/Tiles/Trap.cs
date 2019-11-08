using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Tile
{
    private bool m_open = true;
    public GameObject model;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (m_open)
        {
            m_open = false;
            player.Respawn();
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
    }
}
