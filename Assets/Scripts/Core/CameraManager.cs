using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager : MonoBehaviour
{
    public List<GameObject> cinema_list;
    public List<GameObject> pp_list;
    public List<GameObject> fog_list;
    [HideInInspector]
    public int level_index;
    [HideInInspector]
    public int pp_index;
    private int prevppIndex;
    private bool ppSwitching = false;
    [HideInInspector]
    public int fog_index;
    private ParticleSystem fogParticle;
    private Player player;
    public float ppSwitchSpeed = 0.02f;

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
        pp_index = UIManager.UIInstance.startPPIndex;
        fog_index = UIManager.UIInstance.startFogIndex;
        for (int i = 0; i < cinema_list.Count; i++)
        {
            if (i == level_index)
            {
                cinema_list[i].SetActive(true);
            }
            else
            {
                cinema_list[i].SetActive(false);
            }
        }
        StartCoroutine(InitPostProcessing());
        for (int i = 0; i < fog_list.Count; i++)
        {
            if (i == fog_index)
            {
                fog_list[i].SetActive(true);
                fog_list[i].GetComponent<ParticleSystem>().Play();
            }
            else
            {
                fog_list[i].SetActive(false);
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
        //cinema_list[level_index].transform.parent.gameObject.SetActive(true);
        cinema_list.ForEach(cam => cam.SetActive(false));
        cinema_list[level_index].SetActive(true);
    }

    public void SwitchFog(int index)
    {
        if (fog_index != index)
        {
            fog_index = index;
            fog_list.ForEach(fog => fog.SetActive(false));
            fog_list[fog_index].SetActive(true);
            fog_list[fog_index].GetComponent<ParticleSystem>().Play();
        }
    }

    private IEnumerator InitPostProcessing()
    {
        while (pp_list[pp_index].GetComponent<PostProcessVolume>().weight < 1)
        {
            pp_list[pp_index].GetComponent<PostProcessVolume>().weight += ppSwitchSpeed;
            yield return null;
        }
        pp_list[pp_index].GetComponent<PostProcessVolume>().weight = 1;
    }

    private IEnumerator SwitchPP()
    {
        ppSwitching = true;
        while (pp_list[prevppIndex].GetComponent<PostProcessVolume>().weight > 0)
        {
            pp_list[prevppIndex].GetComponent<PostProcessVolume>().weight -= ppSwitchSpeed;
            pp_list[pp_index].GetComponent<PostProcessVolume>().weight += ppSwitchSpeed;
            yield return null;
        }
        pp_list[prevppIndex].GetComponent<PostProcessVolume>().weight = 0;
        pp_list[pp_index].GetComponent<PostProcessVolume>().weight = 1;
        ppSwitching = false;
    }

    public void SwitchPostProcessing(int new_index)
    {
        if (!ppSwitching)
        {
            prevppIndex = pp_index;
            pp_index = new_index;
            if (prevppIndex != pp_index)
            {
                StartCoroutine(SwitchPP());
            }
        }
    }

    public void SetFogYPosition(float yCoord)
    {
        fog_list[fog_index].transform.position = new Vector3(fog_list[fog_index].transform.position.x,
            yCoord, fog_list[fog_index].transform.position.z);
    }
}




