using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        Interactable = 1 << 11
    }
}

public class Player : MonoBehaviour
{
    #region Singleton
    public static Player Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Initialization
            m_playerData = new VII.PlayerData(initLives, initSteps,
                RespawnPositions[m_RespawnPosIndex].transform.position);
            m_inverseMoveTime = 1 / moveTime;
            RespawnPositions[m_RespawnPosIndex].transform.parent.parent.gameObject.SetActive(true);
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
    public List<GameObject> RespawnPositions; 
    [Header("Prefabs")]
    public GameObject TombstonePrefab;
    #endregion PlayerData

    private float m_inverseMoveTime;
    private const float m_maxCastDistance = 10f;
    private Vector3 m_destination;
    private int m_RespawnPosIndex = 0;
    private VII.PlayerData m_playerData;
    private Vector3 moveDir;
    private Vector3 currentGridPos;
    private Vector3 nextGridPos;

    public bool Move(Vector3 i_dir, bool i_costStep = true, bool i_smoothMove = true)
    {
        // Ground detection
        RaycastHit bodyHit;
        bool bodyHitResult;
        RaycastHit[] groundHits;
        bodyHitResult = Physics.Raycast(BodyDetector.transform.position,
            i_dir, out bodyHit, m_maxCastDistance, (int)VII.HitLayer.Block);
        // Player can't move to that direction even 1 grid

        if (bodyHitResult &&
            Vector3.Distance(BodyDetector.transform.position, bodyHit.transform.position)
            < VII.GameData.STEP_SIZE)
            return false;
        groundHits = Physics.RaycastAll(GroundDetector.transform.position,
            i_dir, m_maxCastDistance, (int)VII.HitLayer.Ice);
        // Sorting the hit results by distance, since RaycastAll doesn't guarantee the order of hit results.
        // More Info: https://docs.unity3d.com/ScriptReference/Physics.RaycastAll.html
        // More Info: https://forum.unity.com/threads/are-raycasthit-arrays-returned-from-raycastall-in-proper-order.385131/
        System.Array.Sort(groundHits, (x, y) => x.distance.CompareTo(y.distance));
        int expectationStep = 1;
        foreach (var item in groundHits)
        {
            // Player can't move that far
            if (Vector3.Distance(GroundDetector.transform.position, item.transform.position)
                - expectationStep * VII.GameData.STEP_SIZE > float.Epsilon)
                break;
            // That position is blocked by wall
            /*if (bodyHitResult &&
                Mathf.Abs(bodyHit.distance + VII.GameData.WALL_WIDTH * 0.5f - item.distance - VII.GameData.STEP_SIZE)
                < VII.GameData.EQUAL_DEVIATION)
                break;*/
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
        // Input
        // TODO Support multiple device
        #region Input
        int horizontal = 0;
        int vertical = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            horizontal = -1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            horizontal = 1;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            vertical = 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
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
            RaycastHit bodyHit;
            bool bodyHitResult;
            bodyHitResult = Physics.Raycast(BodyDetector.transform.position,
           moveDir * VII.GameData.STEP_SIZE, out bodyHit, m_maxCastDistance, (int)VII.HitLayer.Block);
            if (Vector3.Distance(transform.position, nextGridPos) < 0.2f)
            {
                currentGridPos = nextGridPos;
                nextGridPos += moveDir * VII.GameData.STEP_SIZE;
            }
            if (bodyHitResult &&
                Vector3.Distance(BodyDetector.transform.position, bodyHit.transform.position)
                < VII.GameData.STEP_SIZE * 0.5f)
            {
                transform.position = new Vector3(bodyHit.transform.position.x - (moveDir * 0.5f).x, 0, bodyHit.transform.position.z - (moveDir * 0.5f).z);
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
        if (m_playerData.lives <= 0)
        {
            //return;
            m_playerData.lives = initLives;
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
        // Drop Items
        DropItems();
        transform.position = m_playerData.respawnPosition;
        InteractiveCollider.enabled = true;
        m_playerData.steps = initSteps;
        // Respawn Animation
        //animator.Play("Respawn");
        // Respawning Ends
        m_playerData.playerState = VII.PlayerState.IDLE;
        // Broadcast with Event System
        VII.VIIEvents.PlayerRespawnEnd.Invoke(this);
        //animator.Play("WalkDown");
    }

    private void DropItems()
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
        Instantiate(TombstonePrefab, InteractableSpawnPoint.transform.position,
                    Quaternion.identity);
    }

    public void AddStep(int step)
    {
        m_playerData.steps += step;
    }

    public void SetRespawnPosition(int i_Next)
    {
        m_RespawnPosIndex = Mathf.Abs((m_RespawnPosIndex + i_Next) % RespawnPositions.Count);
        PlayerData.respawnPosition = RespawnPositions[m_RespawnPosIndex].transform.position;
        RespawnPositions[m_RespawnPosIndex].transform.parent.parent.gameObject.SetActive(true);
    }

     // Getter
    public VII.PlayerData PlayerData { get { return m_playerData; } }
    public VII.PlayerState PlayerState { get { return m_playerData.playerState; } }
    public VII.Inventory Inventory { get { return m_playerData.Inventory; } }
    public int GetSteps() { return m_playerData.steps; }
    public int GetLives() { return m_playerData.lives; }
}
