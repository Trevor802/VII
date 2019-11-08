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

    protected override void Awake()
    {
        base.Awake();
        m_floorState = initFloorState;
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
            m_stepsAfterDecline++;
            if (m_stepsAfterDecline > stepsBeforeIncline)
            {
                m_stepsAfterDecline = 0;
                // Incline
                m_floorState = FloorState.UP;
                model.transform.position = transform.position;
            }
        }
    }

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (m_floorState == FloorState.DOWN)
        {
            Debug.Log("player died");
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
}
