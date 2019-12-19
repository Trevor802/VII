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
        public bool finished { get; set; }
        #endregion
    }

    public class LevelData
    {
        public LevelData(List<GameObject> i_Tiles, int i_LevelID, GameObject i_LevelObject)
        {
            #region Constructor
            m_TileData = i_Tiles;
            m_LevelID = i_LevelID;
            m_LevelObject = i_LevelObject;
            #endregion

            VIIEvents.LevelFinish.AddListener(OnLevelFinish);
            m_Checkpoint = GetComponentInTiles<Checkpoint>();
            if (!m_Checkpoint) Debug.Log("No checkpoint in " + m_LevelObject.name);
            // TODO Set respawn point
            m_RespawnPoint = GetComponentInTiles<RespawnPoint>();
            if (!m_RespawnPoint) Debug.Log("No respawn point in " + m_LevelObject.name);
            if (m_RespawnPoint) m_PlayerLivesAvailable = m_RespawnPoint.livesAvailable;
        }

        private GameObject m_LevelObject;
        private List<GameObject> m_TileData;
        private int m_LevelID;
        private Checkpoint m_Checkpoint;
        private RespawnPoint m_RespawnPoint;
        private int m_PlayerLivesAvailable = GameData.PLAYER_DEFAULT_LIVES;

        #region Logic Layer
        private void OnLevelFinish(GameObject i_Invoker, Player i_Player)
        {
            this.finished |= i_Invoker == m_Checkpoint.gameObject;
            parentMapData.finished |= parentMapData.CheckMapFinished();
        }

        private T GetComponentInTiles<T>() where T : Component
        {
            foreach (var item in m_TileData)
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
            foreach (var tile in m_TileData)
            {
                if (tile.GetComponent<Tile>() && tile.GetComponent<Tile>().GetPlayerInside())
                {
                    return true;
                }
            }
            return false;
        }

        public Tile GetTilePlayerInside()
        {
            foreach (var tile in m_TileData)
            {
                if (tile.GetComponent<Tile>() && tile.GetComponent<Tile>().GetPlayerInside())
                {
                    return tile.GetComponent<Tile>();
                }
            }
            return null;
        }

        public void SetTilesEnabledState(bool i_bState)
        {
            m_TileData.ForEach(x => x.GetComponent<Tile>().SetReceiveTick(i_bState));
        }

        #endregion

        #region getters/setters
        public List<GameObject> GetTileData() { return m_TileData; }
        public Checkpoint GetCheckpoint() { return m_Checkpoint; }
        public RespawnPoint GetRespawnPoint() { return m_RespawnPoint; }
        public bool finished { get; set; }
        public int GetLevelID() { return m_LevelID; }
        public MapData parentMapData { get; set; }
        public int GetPlayerLives() { return m_PlayerLivesAvailable; }
        #endregion
    }

    public class SceneDataManager : MonoBehaviour
    {
        private List<MapData> m_MapData;
        private const string m_MapTag = "Map";
        private const string m_LevelTag = "Level";
        private const string m_TileTag = "Tile";

        private void Awake()
        {
            var maps = GameObject.FindGameObjectsWithTag(m_MapTag);
            var listOfMapData = new List<MapData>();
            int mapID = 0;
            foreach (var map in maps)
            {
                var levels = new List<GameObject>();
                foreach (Transform level in map.transform)
                {
                    if (level.CompareTag(m_LevelTag))
                    {
                        levels.Add(level.gameObject);
                    }
                }
                var listOfLevelData = new List<LevelData>();
                int levelID = 0;
                foreach (var level in levels)
                {
                    var tiles = new List<GameObject>();
                    foreach (Transform child in level.transform)
                    {
                        if (child.CompareTag(m_TileTag))
                        {
                            tiles.Add(child.gameObject);
                        }
                    }
                    listOfLevelData.Add(new LevelData(tiles, levelID, level.gameObject));
                    levelID++;
                }
                listOfMapData.Add(new MapData(listOfLevelData, mapID, map));
                mapID++;
            }
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

        public Tile GetCurrentTileData()
        {
            LevelData levelData = GetCurrentLevelData();
            return levelData.GetTilePlayerInside();
        }

        #region getters
        public List<MapData> GetMapData() { return m_MapData; }
        #endregion
    }
}
