using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[Serializable]
public class LevelLogData
{
    public string levelName;
    public int remainingLives;
    public int remainingSteps;
    public float levelElapsedTime;
    private JSONObject m_JSON = new JSONObject(JSONObject.Type.OBJECT);

    public LevelLogData(int i_remainingLives, int i_remainingSteps,
        float i_levelElapsedTime, string i_levelName)
    {
        levelName = i_levelName;
        remainingLives = i_remainingLives;
        remainingSteps = i_remainingSteps;
        levelElapsedTime = i_levelElapsedTime;
        m_JSON.AddField("LevelName", this.levelName);
        m_JSON.AddField("RemainingLives", this.remainingLives);
        m_JSON.AddField("RemainingSteps", this.remainingSteps);
        m_JSON.AddField("LevelElapsedTime", this.levelElapsedTime);
    }

    public JSONObject GetJSONObject()
    {
        return m_JSON;
    }
}

[Serializable]
public class MapLogger : MonoBehaviour
{
    [SerializeField]
    private string m_mapName;
    [SerializeField]
    private string m_startTime;
    [SerializeField]
    private string m_endTime;
    [SerializeField]
    private float m_elapsedTime;
    [SerializeField]
    private float m_levelElapsedTime;
    private int m_levelIndex;
    private List<LevelLogData> m_levelData = new List<LevelLogData>();
    private JSONObject m_JSON = new JSONObject(JSONObject.Type.OBJECT);
    private JSONObject m_LevelJSONArray = new JSONObject(JSONObject.Type.ARRAY);

    private void Awake()
    {
        JSONSetFields(m_JSON);
        m_mapName = SceneManager.GetActiveScene().name;
        m_startTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        //VII.VIIEvents.LevelFinish.AddListener(OnLevelFinish);
    }

    private void JSONSetFields(JSONObject i_JSONObject)
    {
        i_JSONObject.SetField("MapName", m_mapName);
        i_JSONObject.SetField("StartTime", m_startTime);
        i_JSONObject.SetField("EndTime", m_endTime);
        i_JSONObject.SetField("ElapsedTime", m_elapsedTime);
        i_JSONObject.SetField("LevelData", m_LevelJSONArray);
    }

    private void Update()
    {
        m_elapsedTime = Time.time;
    }

    private void OnDestroy()
    {
        m_endTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        JSONSetFields(m_JSON);
        LogData(m_JSON.ToString());
    }

    private void OnLevelFinish(GameObject i_Invoker, Player i_player)
    {
        m_levelElapsedTime = m_elapsedTime - m_levelElapsedTime;
        LevelLogData levelLog = new LevelLogData(i_player.GetLives(), i_player.GetSteps(),
            m_levelElapsedTime, m_levelIndex++.ToString());
        m_levelData.Add(levelLog);
        m_LevelJSONArray.Add(levelLog.GetJSONObject());
    }

    private void LogData(string i_data)
    {
        string logPath = Application.dataPath + "/Resources/MapLog.json";
        if (!File.Exists(logPath))
        {
            File.WriteAllText(logPath, "[");
        }
        else
        {
            FileStream fs = File.OpenWrite(logPath);
            fs.SetLength(fs.Length - 1);
            fs.Close();
            File.AppendAllText(logPath, ",");
        }
        File.AppendAllText(logPath, i_data + "]");
    }
}
