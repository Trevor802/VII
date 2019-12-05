using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : Tile
{
    public GameObject wall_detector;
    public GameObject ground_detector;
    public GameObject floor_tile;
    public GameObject lava_tile;
    [SerializeField] private int m_Life;
    [SerializeField] private bool b_DestroyedInFuture;
    private int ice_layer = 9;
    private int default_layer = 0;
    private float m_maxCastDistance = 1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            LavaSpread();
    }

    protected override void OnTickEnd()
    {
        base.OnTickEnd();
        if(b_DestroyedInFuture)
        {
            m_Life--;
            if (m_Life <= 0)
                Destroy(this.gameObject);
            Debug.Log("Lava life: " + m_Life);
        }
        LavaSpread();
    }

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        player.Respawn();
    }

    void LavaSpread()
    {
        CheckAbutTile(transform.forward);
        CheckAbutTile(-transform.forward);
        CheckAbutTile(transform.right);
        CheckAbutTile(-transform.right);
    }

    bool CheckAbutTile(Vector3 i_dir)
    {
        RaycastHit wallHit;
        RaycastHit groundHit;
        bool wallHitResult;
        bool groundHitResult;
        
        wallHitResult = Physics.Raycast(ground_detector.transform.position, i_dir, out wallHit, m_maxCastDistance, (int)VII.HitLayer.Block);
        if(wallHitResult)
        {
            return false;
        }

        groundHitResult = Physics.Raycast(ground_detector.transform.position, i_dir, out groundHit, m_maxCastDistance);
        if(groundHitResult)
        {
            //Debug.Log(hit.transform.gameObject.layer);
            GameObject hit_tile = groundHit.transform.gameObject;
            //Is the abut tile ice? - turn it into normal floor tile
            if(hit_tile.layer == ice_layer)
            {
                Instantiate(floor_tile, groundHit.transform.position, groundHit.transform.rotation);
                Destroy(groundHit.transform.gameObject);
            }
            //Is the abut tile floor? - fill it with lava 
            else if(hit_tile.layer == default_layer)
            {
                Floor hit_floor = hit_tile.GetComponent<Floor>();
                if(hit_floor && hit_floor.GetFloorState() == Floor.FloorState.DOWN)
                {
                    GameObject lava_instant = Instantiate(lava_tile, groundHit.transform.position, groundHit.transform.rotation) as GameObject;
                    if (hit_floor.declineAfterExit)
                    {
                        if(lava_instant.GetComponent<Lava>())
                        {
                            
                            lava_instant.GetComponent<Lava>().m_Life = hit_floor.stepsBeforeIncline - 1;    
                            lava_instant.GetComponent<Lava>().b_DestroyedInFuture = true;
                            Debug.Log(lava_instant.GetComponent<Lava>().m_Life);
                            Debug.Log(lava_instant.GetComponent<Lava>().b_DestroyedInFuture);
                        }
                    }
                        
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
