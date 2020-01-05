using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VII
{
    public class MapData
    {
        public MapData(List<LevelData> i_Levels, int i_MapID, GameObject i_MapObject)
        {
            #region Constructor
            m_LevelData = i_Levels;
            m_MapID = i_MapID;
            m_MapObject = i_MapObject;
            #endregion

            m_LevelData.ForEach(x => x.parentMapData = this);
        }

        private List<LevelData> m_LevelData;
        private int m_MapID;
        private GameObject m_MapObject;
        #region Logic Layer
        public bool CheckMapFinished()
        {
            bool finishedTemp = true;
            foreach (var level in m_LevelData)
            {
                finishedTemp &= level.finished;
            }
            return finishedTemp;
        }

        public bool GetPlayerInside()
        {
            foreach (var level in m_LevelData)
            {
                if (level.GetPlayerInside())
                {
                    return true;
                }
            }
            return false;
        }

        public LevelData GetLevelPlayerInside()
        {
            foreach (var level in m_LevelData)
            {
                if (level.GetPlayerInside())
                {
                    return level;
                }
            }
            return null;
        }
        #endregion
        #region getters/setters
        public List<LevelData> GetLevelData() { return m_LevelData; }
        public int GetMapID() { return m_MapID; }
        public GameObject GetMapObject() { return m_MapObject; }
        public bool finished { get; set; }
        #endregion
    }

    public class LevelData
    {
        public LevelData(int i_LevelID, GameObject i_LevelObject)
        {
            #region Constructor
            m_LevelID = i_LevelID;
            m_LevelObject = i_LevelObject;
            #endregion

            VIIEvents.LevelFinish.AddListener(OnLevelFinish);
            m_Checkpoint = GetComponentInTiles<Checkpoint>();
            if (!m_Checkpoint) Debug.Log("No checkpoint in " + m_LevelObject.name);
            m_RespawnPoint = GetComponentInTiles<RespawnPoint>();
            if (!m_RespawnPoint) Debug.Log("No respawn point in " + m_LevelObject.name);
            if (m_RespawnPoint) m_BestLifeCost = m_RespawnPoint.bestLifeCost;
        }

        private GameObject m_LevelObject;
        private int m_LevelID;
        private Checkpoint m_Checkpoint;
        private RespawnPoint m_RespawnPoint;
        private int m_BestLifeCost;

        #region Logic Layer
        private void OnLevelFinish(GameObject i_Invoker, Player i_Player)
        {
            if (m_Checkpoint != null)
            {
                finished |= i_Invoker == m_Checkpoint.gameObject;
                parentMapData.finished |= parentMapData.CheckMapFinished();
            }
        }

        public List<GameObject> GetChildrenObjectsWithTag(string i_Tag)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (Transform item in m_LevelObject.transform)
            {
                if (item.CompareTag(i_Tag))
                {
                    children.Add(item.gameObject);
                }
            }
            return children;
        }

        private T GetComponentInTiles<T>() where T : Component
        {
            foreach (var item in GetChildrenObjectsWithTag(GameData.TILE_TAG))
            {
                if (item.GetComponent<T>() != null)
                {
                    return item.GetComponent<T>() as T;
                }
            }
            return null;
        }

        public bool GetPlayerInside()
        {
            foreach (var tile in GetChildrenObjectsWithTag(GameData.TILE_TAG))
            {
                if (!tile)
                {
                    Debug.Log(m_LevelObject);
                }
                if (tile.GetComponent<Tile>() &&
                    tile.GetComponent<Tile>() == Player.Instance.tilePlayerInside)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetTilesEnabledState(bool i_bState)
        {
            List<GameObject> TileList = GetChildrenObjectsWithTag(GameData.TILE_TAG);
            foreach (GameObject tile in TileList)
            {
                if (tile.GetComponent<Tile>())
                    tile.GetComponent<Tile>().SetReceiveTick(i_bState);
            }
        }

        #endregion

        #region getters/setters
        public Checkpoint GetCheckpoint() { return m_Checkpoint; }
        public RespawnPoint GetRespawnPoint() { return m_RespawnPoint; }
        public bool finished { get; set; }
        public int GetLevelID() { return m_LevelID; }
        public MapData parentMapData { get; set; }
        public int GetBestLivesCost() { return m_BestLifeCost; }
        public GameObject GetLevelObject() { return m_LevelObject; }
        #endregion
    }

    public class SceneDataManager : MonoBehaviour
    {
        public static SceneDataManager Instance = null;

        private List<MapData> m_MapData;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            var maps = GameObject.Find("Maps");
            var mapList = new List<GameObject>();
            foreach (Transform map in maps.transform)
            {
                if (map.CompareTag(GameData.MAP_TAG))
                {
                    mapList.Add(map.gameObject);
                }
            }
            var listOfMapData = new List<MapData>();
            int mapID = 0;
            foreach (var map in mapList)
            {
                var levels = new List<GameObject>();
                foreach (Transform level in map.transform)
                {
                    if (level.CompareTag(GameData.LEVEL_TAG))
                    {
                        levels.Add(level.gameObject);
                    }
                }
                var listOfLevelData = new List<LevelData>();
                int levelID = 0;
                foreach (var level in levels)
                {
                    listOfLevelData.Add(new LevelData(levelID, level.gameObject));
                    levelID++;
                }
                listOfMapData.Add(new MapData(listOfLevelData, mapID, map));
                mapID++;
            }
            m_MapData = listOfMapData;
        }

        public MapData GetCurrentMapData()
        {
            foreach (var map in m_MapData)
            {
                if (map.GetPlayerInside())
                {
                    return map;
                }
            }
            return null;
        }

        public LevelData GetCurrentLevelData()
        {
            MapData mapData = GetCurrentMapData();
            return mapData.GetLevelPlayerInside();
        }

        // Only works at runtime
        public Tile GetCurrentTileData()
        {
            return Player.Instance.tilePlayerInside;
        }

        #region getters
        public List<MapData> GetMapData() { return m_MapData; }
        #endregion
    }
}
