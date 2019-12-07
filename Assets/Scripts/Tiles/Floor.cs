using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : Tile
{
    public enum FloorState
    {
        UP = 0,
        DOWN = 1
    }

    public bool declineAfterExit;
    public int stepsBeforeIncline;
    public FloorState initFloorState;
    public GameObject model;

    private int m_stepsAfterDecline;
    private FloorState m_floorState;
    private bool m_lavaFillsIn = false; 
    private int m_lavaFillCounter = 1;

    protected override void Awake()
    {
        base.Awake();
        m_floorState = initFloorState;
        if (!declineAfterExit)
            m_lavaFillsIn = true;
    }

    private void OnValidate()
    {
        switch (initFloorState)
        {
            case FloorState.UP:
                if (model)
                    model.transform.position = transform.position;
                break;
            case FloorState.DOWN:
                if (model)
                    model.transform.position = transform.position -
                        new Vector3(0, VII.GameData.STEP_SIZE, 0);
                break;
            default:
                break;
        }
    }

    protected override void OnTickEnd()
    {
        base.OnTickEnd();
        if (declineAfterExit && m_floorState == FloorState.DOWN) 
        {
            //Debug.Log("Change state!");
            if (m_lavaFillCounter == 0)
            {
                m_lavaFillsIn = true;
                m_lavaFillCounter = 1;
            }
            else if(m_lavaFillCounter - 1 >= 0 && !m_lavaFillsIn)
                m_lavaFillCounter--;

            m_stepsAfterDecline++;
            if (m_stepsAfterDecline > stepsBeforeIncline)
            {
                //Debug.Log(m_stepsAfterDecline);
                m_stepsAfterDecline = 0;
                // Incline
                m_floorState = FloorState.UP;
                model.transform.position = transform.position;
                m_lavaFillsIn = false;
            }
        }
    }

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (m_floorState == FloorState.DOWN)
        {
            player.Respawn();
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
        if (declineAfterExit && m_floorState == FloorState.UP)
        {
            // Decline
            m_floorState = FloorState.DOWN;
            model.transform.position = transform.position -
                new Vector3(0, VII.GameData.STEP_SIZE, 0);
        }
    }

    public bool GetFloorState()
    {
        return m_lavaFillsIn;
    }

}
