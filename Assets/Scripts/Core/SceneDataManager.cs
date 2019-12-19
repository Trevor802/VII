using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public abstract class BasicData
{
    public int m_InstanceID;
    public string m_Name;
    public int m_IsActive;
    public Vector3 m_LocalPosition;
    public Quaternion m_LocalRotation;
}

[Serializable]
public class MapData : BasicData
{
    public LevelData[] m_Levels;
}

[Serializable]
public class LevelData : BasicData
{
    public TileData[] m_Tiles;
}

[Serializable]
public class TileData : BasicData
{


    public int m_SourcePrefab;
    public Dictionary<string, string> m_Attributes;

    private LevelData m_ParentLevel;
}

public class SceneDataManager : MonoBehaviour
{
    private List<MapData> m_Maps = new List<MapData>();

    private JSONObject m_JSONObject;

    private void Awake()
    {
        string jsonFilePath = SceneManager.GetActiveScene().name + ".json";
        m_JSONObject = LoadJSONObject(Path.Combine(Application.streamingAssetsPath, jsonFilePath));
        DecodeJSON(m_JSONObject);
    }

    private JSONObject LoadJSONObject(string i_JSONFilePath)
    {
        if (File.Exists(i_JSONFilePath))
        {
            string data = File.ReadAllText(i_JSONFilePath);
            return new JSONObject(data);
        }
        return null;
    }

    // TODO Retrieve data using the recursive function
    private void AccessData(JSONObject obj)
    {
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int i = 0; i < obj.list.Count; i++)
                {
                    string key = (string)obj.keys[i];
                    JSONObject j = (JSONObject)obj.list[i];
                    Debug.Log(key);
                    AccessData(j);
                }
                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in obj.list)
                {
                    AccessData(j);
                }
                break;
            case JSONObject.Type.STRING:
                Debug.Log(obj.str);
                break;
            case JSONObject.Type.NUMBER:
                Debug.Log(obj.n);
                break;
            case JSONObject.Type.BOOL:
                Debug.Log(obj.b);
                break;
            case JSONObject.Type.NULL:
                Debug.Log("NULL");
                break;

        }
    }

    // A stupid function to decode json file and generate class data.
    // Will be replaced in the future
    private void DecodeJSON(JSONObject i_JSONObject)
    {
        for (int i = 0; i < i_JSONObject.Count; i++)
        {
            m_Maps.Add(JsonUtility.FromJson<MapData>(i_JSONObject[i].ToString()));
        }
        for (int i = 0; i < m_Maps.Count; i++)
        {
            for (int j = 0; j < m_Maps[i].m_Levels.Length; j++)
            {
                for (int k = 0; k < m_Maps[i].m_Levels[j].m_Tiles.Length; k++)
                {
                    m_Maps[i].m_Levels[j].m_Tiles[k].m_Attributes =
                        i_JSONObject[i].GetField("m_Levels")[j].GetField("m_Tiles")[k].GetField("m_Attributes").ToDictionary();
                }
            }
        }
    }

    #region getter
    public List<MapData> GetMapData()
    {
        return m_Maps;
    }
    #endregion
}
