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

        public bool CheckMapFinished()
        {
            bool finishedTemp = true;
            foreach (var level in m_LevelData)
            {
                finishedTemp &= level.finished;
            }
            return finishedTemp;
        }

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
            foreach (var item in m_TileData)
            {
                if (item.GetComponent<Checkpoint>())
                {
                    m_Checkpoint = item.GetComponent<Checkpoint>();
                    break;
                }
            }
            if (!m_Checkpoint) Debug.Log("No checkpoint in " + m_LevelObject.name);
            // TODO Set respawn point
            m_RespawnPoint = m_TileData[0];
        }

        private GameObject m_LevelObject;
        private List<GameObject> m_TileData;
        private int m_LevelID;
        private Checkpoint m_Checkpoint;
        private GameObject m_RespawnPoint;

        private void OnLevelFinish(GameObject i_Invoker, Player i_Player)
        {
            this.finished |= i_Invoker == m_Checkpoint.gameObject;
            parentMapData.finished |= parentMapData.CheckMapFinished();
        }

        #region getters/setters
        public List<GameObject> GetTileData() { return m_TileData; }
        public Checkpoint GetCheckpoint() { return m_Checkpoint; }
        public GameObject GetRespawnPoint() { return m_RespawnPoint; }
        public bool finished { get; set; }
        public int GetLevelID() { return m_LevelID; }
        public MapData parentMapData { get; set; }
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

        #region getters
        public List<MapData> GetMapData() { return m_MapData; }
        #endregion
    }
}
