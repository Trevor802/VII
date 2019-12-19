using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tile : MonoBehaviour
{
    protected bool playerInTemp;
    protected bool playerOutTemp;
    // TODO Set true for the respawn tile automatically
    public bool playerInside;
    protected bool receiveTick;
    protected Player collidedPlayer;

    #region Event System
    // Child's Awake function needs to execute base.Awake() function to
    // subscribe self to the Event System
    protected virtual void Awake()
    {
        VII.VIIEvents.TickStart.AddListener(OnTickStart);
        VII.VIIEvents.TickEnd.AddListener(OnTickEnd);
        VII.VIIEvents.PlayerRespawnStart.AddListener(OnPlayerRespawnStart);
        VII.VIIEvents.PlayerRespawnEnd.AddListener(OnPlayerRespawnEnd);
    }
    #endregion

    public void SetReceiveTick(bool i_bReceiveTick) { receiveTick = i_bReceiveTick; }

    #region Virtual Functions
    protected virtual void OnTickStart()
    {
        
    }
    protected virtual void OnTickEnd()
    {
        if (playerOutTemp)
        {
            playerInside = false;
            OnPlayerExit(collidedPlayer);
            playerOutTemp = false;
        }
        if (playerInTemp)
        {
            playerInside = true;
            OnPlayerEnter(collidedPlayer);
            playerInTemp = false;
        }
        if (playerInside)
            Player.Instance.tilePlayerInside = this;
    }

    protected virtual void OnPlayerRespawnStart(Player player)
    {
        playerInTemp = false;
        playerOutTemp = false;
        playerInside = false;
    }
    protected virtual void OnPlayerRespawnEnd(Player player)
    {
        playerInTemp = false;
        playerOutTemp = false;
    }

    protected virtual void OnPlayerEnter(Player player)
    {
        
    }
    protected virtual void OnPlayerExit(Player player)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        collidedPlayer = other.GetComponentInParent<Player>();
        if (collidedPlayer && !playerInside)
        {
            playerInTemp = true;
            playerOutTemp = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidedPlayer && collidedPlayer == other.GetComponentInParent<Player>())
        {
            playerOutTemp = true;
            playerInTemp = false;
        }
    }
    #endregion Virtual Functions

    #region setters/getters
    public bool GetPlayerInside() { return playerInside; }
    #endregion
}
