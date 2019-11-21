using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : Tile
{
    public GameObject ground_detector;
    public GameObject floor_tile;
    public GameObject lava_tile;
    private int ice_layer = 9;
    private int default_layer = 0;
    private float m_maxCastDistance = 1f;
    void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            LavaSpread();
    }

    protected override void OnTickEnd()
    {
        base.OnTickEnd();
        LavaSpread();
    }

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        player.Respawn();
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
            //Debug.Log(hit.transform.gameObject.layer);
            GameObject hit_tile = hit.transform.gameObject;
            if(hit_tile.layer == ice_layer)
            {
                Instantiate(floor_tile, hit.transform.position, hit.transform.rotation);
                Destroy(hit.transform.gameObject);
            }
            else if(hit_tile.layer == default_layer)
            {
                Floor hit_floor = hit_tile.GetComponent<Floor>();
                if(hit_floor && hit_floor.GetFloorState() == Floor.FloorState.DOWN)
                {
                    Instantiate(lava_tile, hit.transform.position, hit.transform.rotation);
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
