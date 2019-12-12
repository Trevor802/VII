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
    private int m_lavaFillCounter = 0;
    private bool m_lavaFlows = false;
    protected override void Awake()
    {
        base.Awake();
        m_floorState = initFloorState;
        if (!declineAfterExit && m_floorState == FloorState.DOWN)
        {
            m_lavaFlows = true;
            m_lavaFillCounter = 0;
            this.gameObject.layer = 8;
        }
            
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

    protected override void OnTickStart()
    {
        base.OnTickStart();
        if(m_lavaFillCounter == 1)
        {
            //Debug.Log("Lava can flow in");
            m_lavaFlows = true;
            m_lavaFillCounter--;
        }
    }
    protected override void OnTickEnd()
    {
        base.OnTickEnd();
        if (declineAfterExit && m_floorState == FloorState.DOWN) 
        {
            //step counter
            m_stepsAfterDecline++;
            if (m_stepsAfterDecline > stepsBeforeIncline)
            {
                //Debug.Log(m_stepsAfterDecline);
                m_stepsAfterDecline = 0;
                // Incline
                m_floorState = FloorState.UP;
                model.transform.position = transform.position;
                m_lavaFlows = false;
                //this.gameObject.layer = 0;
            }
        }
    }

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if(declineAfterExit)
        {
            Debug.Log("Player steps on floor");
        }
        
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
            Debug.Log("Player exits floor");
            m_floorState = FloorState.DOWN;
            model.transform.position = transform.position -
                new Vector3(0, VII.GameData.STEP_SIZE, 0);
            m_lavaFillCounter = 1;
            //set this floor to block layer
            //this.gameObject.layer = 8;
        }
    }

    public FloorState GetFloorState()
    {
        return m_floorState;
    }

    public bool GetLavaFlowState()
    {
        return m_lavaFlows;
    }
}
