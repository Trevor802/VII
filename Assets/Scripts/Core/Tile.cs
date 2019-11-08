using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool playerInTemp;
    private bool playerOutTemp;
    protected bool playerInside;
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

    #region Virtual Functions
    protected virtual void OnTickStart() { }
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
    }

    protected virtual void OnPlayerRespawnStart(Player player) { }
    protected virtual void OnPlayerRespawnEnd(Player player)
    {
        playerInTemp = false;
        playerOutTemp = false;
        playerInside = false;
    }

    protected virtual void OnPlayerEnter(Player player) { }
    protected virtual void OnPlayerExit(Player player) { }

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
}
