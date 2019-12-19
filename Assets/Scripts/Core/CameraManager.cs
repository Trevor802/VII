﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public List<GameObject> cinema_list;
    public bool debugMode;  //in debug mode, the player will transfer with the camera to next or prev level.
    public int level_index;
    private Player player;

    #region Singleton
    public static CameraManager Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    void Start()
    {
        GameObject[] all_Virtual_Cameras = GameObject.FindGameObjectsWithTag("VirtualCamera");
        foreach(var vc in all_Virtual_Cameras)
        {
            vc.SetActive(false);
        }

        level_index = UIManager.UIInstance.startLevelIndex;
        for (int i = 0; i < cinema_list.Count; i++)
        {
            if (i < level_index)
            {
                cinema_list[i].transform.parent.gameObject.SetActive(true);
                cinema_list[i].SetActive(false);
            }
            else if (i == level_index)
            {
                cinema_list[i].transform.parent.gameObject.SetActive(true);
                cinema_list[i].SetActive(true);
            }
            else
            {
                cinema_list[i].SetActive(false);
            }
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player == null)
            Debug.LogError("CameraManager: Player not found!");
    }

    public void SwitchLevelCamera(int index)
    {
        level_index += index;
        if (level_index < 0)
        {
            level_index += cinema_list.Count;
        }
        level_index %= cinema_list.Count;
        cinema_list[level_index].transform.parent.gameObject.SetActive(true);
        cinema_list.ForEach(cam => cam.SetActive(false));
        cinema_list[level_index].SetActive(true);
        if(debugMode)
        {
            //Finish here when respawn points are done
            //Vector3 next_checkpoint = cinema_list[level_index].GetComponent<LevelData>().level_checkpoint.transform.position;
            //player.ResetRespawnPos(next_checkpoint);
            //player.Respawn(false);
        }
        
    }
}


