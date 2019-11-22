using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TriggerBoard : Tile
{
    public ByteSheep.Events.QuickEvent OnPlayerEnterEvent;
    public ByteSheep.Events.QuickEvent OnPlayerExitEvent;

    private bool m_PlayerOn;
    private bool m_TombstoneOn;

    protected override void OnTickEnd()
    {
        if (playerOutTemp)
        {
            playerInside = false;
            playerOutTemp = false;
        }
        if (playerInTemp)
        {
            playerInside = true;
            playerInTemp = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        collidedPlayer = other.GetComponentInParent<Player>();
        if (collidedPlayer && !playerInside)
        {
            OnPlayerEnter(collidedPlayer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidedPlayer && collidedPlayer == other.GetComponentInParent<Player>())
        {
            OnPlayerExit(collidedPlayer);
        }
    }

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (m_TombstoneOn) return;
        m_PlayerOn = true;
        OnPlayerEnterEvent.Invoke();
    }

    protected override void OnPlayerRespawnEnd(Player player)
    {
        base.OnPlayerRespawnStart(player);
        if (m_TombstoneOn) return;
        if (m_PlayerOn)
        {
            m_TombstoneOn = true;
            m_PlayerOn = false;
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
        if (m_TombstoneOn) return;
        m_PlayerOn = false;
        OnPlayerExitEvent.Invoke();
    }
}
