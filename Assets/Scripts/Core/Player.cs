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
        Ice = 1 << 9
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
            m_playerData = new VII.PlayerData(initLives, initSteps, transform.position);
            m_inverseMoveTime = 1 / moveTime;
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
    #endregion PlayerData

    
    private float m_inverseMoveTime;
    private const float m_maxCastDistance = 10f;
    private Vector3 m_destination;
    private VII.PlayerData m_playerData;

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
            Mathf.Abs(Vector3.Distance(BodyDetector.transform.position, bodyHit.transform.position)
            - VII.GameData.STEP_SIZE) < float.Epsilon)
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
            if (bodyHitResult &&
                Mathf.Abs(bodyHit.distance - item.distance - VII.GameData.STEP_SIZE)
                < float.Epsilon)
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

    private IEnumerator Respawning(bool costLife)
    {
        if (costLife)
        {
            //animator.Play("Death");
        }
        yield return null;
        // EVENT: Respawing Ends
        //Vector3 deathPos = transform.position;
        //Quaternion deathRot = transform.rotation;
        //ObjectPooler.Instance.SpawnFromPool("Body", deathPos, deathRot);
        transform.position = m_playerData.respawnPosition;
        m_playerData.steps = initLives;
        // Respawn Animation
        //animator.Play("Respawn");
        // Respawning Ends
        m_playerData.playerState = VII.PlayerState.IDLE;
        // Broadcast with Event System
        VII.VIIEvents.PlayerRespawnEnd.Invoke(this);
        //animator.Play("WalkDown");
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
                Move(new Vector3(horizontal * VII.GameData.STEP_SIZE, 0, vertical * VII.GameData.STEP_SIZE));
            }
        }
        #region Moving
        if (m_playerData.playerState == VII.PlayerState.MOVING)
        {
            if (Vector3.Distance(transform.position, m_destination) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    m_destination, Time.deltaTime * m_inverseMoveTime);
            }
            else
            {
                // Movement ends
                VII.VIIEvents.TickEnd.Invoke();
                m_playerData.playerState = VII.PlayerState.IDLE;
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
            return;
        }
        StartCoroutine(Respawning(costLife));
    }

    public void AddStep(int step)
    {
        m_playerData.steps += step;
    }

    public void GameWin()
    {
        m_playerData.playerState = VII.PlayerState.ENDING;
    }

    // Getter
    public VII.PlayerData GetPlayerData() { return m_playerData; }
    public VII.PlayerState GetPlayerState() { return m_playerData.playerState; }
    public int GetSteps() { return m_playerData.steps; }
    public int GetLives() { return m_playerData.lives; }
}
