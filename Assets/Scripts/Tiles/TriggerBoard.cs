using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class TriggerBoard : Tile
{
    public ByteSheep.Events.AdvancedEvent OnPlayerEnterEvent;
    public ByteSheep.Events.AdvancedEvent OnPlayerExitEvent;

    public GameObject model;

    public Level8TriggerBoard Level8TriggerBoard;

    private bool m_PlayerOn;
    private bool m_TombstoneOn;
    private Animator m_animator;

    protected override void Awake()
    {
        base.Awake();
        m_animator = model.GetComponent<Animator>();
    }

    protected override void OnTickEnd()
    {
        if (playerOutTemp)
        {
            playerInside = false;
            playerOutTemp = false;

            if (Level8TriggerBoard && !m_TombstoneOn)
            {
                Level8TriggerBoard.TriggerBoardDown = false;
            }
        }
        if (playerInTemp)
        {
            playerInside = true;
            playerInTemp = false;

            if (Level8TriggerBoard && !m_TombstoneOn)
            {
                Level8TriggerBoard.TriggerBoardDown = true;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        collidedPlayer = other.GetComponentInParent<Player>();
        if (collidedPlayer && !playerInside)
        {
            playerInTemp = true;
            playerOutTemp = false;
            OnPlayerEnter(collidedPlayer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidedPlayer && collidedPlayer == other.GetComponentInParent<Player>())
        {
            playerOutTemp = true;
            playerInTemp = false;
            OnPlayerExit(collidedPlayer);
        }

    }

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (m_TombstoneOn) return;
        m_PlayerOn = true;
        #region Presentation Layer
        AudioManager.instance.PlaySingle(AudioManager.instance.triggerBoard);
        #endregion
        m_animator.SetBool("Press", true);
        OnPlayerEnterEvent.Invoke();
    }

    protected override void OnPlayerRespawnStart(Player player)
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
        if(player.mapIndex == 3 && player.levelIndex == 0)
        {
            player.makeSentence.EnableLevel7_Sentence1();
        }
        base.OnPlayerExit(player);
        if (m_TombstoneOn) return;
        m_PlayerOn = false;
        m_animator.SetBool("Press", false);
        OnPlayerExitEvent.Invoke();
    }
}
