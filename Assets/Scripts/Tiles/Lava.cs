using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VII;

public class Lava : Tile
{
    public GameObject wall_detector;
    public GameObject ground_detector;
    public GameObject floor_tile;
    public GameObject lava_tile;
    public GameObject lava_block;
    public int m_Life;
    public bool b_DestroyedInFuture;
    private int ice_layer = 9;
    private int default_layer = 0;
    private float m_maxCastDistance = 1f;
    private GameObject currentFloor;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            LavaSpread();
    }

    protected override void OnTickStart()
    {
        base.OnTickStart();
        
        
    }

    protected override void OnTickEnd()
    {
        base.OnTickEnd();
        if (!receiveTick) { return; }
        if (b_DestroyedInFuture)
        {
            m_Life--;
            if (m_Life <= 0)
                Destroy(this.gameObject);
            //Debug.Log("Lava life: " + m_Life);
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
            //Debug.Log("From "+ this.name +": " +groundHit.transform.gameObject.name);
            GameObject hit_tile = groundHit.transform.gameObject;
            //Is the abut tile ice? - turn it into normal floor tile
            if (hit_tile.layer == ice_layer)
            {
                //Debug.Log("Melt ice");
                GameObject newFloor = Instantiate(floor_tile, groundHit.transform.parent);
                newFloor.transform.position = groundHit.transform.position;
                newFloor.transform.rotation = groundHit.transform.rotation;
                Destroy(groundHit.transform.gameObject);
            }
            //Is the abut tile unreachable floor? - fill it with lava 
            else if(hit_tile.layer == 12)
            {
                Floor hit_floor = hit_tile.GetComponent<Floor>();
                //Debug.Log(!hit_tile.GetComponent<Lava>());
                if(hit_floor 
                    && hit_floor.GetFloorState() == Floor.FloorState.DOWN 
                    && hit_floor.GetLavaFlowState() 
                    && !hit_tile.GetComponent<Lava>())
                {
                    //Debug.Log(hit_floor.name + ": Set floor state to allow lava");
                    //if(!hit_floor.GetLavaFlowState())
                    //{
                    //    hit_floor.SetLavaFlowState();
                    //}
                    currentFloor = hit_tile;
                    hit_floor.GetComponent<BoxCollider>().enabled = false;
                    GameObject lava_instance = Instantiate(lava_tile, groundHit.transform.parent) as GameObject;
                    AudioManager.instance.PlaySingle(AudioManager.instance.lavaSpread);
                    Debug.Log("???");
                    lava_instance.transform.position = groundHit.transform.position;
                    lava_instance.transform.rotation = groundHit.transform.rotation;
                    //Debug.Log(i_dir);
                    if(i_dir == new Vector3(-1,0,0))
                    {
                        //Debug.Log("180");
                        //lava_instance.GetComponent<Lava>().ChangeSpreadDirection(180);
                        lava_instance.transform.eulerAngles = new Vector3(0, 180, 0);
                    }
                    else if (i_dir == new Vector3(0, 0, 1))
                    {
                        //Debug.Log("-90");
                        lava_instance.transform.eulerAngles = new Vector3(0, -90, 0);
                    }
                    else if (i_dir == new Vector3(0, 0, -1))
                    {
                        //Debug.Log("90");
                        lava_instance.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    //Debug.Break();
                    lava_instance.GetComponent<Lava>().b_DestroyedInFuture = false;
                    lava_instance.GetComponent<Lava>().SetReceiveTick(true);
                    if (hit_floor.declineAfterExit)
                    {
                        if (lava_instance.GetComponent<Lava>())
                        {

                            lava_instance.GetComponent<Lava>().m_Life = hit_floor.stepsBeforeIncline - 1;
                            lava_instance.GetComponent<Lava>().b_DestroyedInFuture = true;
                            //Debug.Log(i_dir);
                            //Debug.Log(lava_instance.transform.name + lava_instance.GetComponent<Lava>().m_Life);
                            //Debug.Log(lava_instance.GetComponent<Lava>().b_DestroyedInFuture);
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

    public void ChangeSpreadDirection(float i_eulerAngleY)
    {
        lava_block.transform.eulerAngles = new Vector3(0, i_eulerAngleY, 0);
    }

    private void OnDestroy()
    {
        if (currentFloor)
            currentFloor.GetComponent<BoxCollider>().enabled = true;
    }
}
