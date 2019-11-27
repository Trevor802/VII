using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Wall : MonoBehaviour
{
    private Collider m_Collider;
    private Vector3 m_InitPos;

    private void Awake()
    {
        m_Collider = GetComponent<Collider>();
        m_InitPos = transform.position;
    }

    public void Move(Vector3 i_Direction)
    {
        transform.position += i_Direction;
    }

    public void ResetPosition()
    {
        transform.position = m_InitPos;
    }
}
