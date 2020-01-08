using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
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
            m_PlayerAnimationController = GetComponentInChildren<VII.PlayerAnimationController>();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region PlayerData
    [Header("Configuration")]
    public float moveTime = 0.5f;
    public float fallingSpeed = 5f;
    public int initSteps = VII.GameData.PLAYER_DEFAULT_STEPS;
    [Header("Game Objects")]
    public GameObject GroundDetector;
    public GameObject BodyDetector;
    public GameObject InteractableSpawnPoint;
    public Collider InteractiveCollider;
    [Header("Data for Achievements")]
    //map data
    public int levelIndex;
    public int mapIndex;
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
    //map completion
    public bool completeDungeon;
    public bool completeIce;
    public bool completeLava;
    public bool summonGreatOne;
    //optimized route
    public bool checkLeastLives;
    public int livesLeft;
    public bool playedLevel17; //TODO: This needs to be saved
    #endregion PlayerData
    [Header("Show Map Transition Texts")]
    public DialogueManager dialogueManager;
    //public Canvas transitionTextCanvas;
    //public bool display_text_trap;
    //public bool display_text_ice;
    //public bool display_text_lava;
    //public bool startSentence;
    public GameObject restartUI;

    private float m_inverseMoveTime;
    private const float m_maxCastDistance = 10f;
    private Vector3 m_destination;
    private int initLives = VII.GameData.PLAYER_DEFAULT_LIVES;
    private int bestLifeCost;
    private VII.PlayerData m_playerData;
    private Vector3 moveDir;
    private Vector3 currentGridPos;
    private Vector3 nextGridPos;
    private InputActions playerInput;
    private ObjectPooler Pools;
    private List<VII.MapData> mapData;
    private int currentLevelID;
    private int currentMapID;
    private RespawnPoint currentRespawnPoint;
    [SerializeField]
    private VII.PlayerAnimationController m_PlayerAnimationController;

    private void Start()
    {
        Pools = GameObject.Find("Pools").GetComponent<ObjectPooler>();
        mapData = VII.SceneDataManager.Instance.GetMapData();
        if (VII.SceneManager.instance.GetSave())
        {
            SavePlayerData data = SaveSystem.LoadPlayer();
            currentMapID = data.saveMapId;
            currentLevelID = data.saveLevelId;
        }
        else
        {
            currentMapID = VII.SceneManager.instance.GetStartMapID();
            currentLevelID = VII.SceneManager.instance.GetStartLevelID();
        }
        if (currentMapID > 0)
        {
            for (int i = 0; i < currentMapID; i++)
                mapData[i].GetMapObject().SetActive(false);
        }
        if (currentLevelID > 0)
        {
            mapData[currentMapID].GetLevelData()[currentLevelID - 1].GetCheckpoint().activated = true;
        }
        for (int i = 0; i < mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.childCount; i++)
        {
            if (mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.GetChild(i).name == "Level_blocker")
                mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.GetChild(i).GetComponent<Wall>().Move(new Vector3(0, 1, 0));
            if (mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.GetChild(i).name == "Level_blocker2")
                mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.GetChild(i).GetComponent<Wall>().Move(new Vector3(0, -1, 0));
        }
        mapData[currentMapID].GetLevelData()[currentLevelID].SetTilesEnabledState(true);
        currentRespawnPoint = mapData[currentMapID].GetLevelData()[currentLevelID].GetRespawnPoint();
        bestLifeCost = mapData[currentMapID].GetLevelData()[currentLevelID].GetBestLivesCost();
        transform.position = currentRespawnPoint.transform.position + VII.GameData.PLAYER_RESPAWN_POSITION_OFFSET;
        currentRespawnPoint.playerInside = true;
        tilePlayerInside = currentRespawnPoint;
        VII.VIIEvents.PlayerRespawnEnd.Invoke(this);
        m_playerData.playerState = VII.PlayerState.IDLE;
        UIManager.UIInstance.UpdateUI();
        m_PlayerAnimationController.InitStepUI();
    }

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
            if (Vector3.Distance(GroundDetector.transform.position, unreachableTileHit.transform.position)
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
            #region Presentation Layer
            m_PlayerAnimationController.TriggerAnimation(VII.PlayerAnimationState.Moving);
            if (expectationStep > 1)
            {
                m_PlayerAnimationController.TriggerSlidingAnimation(true);
                AudioManager.instance.PlaySingle(AudioManager.instance.slide);
            }
            if (m_playerData.steps > 0)
            {
                m_PlayerAnimationController.UpdateStepUI();
            }
            #endregion
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
        //Achievement stuff
        mapIndex = currentMapID;
        levelIndex = currentLevelID;
        livesLeft = m_playerData.lives;
        // Input
        // TODO Support multiple device
        #region Input
        if (Input.GetKeyDown(KeyCode.R))
        {
            VII.SceneManager.instance.SetStartMapID(currentMapID);
            VII.SceneManager.instance.SetStartLevelID(currentLevelID);
            VII.SceneManager.instance.SetStartStartLevelIndex(CameraManager.Instance.level_index);
            VII.SceneManager.instance.SetStartStartPPIndex(CameraManager.Instance.pp_index);
            VII.SceneManager.instance.SetStartStartFogIndex(CameraManager.Instance.fog_index);
            UIManager.UIInstance.ClearUI();
            VII.SceneManager.instance.LoadScene(VII.SceneType.GameScene);
        }
        int horizontal = 0;
        int vertical = 0;
        if (Input.GetAxis("Horizontal") < 0)
        {
            horizontal = -1;
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            horizontal = 1;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            vertical = 1;
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            vertical = -1;
        }
        if (horizontal != 0)
        {
            vertical = 0;
        }
        #endregion
        if ((horizontal != 0 || vertical != 0) & !dialogueManager.displayingTexts /*cant move when displaying sentences*/)
        {
            if (m_playerData.playerState == VII.PlayerState.IDLE && m_PlayerAnimationController.GetAnimationState() == VII.PlayerAnimationState.Idling)
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
            // If there is wall on path
            RaycastHit bodyHit;
            bool bodyHitResult;
            bodyHitResult = Physics.Raycast(BodyDetector.transform.position,
           moveDir * VII.GameData.STEP_SIZE, out bodyHit, VII.GameData.STEP_SIZE, (int)VII.HitLayer.Block);
            if (bodyHitResult &&
                Vector3.Distance(BodyDetector.transform.position, bodyHit.transform.position)
                < VII.GameData.STEP_SIZE * 0.5f)
            {
                transform.position = new Vector3(bodyHit.transform.position.x - (moveDir * 0.5f).x, bodyHit.transform.position.y - VII.GameData.STEP_SIZE * 0.5f, bodyHit.transform.position.z - (moveDir * 0.5f).z);
                currentGridPos = transform.position;
                nextGridPos = currentGridPos;
                m_PlayerAnimationController.TriggerSlidingAnimation(false);
                m_PlayerAnimationController.TriggerAnimation(VII.PlayerAnimationState.Idling);
                m_playerData.playerState = VII.PlayerState.IDLE;
                VII.VIIEvents.TickEnd.Invoke();
                if (m_playerData.steps <= 0)
                {
                    Respawn(true, false, true);
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
                m_PlayerAnimationController.TriggerSlidingAnimation(false);
                m_PlayerAnimationController.TriggerAnimation(VII.PlayerAnimationState.Idling);
                m_playerData.playerState = VII.PlayerState.IDLE;
                VII.VIIEvents.TickEnd.Invoke();
                if (m_playerData.steps <= 0)
                {
                    Respawn(true, false, true);
                }
            }
        }
        #endregion
    }


    public void Respawn(bool costLife = true, bool i_bSmoothMove = false, bool waitIdle = false)
    {
        // Respawn Start
        if (m_playerData.playerState == VII.PlayerState.RESPAWNING)
        {
            return;
        }
        m_playerData.playerState = VII.PlayerState.RESPAWNING;
        if (costLife)
        {
            m_playerData.lives++;
        }
        else
        {
            m_playerData.lives = initLives;
        }
        VII.VIIEvents.PlayerRespawnStart.Invoke(this);

        //Data for Achievements
        if (m_playerData.respawnPositionIndex == 0 && costLife == true)
        {
            DiedInLevel0 = true;
        }
        if (m_playerData.respawnPositionIndex == 5 && !DiedInTrapInLevel5 && costLife == true)
        {
            DiedInLevel5 = true;
        }
        #region Presentation Layer
        if (costLife)
            AudioManager.instance.PlaySingle(AudioManager.instance.death);
        else
            AudioManager.instance.PlaySingle(AudioManager.instance.checkpoint);
        #endregion
        StartCoroutine(Respawning(costLife, i_bSmoothMove, waitIdle));
    }

    private IEnumerator Respawning(bool costLife, bool i_bSmoothMove, bool waitIdle)
    {
        if (costLife)
        {
            while (Vector3.Distance(transform.position, nextGridPos) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    nextGridPos, Time.deltaTime * m_inverseMoveTime);
                yield return null;
            }
            transform.position = nextGridPos;
            if (waitIdle)
            {
                while (m_PlayerAnimationController.GetAnimationState() != VII.PlayerAnimationState.Idling)
                {
                    yield return null;
                }
            }
            // Death Animation 
            m_PlayerAnimationController.TriggerAnimation(VII.PlayerAnimationState.Death);
            while (m_PlayerAnimationController.GetAnimationState() != VII.PlayerAnimationState.Respawning)
            {
                yield return null;
            } 
        }
        m_PlayerAnimationController.ClearStepUI();
        // Drop Items
        DropItems(costLife);
        GetComponentInChildren<CrystalRotating>().DeactivateCrystal();
        // EVENT: Respawing Ends
        InteractiveCollider.enabled = false;
        GroundDetector.SetActive(false);

        m_destination = currentRespawnPoint.transform.position
            + VII.GameData.PLAYER_RESPAWN_POSITION_OFFSET;
        if (tilePlayerInside.GetComponent<Checkpoint>() && !costLife)
            yield return tilePlayerInside.GetComponent<Checkpoint>().WaitUntilAnimation(Checkpoint.hashIdleTag);
        if (i_bSmoothMove)
        {
            if (m_PlayerAnimationController.GetAnimationState() != VII.PlayerAnimationState.Respawning)
            {
                m_PlayerAnimationController.TriggerAnimation(VII.PlayerAnimationState.Respawning);
            }
            while (Vector3.Distance(transform.position, m_destination) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position,
                    m_destination, Time.fixedDeltaTime * fallingSpeed);
                yield return null;
            }
        }
        m_PlayerAnimationController.TriggerAnimation(VII.PlayerAnimationState.Reviving);
        transform.position = m_destination;
        currentGridPos = transform.position;
        nextGridPos = currentGridPos;
        currentRespawnPoint.playerInside = true;
        tilePlayerInside = currentRespawnPoint;
        InteractiveCollider.enabled = true;
        GroundDetector.SetActive(true);
        m_playerData.steps = initSteps;
        // Respawn Animation
        while(m_PlayerAnimationController.GetAnimationState() != VII.PlayerAnimationState.Idling)
        {
            yield return null;
        }
        if (i_bSmoothMove)
            VII.SceneDataManager.Instance.GetCurrentMapData().previousMap.GetMapObject().SetActive(false);
        UIManager.UIInstance.UpdateUI();
        m_PlayerAnimationController.InitStepUI();
        // Respawning Ends
        m_playerData.playerState = VII.PlayerState.IDLE;
        if (m_playerData.lives > bestLifeCost)
        {
            restartUI.GetComponent<Animator>().SetBool("Active", true);
        }
        else
        {
            restartUI.GetComponent<Animator>().SetBool("Active", false);
        }
        // Broadcast with Event System
        VII.VIIEvents.PlayerRespawnEnd.Invoke(this);
        if (!costLife)
            SavePlayer();
    }

    private void DropItems(bool dropTombstone = true)
    {
        foreach (var item in Inventory.items)
        {
            if (item.droppable)
            {
                GameObject itemDroped = Instantiate(item.prefab, InteractableSpawnPoint.transform.position,
                Quaternion.identity);
                itemDroped.transform.parent = mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform;
            }
        }
        Inventory.RemoveDroppableItems();
        if (dropTombstone)
        {
            GameObject tomb = Pools.SpawnFromPool("Tomb", InteractableSpawnPoint.transform.position, Quaternion.identity);
            tomb.transform.parent = mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform;
        }
    }

    public void AddStep(int step)
    {
        m_playerData.steps += step;
    }


    public void SetRespawnPoint(int i_Next)
    {
        if (currentLevelID + i_Next < mapData[currentMapID].GetLevelData().Count && currentLevelID + i_Next >= 0)
        {
            currentLevelID += i_Next;
        }
        // go to previous map
        else if (currentLevelID + i_Next < 0)
        {
            if (currentMapID + i_Next >= 0)
            {
                currentMapID += i_Next;
                currentLevelID = mapData[currentMapID].GetLevelData().Count - 1;
            }
            else if (currentMapID + i_Next < 0)
            {
                currentMapID = mapData.Count - 1;
                currentLevelID = mapData[currentMapID].GetLevelData().Count - 1;
            }
        }
        // go to next map
        else if (currentLevelID + i_Next >= mapData[currentMapID].GetLevelData().Count)
        {
            if (currentMapID + i_Next < mapData.Count)
            {
                currentMapID += i_Next;
                currentLevelID = 0;
            }
            else if (currentMapID + i_Next >= mapData.Count)
            {
                currentMapID = 0;
                currentLevelID = 0;
            }
        }
        currentRespawnPoint = mapData[currentMapID].GetLevelData()[currentLevelID].GetRespawnPoint();
        mapData[currentMapID].GetLevelData()[currentLevelID].SetTilesEnabledState(true);
        bestLifeCost = mapData[currentMapID].GetLevelData()[currentLevelID].GetBestLivesCost();
    }

    public void PerformMove()
    {
        Debug.Log("Use controller");
        //Debug.Log(playerInput.Player.Move.interactions);
    }
    public void SavePlayer()
    {

        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        SavePlayerData data = SaveSystem.LoadPlayer();
        currentMapID = data.saveMapId;
        currentLevelID = data.saveLevelId;
        currentRespawnPoint = mapData[currentMapID].GetLevelData()[currentLevelID].GetRespawnPoint();
        CameraManager.Instance.SwitchPostProcessing(data.savePPIndex);
        CameraManager.Instance.SwitchLevelCamera(data.cameraIndex - CameraManager.Instance.level_index);
        CameraManager.Instance.SwitchFog(data.saveFogIndex);
        if (currentMapID > 0)
        {
            for (int i = 0; i < currentMapID; i++)
                mapData[i].GetMapObject().SetActive(false);
        }
        if (currentLevelID > 0)
        {
            mapData[currentMapID].GetLevelData()[currentLevelID - 1].GetCheckpoint().activated = true;
        }
        for (int i = 0; i < mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.childCount; i++)
        {
            if (mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.GetChild(i).name == "Level_blocker")
                mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.GetChild(i).GetComponent<Wall>().Move(new Vector3(0, 1, 0));
            if (mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.GetChild(i).name == "Level_blocker2")
                mapData[currentMapID].GetLevelData()[currentLevelID].GetLevelObject().transform.GetChild(i).GetComponent<Wall>().Move(new Vector3(0, -1, 0));
        }
        mapData[currentMapID].GetLevelData()[currentLevelID].SetTilesEnabledState(true);
        bestLifeCost = mapData[currentMapID].GetLevelData()[currentLevelID].GetBestLivesCost();
        //m_playerData.lives = data.savelives;
        Respawn(false);
    }

    // Getters/Setters
    public VII.PlayerData PlayerData { get { return m_playerData; } }
    public VII.PlayerState PlayerState { get { return m_playerData.playerState; } }
    public VII.Inventory Inventory { get { return m_playerData.Inventory; } }
    public int GetSteps() { return m_playerData.steps; }
    public int GetLives() { return m_playerData.lives; }
    public int GetPosIndex() { return m_playerData.respawnPositionIndex; }
    public Vector3 GetMoveDirection() { return moveDir; }
    [HideInInspector]
    public Tile tilePlayerInside { get; set; }
    
}
