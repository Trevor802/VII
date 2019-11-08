using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Tile
{
    [SerializeField]
    private bool m_spikeUp;
    public GameObject model;


    protected override void Awake()
    {
        base.Awake();
    }

    private void OnValidate()
    {
        UpdateSpike();
    }

    private void UpdateSpike()
    {
        if (m_spikeUp)
        {
            model.transform.position = transform.position;
        }
        else
        {
            model.transform.position = transform.position -
                new Vector3(0, VII.GameData.STEP_SIZE / 2, 0);
        }
    }

    protected override void OnTickEnd()
    {
        m_spikeUp = !m_spikeUp;
        UpdateSpike();
        base.OnTickEnd();
    }

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (m_spikeUp)
        {
            player.Respawn();
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
    }
}
