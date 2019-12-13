using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathTipNode : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] m_NodeLines = new SpriteRenderer[4];
    [SerializeField]
    private GameObject m_Root;

    private void Awake()
    {
        foreach (var line in m_NodeLines)
        {
            line.enabled = false;
        }
    }

    private void OnEnable()
    {
        UpdateLineVisibility();
    }

    private void UpdateLineVisibility()
    {
        if ((int)VII.HitLayer.Unreachable == ((int)VII.HitLayer.Unreachable | (1 << m_Root.layer)))
        {
            gameObject.SetActive(false);
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            RaycastHit hitResult;
            bool showLine;
            showLine = Physics.Raycast(m_Root.transform.position,
                Quaternion.AngleAxis(90 * i, Vector3.up) * m_Root.transform.forward, out hitResult,
                VII.GameData.STEP_SIZE * 1.1f, (int)(VII.HitLayer.Default | VII.HitLayer.Ice | VII.HitLayer.Unreachable));
            if (showLine && (int)VII.HitLayer.Unreachable == ((int)VII.HitLayer.Unreachable | (1 << hitResult.collider.gameObject.layer)))
                showLine = false;
            m_NodeLines[i].enabled |= showLine;
        }
    }
}
