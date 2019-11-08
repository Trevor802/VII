using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace VII
{
    public enum PlayerState
    {
        IDLE = 0,
        MOVING = 1,
        RESPAWNING = 2,
        ENDING = 3
    }

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

    private Vector3 m_respawnPos;
    private int m_steps;
    private int m_lives;
    private float m_inverseMoveTime;
    private Coroutine m_movingCoroutine;
    private VII.PlayerState m_playerState;
    private const float m_maxCastDistance = 10f;
    private Vector3 m_destination;
    private float m_stepSize = 1f;

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
            - m_stepSize) < float.Epsilon)
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
                - expectationStep * m_stepSize > float.Epsilon)
                break;
            // That position is blocked by wall
            if (bodyHitResult &&
                Mathf.Abs(bodyHit.distance - item.distance - m_stepSize)
                < float.Epsilon)
                break;
            // Player can move 1 more step
            expectationStep++;
        }
        Vector3 end = transform.position + i_dir * expectationStep * m_stepSize;
        if (i_costStep)
        {
            m_steps--;
        }
        if (i_smoothMove)
        {
            // Movement starts
            m_destination = end;
            m_playerState = VII.PlayerState.MOVING;
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
        // yield return new WaitForSeconds(spawnDur);

        // EVENT: Respawing Ends
        Vector3 deathPos = transform.position;
        Quaternion deathRot = transform.rotation;
        //ObjectPooler.Instance.SpawnFromPool("Body", deathPos, deathRot);
        transform.position = m_respawnPos;
        m_steps = initSteps;
        // Respawn Animation
        //animator.Play("Respawn");
        // Respawning Ends
        m_playerState = VII.PlayerState.IDLE;
        // Broadcast with Event System
        //VII.VIIEvents.PlayerRespawnEnd.Invoke(this);
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
            if (m_playerState == VII.PlayerState.IDLE)
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
                Move(new Vector3(horizontal * m_stepSize, 0, vertical * m_stepSize));
            }
        }
        #region Moving
        if (m_playerState == VII.PlayerState.MOVING)
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
                m_playerState = VII.PlayerState.IDLE;
            }
        }
        #endregion
    }

    public void ResetRespawnPos(Vector3 pos)
    {
        m_respawnPos = pos;
    }

    public void Respawn(bool costLife = true)
    {
        // Respawn Start
        if (m_playerState == VII.PlayerState.RESPAWNING)
        {
            return;
        }
        m_playerState = VII.PlayerState.RESPAWNING;
        if (costLife)
        {
            m_lives--;
        }
        else
        {
            m_lives = initLives;
        }
        if (m_movingCoroutine != null)
        {
            StopCoroutine(m_movingCoroutine);
        }
        //VII.VIIEvents.PlayerRespawnStart.Invoke(this);
        if (m_lives <= 0)
        {
            return;
        }
        StartCoroutine(Respawning(costLife));
    }

    public void AddStep(int step)
    {
        m_steps += step;
    }

    public void GameWin()
    {
        m_playerState = VII.PlayerState.ENDING;
    }

    // Getter

    public VII.PlayerState GetPlayerState() { return m_playerState; }
    public int GetSteps() { return m_steps; }
    public int GetLives() { return m_lives; }
}
