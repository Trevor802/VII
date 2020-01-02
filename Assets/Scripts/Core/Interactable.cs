using UnityEngine;

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
                if(player.mapIndex == 3 && player.levelIndex == 1)
                {
                    player.HasKeyInLevel8 = true;
                }
                player.PlayerData.Inventory.AddItem(item);
                // Destroy gameobject
                Destroy(gameObject);
            }
        }
    }
}