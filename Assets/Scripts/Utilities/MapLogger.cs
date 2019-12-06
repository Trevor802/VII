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

    public LevelLogData(int i_remainingLives, int i_remainingSteps,
        float i_levelElapsedTime, string i_levelName) =>
        (remainingLives, remainingSteps, levelElapsedTime, levelName) =
        (i_remainingLives, i_remainingSteps, i_levelElapsedTime, i_levelName);
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
    private string m_levelDataJson;
    private float m_levelElapsedTime;
    private int m_levelIndex;
    private List<LevelLogData> m_levelData = new List<LevelLogData>();

    private void Awake()
    {
        m_mapName = SceneManager.GetActiveScene().name;
        m_startTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        VII.VIIEvents.LevelFinish.AddListener(OnLevelFinish);
    }

    private void Update()
    {
        m_elapsedTime = Time.time;
    }

    private void OnDestroy()
    {
        m_endTime = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        m_levelDataJson = JsonHelper.ToJson(m_levelData.ToArray());
        string data = JsonUtility.ToJson(this);
        //data = data.Replace("\\", "");
        LogData(data);
    }

    private void OnLevelFinish(Player i_player)
    {
        m_levelElapsedTime = m_elapsedTime - m_levelElapsedTime;
        m_levelData.Add(new LevelLogData(i_player.GetLives(), i_player.GetSteps(),
            m_levelElapsedTime, m_levelIndex++.ToString()));
    }

    private void LogData(string i_data)
    {
        string logPath = Application.dataPath + "/MapLog.json";
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
