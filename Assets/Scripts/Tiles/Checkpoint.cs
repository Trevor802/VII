using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Tile
{
    public Item requiredItem;
    public ByteSheep.Events.AdvancedEvent OnPlayerEnterEvent;
    private bool activated = false;

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (player.PlayerData.Inventory.ContainItem(requiredItem) && !activated)
        {
            if (player.GetRespawnPosIndex() == 7)
            {
                player.FinishLevel7 = true;
            }
            // Reset respawn position and respawn player
            activated = true;
            VII.VIIEvents.LevelFinish.Invoke(player);
            player.PlayerData.Inventory.RemoveItem(requiredItem);
            player.SetRespawnPosition(1);
            OnPlayerEnterEvent.Invoke();
            player.Respawn(false);
        }
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
    }

    public void Win()
    {
        UIManager.UIInstance.gameObject.SetActive(false);
        VII.SceneManager.instance.LoadScene(VII.SceneType.WinScene);
    }
}
