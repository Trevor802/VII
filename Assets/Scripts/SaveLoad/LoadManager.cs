using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;
    void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else if(instance!=this)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Data
{
    public Vector3 respawnPosition;
    public int steps;
    public int lives;

    public Data()
    {
        //lives = i_initLifes;
        //steps = i_initSteps;
        //respawnPosition = i_initRespawnPosition;
    }

}

