﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace VII
{

    [System.Flags]
    public enum HitLayer
    {
        Everything = -1,
        Nothing = 0,
        Default = 1 << 0,
        TransparentFX = 1 << 1,
        IgnoreRaycast = 1 << 2,
        Water = 1 << 4,
        UI = 1 << 5,

        Block = 1 << 8,
        Ice = 1 << 9,
        Player = 1 << 10,
        Interactable = 1 << 11,
        Unreachable = 1 << 12
    }
}

public class Player : MonoBehaviour
{
    #region Singleton
    public static Player Instance = null;

    private void Start()
    {
        //UpdateStepUI();
        //UI Initialization 
        UIManager.UIInstance.InitUI();
        UIManager.UIInstance.UpdateUI();

        m_playerData.respawnPositionIndex = UIManager.UIInstance.startRespawnIndex;
        restartRespawnPositionIndex = m_playerData.respawnPositionIndex;
        initLives = UIManager.UIInstance.startLives;
        Pools = GameObject.Find("Pools").GetComponent<ObjectPooler>();
        transform.position = RespawnTargetGameObjects[m_playerData.respawnPositionIndex].transform.position +
            VII.GameData.PLAYER_RESPAWN_POSITION_OFFSET;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Initialization
            m_playerData = new VII.PlayerData(initLives, initSteps);
            m_inverseMoveTime = 1 / moveTime;
            //Binding Input
            playerInput = new InputActions();
            playerInput.Player.Move.performed += ctx => PerformMove();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }    
    }
    #endregion

    #region PlayerData
    [Header("Audio Clips")]
    public AudioClip footStep;
    public AudioClip death;
    public AudioClip respawn;
    [Header("Configuration")]
    public float moveTime = 0.5f;
    public int initLives = 7;
    public int initSteps = 7;
    [Header("Game Objects")]
    public GameObject GroundDetector;
    public GameObject BodyDetector;
    public GameObject InteractableSpawnPoint;
    public Collider InteractiveCollider;
    public List<GameObject> RespawnTargetGameObjects; 
    [Header("Data for Achievements")]
    //level0
    public bool DiedInLevel0;
    //level5
    public bool DiedInLevel5;
    public bool DiedInTrapInLevel5;
    //level7
    public bool FinishLevel7;
    public bool DiedInTrapInLevel7;
    //level8
    public bool HasKeyInLevel8;
    #endregion PlayerData

    private float m_inverseMoveTime;
    private const float m_maxCastDistance = 10f;
    private Vector3 m_destination;
    private VII.PlayerData m_playerData;
    private Vector3 moveDir;
    private Vector3 currentGridPos;
    private Vector3 nextGridPos;
    private InputActions playerInput;
    private ObjectPooler Pools;
    private int restartRespawnPositionIndex;

    public bool Move(Vector3 i_dir, bool i_costStep = true, bool i_smoothMove = true)
    {
        // Ground detection
        bool groundHit = Physics.Raycast(GroundDetector.transform.position, i_dir, VII.GameData.STEP_SIZE);
        if (!groundHit)
        {
            return false;
        }

        RaycastHit bodyHit;
        bool bodyHitResult;
        bodyHitResult = Physics.Raycast(BodyDetector.transform.position,
            i_dir, out bodyHit, m_maxCastDistance, (int)VII.HitLayer.Block);
        // Player can't move to that direction even 1 grid
        if (bodyHitResult &&
            Vector3.Distance(BodyDetector.transform.position, bodyHit.transform.position)
            < VII.GameData.STEP_SIZE)
        {
            return false;
        }

        RaycastHit unreachableTileHit;
        bool unreachableTileHitResult;
        unreachableTileHitResult = Physics.Raycast(GroundDetector.transform.position,
            i_dir, out unreachableTileHit, m_maxCastDistance, (int)VII.HitLayer.Unreachable);
        if (unreachableTileHitResult)
        {
            //Debug.Log(unreachableTileHit.collider.transform.name);
            //Debug.Log(Vector3.Distance(GroundDetector.transform.position, unreachableTileHit.transform.position));
            if(Vector3.Distance(GroundDetector.transform.position, unreachableTileHit.transform.position)
            <= VII.GameData.STEP_SIZE)
                return false;
        }

        RaycastHit[] iceHits;
        iceHits = Physics.RaycastAll(GroundDetector.transform.position,
            i_dir, m_maxCastDistance, (int)VII.HitLayer.Ice);
        // Sorting the hit results by distance, since RaycastAll doesn't guarantee the order of hit results.
        // More Info: https://docs.unity3d.com/ScriptReference/Physics.RaycastAll.html
        // More Info: https://forum.unity.com/threads/are-raycasthit-arrays-returned-from-raycastall-in-proper-order.385131/
        System.Array.Sort(iceHits, (x, y) => x.distance.CompareTo(y.distance));
        int expectationStep = 1;
        foreach (var item in iceHits)
        {
            //Debug.Log(Vector3.Distance(GroundDetector.transform.position, item.transform.position));
            // Player can't move that far
            if (Vector3.Distance(GroundDetector.transform.position, item.transform.position)
                - expectationStep * VII.GameData.STEP_SIZE > 0.2f * VII.GameData.STEP_SIZE)
                break;
            // Player can move 1 more step
            expectationStep++;
        }
        Vector3 end = transform.position + i_dir * expectationStep * VII.GameData.STEP_SIZE;
        if (i_costStep)
        {
            m_playerData.steps--;
        }
        if (i_smoothMove)
        {
            // Movement starts
            m_destination = end;
            m_playerData.playerState = VII.PlayerState.MOVING;
            VII.VIIEvents.TickStart.Invoke();
        }
        else
        {
            transform.position = end;
        }
        return true;
    }

    private void Update()
    {
        //print("level index" + m_RespawnPosIndex);
        //print(DiedInLevel0);
        // Input
        // TODO Support multiple device
        #region Input
        //if (Input.inputString != "")
        //    Debug.Log(Input.inputString);
        int horizontal = 0;
        int vertical = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            horizontal = -1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            horizontal = 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            vertical = 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            vertical = -1;
        }
        if (horizontal != 0)
        {
            vertical = 0;
        }
        #endregion
        if (horizontal != 0 || vertical != 0)
        {
            if (m_playerData.playerState == VII.PlayerState.IDLE)
            {
                if (horizontal != 0)
                {
                    if (horizontal == 1)
                    {
                        //animator.Play("WalkRight");
                    }
                    else
                    {
                        //animator.Play("WalkLeft");
                    }
                }
                else
                {
                    if (vertical == 1)
                    {
                        //animator.Play("WalkUp");
                    }
                    else
                    {
                        //animator.Play("WalkDown");
                    }
                }
                currentGridPos = transform.position;
                moveDir = new Vector3(horizontal, 0, vertical);
                Move(moveDir * VII.GameData.STEP_SIZE);
                nextGridPos = transform.position + moveDir * VII.GameData.STEP_SIZE;
            }
        }
        #region Moving
        if (m_playerData.playerState == VII.PlayerState.MOVING)
        {
            if (Vector3.Distance(transform.position, nextGridPos) < 0.2f)
            {
                currentGridPos = nextGridPos;
                nextGridPos += moveDir * VII.GameData.STEP_SIZE;
            }
            // If there is wall on ice
            RaycastHit bodyHit;
            bool bodyHitResult;
            bodyHitResult = Physics.Raycast(BodyDetector.transform.position,
           moveDir * VII.GameData.STEP_SIZE, out bodyHit, m_maxCastDistance, (int)VII.HitLayer.Block);
            if (bodyHitResult &&
                Vector3.Distance(BodyDetector.transform.position, bodyHit.transform.position)
                < VII.GameData.STEP_SIZE * 0.5f)
            {
                transform.position = new Vector3(bodyHit.transform.position.x - (moveDir * 0.5f).x, bodyHit.transform.position.y - VII.GameData.STEP_SIZE * 0.5f, bodyHit.transform.position.z - (moveDir * 0.5f).z);
                currentGridPos = transform.position;
                nextGridPos = currentGridPos;
                m_playerData.playerState = VII.PlayerState.IDLE;
                VII.VIIEvents.TickEnd.Invoke();
                if (m_playerData.steps <= 0)
                {
                    Respawn();
                }
                return;
            }
            if (Vector3.Distance(transform.position, m_destination) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    m_destination, Time.deltaTime * m_inverseMoveTime);
            }

            if (Vector3.Distance(transform.position, m_destination) < float.Epsilon)
            {
                // Movement ends
                transform.position = m_destination;
                currentGridPos = transform.position;
                nextGridPos = currentGridPos;
                m_playerData.playerState = VII.PlayerState.IDLE;
                VII.VIIEvents.TickEnd.Invoke();
                if (m_playerData.steps <= 0)
                {
                    Respawn();
                }
                //UI Update
               // UIManager.UIInstance.UpdateUI();
            }
    }
        #endregion
    }


    public void Respawn(bool costLife = true)
    {
        // Respawn Start
        if (m_playerData.playerState == VII.PlayerState.RESPAWNING)
        {
            return;
        }
        m_playerData.playerState = VII.PlayerState.RESPAWNING;
        if (costLife)
        {
            m_playerData.lives--;
        }
        else
        {
            m_playerData.lives = initLives;
        }
        VII.VIIEvents.PlayerRespawnStart.Invoke(this);
        //UI Update
        UIManager.UIInstance.UpdateUI();
        if (m_playerData.lives <= 0)
        {
            UIManager.UIInstance.startLevelIndex = CameraManager.Instance.big_level_index;
            UIManager.UIInstance.startRespawnIndex = restartRespawnPositionIndex;
            UIManager.UIInstance.startLives = initLives;
            SceneManager.LoadScene("All_Levels(Draft 1)");
            //Clear UI manager
            UIManager.UIInstance.ClearUI();
        }

        //Data for Achievements
        if (m_playerData.respawnPositionIndex == 0 && costLife == true)
        {
            DiedInLevel0 = true;
        }
        if (m_playerData.respawnPositionIndex == 5 && !DiedInTrapInLevel5 && costLife == true)
        {
            DiedInLevel5 = true;
        }

        StartCoroutine(Respawning(costLife));
    }

    private IEnumerator Respawning(bool costLife)
    {
        transform.position = nextGridPos;
        if (costLife)
        {
            //animator.Play("Death");
        }
        yield return null;
        // EVENT: Respawing Ends
        //Vector3 deathPos = transform.position;
        //Quaternion deathRot = transform.rotation;
        //ObjectPooler.Instance.SpawnFromPool("Body", deathPos, deathRot);
        InteractiveCollider.enabled = false;
        GroundDetector.SetActive(false);
        // Drop Items
        DropItems(costLife);
        transform.position = RespawnTargetGameObjects[m_playerData.respawnPositionIndex].transform.position
            + VII.GameData.PLAYER_RESPAWN_POSITION_OFFSET;
        InteractiveCollider.enabled = true;
        GroundDetector.SetActive(true);
        m_playerData.steps = initSteps;
        // Respawn Animation
        //animator.Play("Respawn");
        // Respawning Ends
        m_playerData.playerState = VII.PlayerState.IDLE;
        // Broadcast with Event System
        VII.VIIEvents.PlayerRespawnEnd.Invoke(this);
        //animator.Play("WalkDown");
    }

    private void DropItems(bool dropTombstone = true)
    {
        foreach (var item in Inventory.items)
        {
            if (item.droppable)
            {
                Instantiate(item.prefab, InteractableSpawnPoint.transform.position,
                Quaternion.identity);
            }
        }
        Inventory.RemoveDroppableItems();
        if (dropTombstone)
            /*Instantiate(TombstonePrefab, InteractableSpawnPoint.transform.position,
                    Quaternion.identity);*/
            Pools.SpawnFromPool("Tomb", InteractableSpawnPoint.transform.position, Quaternion.identity);
    }

    public void AddStep(int step)
    {
        m_playerData.steps += step;
    }


    public void SetRespawnPosition(int i_Next)
    {
        m_playerData.respawnPositionIndex = Mathf.Abs((RespawnTargetGameObjects.Count + m_playerData.respawnPositionIndex + i_Next) % RespawnTargetGameObjects.Count);
    }

    public void SetRestartRespawnPosition()
    {
        restartRespawnPositionIndex = m_playerData.respawnPositionIndex;
    }

    public void SetInitLives(int newLife)
    {
        initLives = newLife;
    }

    public void PerformMove()
    {
        Debug.Log("Use controller");
        //Debug.Log(playerInput.Player.Move.interactions);
    }

     // Getter
    public VII.PlayerData PlayerData { get { return m_playerData; } }
    public VII.PlayerState PlayerState { get { return m_playerData.playerState; } }
    public VII.Inventory Inventory { get { return m_playerData.Inventory; } }
    public int GetSteps() { return m_playerData.steps; }
    public int GetLives() { return m_playerData.lives; }
    public int GetRespawnPosIndex() { return m_playerData.respawnPositionIndex; }
}
