using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTool : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Player.Instance.SetRespawnPoint(1);
            CameraManager.Instance.SwitchLevelCamera(1);
            Player.Instance.Respawn(false);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Player.Instance.SetRespawnPoint(-1);
            CameraManager.Instance.SwitchLevelCamera(-1);
            Player.Instance.Respawn(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log(VII.SceneDataManager.Instance.GetCurrentTileData().name);
    }
}
