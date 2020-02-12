using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Tile
{
    private bool m_close;
    public GameObject model;
    private Animator m_animator;

    protected override void Awake()
    {
        base.Awake();
        m_animator = model.GetComponent<Animator>();
    }
    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (!m_close)
        {
            m_close = true;
            if (player.mapIndex == 2 && player.levelIndex == 0)
            {
                player.DiedInTrapInLevel5 = true;
            }
            if (player.mapIndex == 3 && player.levelIndex == 0)
            {
                player.DiedInTrapInLevel7 = true;
            }
            player.Respawn();
            #region Presentation Layer
            m_animator.SetTrigger("Close");
            AudioManager.instance.PlaySingle(AudioManager.instance.trapDeath);
            #endregion
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
    }
}
