using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalRotating : MonoBehaviour
{
    public float rotateSpeed = 100;
    private bool hasCrystal = false;
    public GameObject crystalModel;
    // Update is called once per frame
    private void Awake()
    {
        DeactivateCrystal();
    }
    void Update()
    {
        if(hasCrystal)
        {
            this.transform.Rotate(this.transform.up, rotateSpeed * Time.deltaTime);
        }
    }

    public void ActivateCrystal()
    {
        crystalModel.GetComponent<Renderer>().enabled = true;
        hasCrystal = true;
    }

    public void DeactivateCrystal()
    {
        crystalModel.GetComponent<Renderer>().enabled = false;
        hasCrystal = false;
    }
}
