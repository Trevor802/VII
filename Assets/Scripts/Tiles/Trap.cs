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
            player.Respawn();

            #region Presentation Layer
            m_animator.SetTrigger("Close");
            #endregion
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
    }
}
