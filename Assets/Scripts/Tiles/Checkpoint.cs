using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Tile
{
    public Item requiredItem;
    public ByteSheep.Events.AdvancedEvent OnPlayerEnterEvent;
    public bool activated = false;

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (player.PlayerData.Inventory.ContainItem(requiredItem) && !activated)
        {
            //Achievement Data
            if (player.mapIndex == 0 && player.levelIndex == 8)
            {
                player.FinishLevel7 = true;
            }
            if (player.mapIndex == 0 && player.levelIndex == 9)
            {
                player.completeDungeon = true;
            }
            if (player.mapIndex == 1 && player.levelIndex == 8)
            {
                player.completeIce = true;
            }
            if (player.mapIndex == 2 && player.levelIndex == 6)
            {
                player.completeLava = true;
            }
            player.checkLeastLives = true;
            //Transition Texts Data
            if(player.mapIndex == 0 && player.levelIndex == 5)
            {
                player.display_text_trap = true;
            }
            if (player.mapIndex == 0 && player.levelIndex == 9)
            {
                player.display_text_ice = true;
            }
            if (player.mapIndex == 1 && player.levelIndex == 8)
            {
                player.display_text_lava = true;
            }
            // Reset respawn position and respawn player
            activated = true;
            VII.VIIEvents.LevelFinish.Invoke(gameObject, player);
            player.PlayerData.Inventory.RemoveItem(requiredItem);
            player.SetRespawnPoint(1);
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
