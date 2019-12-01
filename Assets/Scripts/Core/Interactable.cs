﻿using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public Item item;

    private Collider m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider i_Other)
    {
        Player player = i_Other.GetComponentInParent<Player>();
        if (player)
        {
            if (!player.PlayerData.Inventory.ContainItem(item) || item.stackable)
            {
                player.PlayerData.Inventory.AddItem(item);
                // Destroy gameobject
                Destroy(gameObject);
            }
        }
    }
}