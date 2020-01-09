using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VII
{
    public enum SceneType { MainScene, GameScene, RestartScene, WinScene };

    [System.Serializable]
    public class MusicOfScene
    {
        public SceneType SceneType;
        public AudioClip MusicClip;
    }

    public class SceneManager : MonoBehaviour
    {
        #region Singleton
        public static SceneManager instance = null;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(transform.gameObject);
        }
        #endregion
        public List<MusicOfScene> music;
        private Dictionary<SceneType, AudioClip> musicOfScenes;
        private bool saveLoaded = false;
        private int startMapID = 0;
        private int startLevelID = 0;
        private int startLevelIndex = 0;
        private int startPPIndex = 0;
        private int startFogIndex = 0;
        private float startMusicVolume = 1;
        private float startSoundVolume = 1;

        private void Start()
        {
            musicOfScenes = new Dictionary<SceneType, AudioClip>()
            {
                {SceneType.MainScene, null },
                {SceneType.GameScene, null },
                {SceneType.RestartScene, null },
                {SceneType.WinScene, null }
            };
            foreach (var m in music)
            {
                musicOfScenes[m.SceneType] = m.MusicClip;
            }
        }

        public void LoadScene(SceneType scene)
        {
            switch (scene)
            {
                case SceneType.MainScene:
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
                    break;
                case SceneType.GameScene:
                    UnityEngine.SceneManagement.SceneManager.LoadScene("All_Levels(Draft 1)"); //should be GameScene 
                    break;
                case SceneType.RestartScene:
                    UnityEngine.SceneManagement.SceneManager.LoadScene("RestartScene");
                    break;
                case SceneType.WinScene:
                    UnityEngine.SceneManagement.SceneManager.LoadScene("FinalStageScene");
                    break;
                default:
                    break;
            }
            AudioManager.instance.PlayMusic(musicOfScenes[scene]);
        }

        public void ResetStartValues()
        {
            startMapID = 0;
            startLevelID = 0;
            startLevelIndex = 0;
            startPPIndex = 0;
            startFogIndex = 0;
    }

        public bool GetSave() { return saveLoaded; }
        public int GetStartMapID() { return startMapID; }
        public int GetStartLevelID() { return startLevelID; }
        public int GetStartLevelIndex() { return startLevelIndex; }
        public int GetStartFogIndex() { return startFogIndex; }
        public int GetStartPPIndex() { return startPPIndex; }
        public float GetStartMusicVolume() { return startMusicVolume; }
        public float GetStartSoundVolume() { return startSoundVolume; }
        public void SetSave(bool ifSave) { saveLoaded = ifSave; }
        public void SetStartMapID(int newMapID) { startMapID = newMapID; }
        public void SetStartLevelID(int newLevelID) { startLevelID = newLevelID; }
        public void SetStartStartLevelIndex(int newLevelIndex) { startLevelIndex = newLevelIndex; }
        public void SetStartStartFogIndex(int newFogIndex) { startFogIndex = newFogIndex; }
        public void SetStartStartPPIndex(int newPPIndex) { startPPIndex = newPPIndex; }
        public void SetStartMusicVolume(float musicVolume) { startMusicVolume = musicVolume; }
        public void SetStartSoundVolume(float soundVolume) { startSoundVolume = soundVolume; }
    }
}

