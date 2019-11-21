using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public GameObject ground_detector;
    private float m_maxCastDistance = 1f;
    void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            LavaSpread();
    }

    void LavaSpread()
    {
        if (CheckAbutTile(transform.forward))
            Debug.Log("Forward hit");
        if (CheckAbutTile(-transform.forward))
            Debug.Log("Backward hit");
        if (CheckAbutTile(transform.right))
            Debug.Log("Right hit");
        if (CheckAbutTile(-transform.right))
            Debug.Log("Left hit");
    }

    bool CheckAbutTile(Vector3 i_dir)
    {
        RaycastHit hit;
        bool hitResult;
        hitResult = Physics.Raycast(ground_detector.transform.position, i_dir, out hit, m_maxCastDistance);
        if(hitResult)
        {
            //Debug.Log(hit.transform.name);
            return true;
        }
        else
        {
            return false;
        }
    }
}
