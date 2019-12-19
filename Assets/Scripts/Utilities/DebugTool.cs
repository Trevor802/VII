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
            CameraManager.Instance.SwitchLevelCamera(1);
            Player.Instance.SetRespawnPosition(1);
            Player.Instance.Respawn(false);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CameraManager.Instance.SwitchLevelCamera(-1);
            Player.Instance.SetRespawnPosition(-1);
            Player.Instance.Respawn(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log(VII.SceneDataManager.Instance.GetCurrentTileData().name);
    }
}
