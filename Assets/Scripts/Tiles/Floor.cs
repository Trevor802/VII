using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VII;
using UnityEditor;

[ExecuteAlways]
public class Floor : Tile
{
    public enum FloorState
    {
        UP = 0,
        DOWN = 1
    }

    public bool declineAfterExit;
    public int stepsBeforeIncline;
    public GameObject model;

    private int m_stepsAfterDecline;
    [SerializeField]
    private FloorState m_floorState = FloorState.UP;
    private int m_lavaFillCounter = 0;
    private bool m_lavaFlows = false;
    private int unreachable_layer = (int)VII.HitLayer.Unreachable;
    private Animator m_animator;
    protected override void Awake()
    {
        base.Awake();
        if (!declineAfterExit && m_floorState == FloorState.DOWN)
        {
            m_lavaFlows = true;
            m_lavaFillCounter = 0;
            this.gameObject.layer = 12;
        }
        #region Presentation Layer
        m_animator = model.GetComponent<Animator>();
        #endregion
    }

    private void OnEnable()
    {
        if (!m_animator) m_animator = model.GetComponent<Animator>();
#if UNITY_EDITOR
        EditorApplication.update += EditorUpdate;
#endif
        UpdateFloor();
    }
#if UNITY_EDITOR
    private void OnDisable()
    {
        if (!m_animator) m_animator = model.GetComponent<Animator>();
        EditorApplication.update -= EditorUpdate;
    }

    private void OnDestroy()
    {
        if (!m_animator) m_animator = model.GetComponent<Animator>();
        EditorApplication.update -= EditorUpdate;
    }

    private void EditorUpdate()
    {
        if (gameObject.activeSelf)
            m_animator.Update(Time.deltaTime);
    }

    private void OnValidate()
    {
        UpdateFloor();
    }
#endif

    private void UpdateFloor()
    {
        #region Presentation Layer
        if (!m_animator) m_animator = model.GetComponent<Animator>();
        if (m_animator.isActiveAndEnabled)
        {
            switch (m_floorState)
            {
                case FloorState.UP:
                    if (m_animator.GetBool("Decline") == true)
                        m_animator.SetBool("Decline", false);
                    break;
                case FloorState.DOWN:
                    if (m_animator.GetBool("Decline") == false)
                        m_animator.SetBool("Decline", true);
                    break;
                default:
                    break;
            }
            gameObject.layer = (!declineAfterExit && m_floorState == FloorState.DOWN) ?
                LayerMask.NameToLayer(VII.HitLayer.Unreachable.ToString()) :
                LayerMask.NameToLayer(VII.HitLayer.Default.ToString());
        }
        #endregion
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
                m_lavaFlows = false;
                UpdateFloor();
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
            m_lavaFillCounter = 1;
            UpdateFloor();
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
