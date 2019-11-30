using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject model;

    private Vector3 m_InitPos;

    private void Awake()
    {
        m_InitPos = transform.position;
    }

    public void Move(Vector3 i_Direction)
    {
        model.transform.position += i_Direction;
    }

    public void ResetPosition()
    {
        model.transform.position = m_InitPos;
    }
}
