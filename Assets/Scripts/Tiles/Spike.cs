using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEditor;[ExecuteAlways]public class Spike : Tile{    [SerializeField]    private bool m_spikeUp;    public GameObject model;    private Animator m_animator;    protected override void Awake()    {        base.Awake();

        #region Presentation Layer        m_animator = model.GetComponent<Animator>();
        #endregion    }

    #region Editor Mode    private void OnEnable()
    {
        if (!m_animator) m_animator = model.GetComponent<Animator>();
        EditorApplication.update += EditorUpdate;
    }    private void OnDisable()
    {
        if (!m_animator) m_animator = model.GetComponent<Animator>();
        EditorApplication.update -= EditorUpdate;
    }    private void OnDestroy()
    {
        if (!m_animator) m_animator = model.GetComponent<Animator>();
        EditorApplication.update -= EditorUpdate;
    }    private void EditorUpdate()
    {
        if (gameObject.activeSelf)
            m_animator.Update(Time.deltaTime);
    }    private void OnValidate()    {        UpdateSpike();    }
    #endregion    private void UpdateSpike()    {
        #region Presentation Layer
        if (!m_animator) m_animator = model.GetComponent<Animator>();
        if (m_animator.isActiveAndEnabled)
            m_animator.SetBool("SpikeUp", m_spikeUp);
        #endregion    }    protected override void OnTickStart()    {        base.OnTickStart();        m_spikeUp = !m_spikeUp;        UpdateSpike();    }    protected override void OnTickEnd()    {        if (playerOutTemp)        {            playerInside = false;            playerOutTemp = false;        }        if (playerInTemp)        {            playerInside = true;            playerInTemp = false;        }    }    private void OnTriggerEnter(Collider other)    {        collidedPlayer = other.GetComponentInParent<Player>();        if (collidedPlayer && !playerInside)        {            playerInTemp = true;            playerOutTemp = false;            OnPlayerEnter(collidedPlayer);        }    }    private void OnTriggerExit(Collider other)    {        if (collidedPlayer && collidedPlayer == other.GetComponentInParent<Player>())        {            playerOutTemp = true;            playerInTemp = false;            OnPlayerExit(collidedPlayer);        }    }    protected override void OnPlayerEnter(Player player)    {        base.OnPlayerEnter(player);        if (m_spikeUp)        {            player.Respawn();        }    }    protected override void OnPlayerExit(Player player)    {        base.OnPlayerExit(player);    }}