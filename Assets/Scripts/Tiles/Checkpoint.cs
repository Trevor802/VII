using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : Tile
{
    public Item requiredItem;
    public ByteSheep.Events.AdvancedEvent OnPlayerEnterEvent;
    public ByteSheep.Events.AdvancedEvent OnPlayerExitEvent;
    public bool activated = false;
    public Animator floorAnimator;
    public Animator baseAnimator;

    public static readonly int hashIdleTag = Animator.StringToHash("Idle");

    private readonly int m_hashFallTrigger = Animator.StringToHash("Fall");
    private readonly int m_hashPressTrigger = Animator.StringToHash("Press");

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
            if (player.mapIndex == 3 && player.levelIndex == 1)
            {
                player.completeDungeon = true;
            }
            if (player.mapIndex == 8 && player.levelIndex == 1)
            {
                player.completeIce = true;
            }
            if (player.mapIndex == 12 && player.levelIndex == 0)
            {
                player.completeLava = true;
                player.summonGreatOne = true;
            }
            //Text stuff
            if (player.mapIndex == 0 && player.levelIndex == 0)
            {
                player.displayLevel0Text = true;
            }
            if(player.mapIndex == 13 && player.levelIndex == 0)
            {
                player.makeSentence.EnableFinalLevel_Sentence1();
                player.winCountDown = true;
            }

            player.checkLeastLives = true;
            // Reset respawn position and respawn player
            activated = true;
            VII.VIIEvents.LevelFinish.Invoke(gameObject, player);
            player.PlayerData.Inventory.RemoveItem(requiredItem);
            // Set respoint point after death animation. Thus it's moved to Respawn
            // function in Player.cs
            player.SetRespawnPoint(1);
            baseAnimator.SetTrigger(m_hashPressTrigger);
            VII.SceneDataManager.Instance.GetCurrentMapData().GetMapObject().SetActive(true);
            bool willFall = VII.SceneDataManager.Instance.GetCurrentLevelData().GetLevelID() ==
                VII.SceneDataManager.Instance.GetCurrentMapData().GetLevelData().Count - 1;
            if (willFall)
            {
                AudioManager.instance.PlaySingle(AudioManager.instance.respawn);
                floorAnimator.SetTrigger(m_hashFallTrigger);
            }
            else
            {
                floorAnimator.SetTrigger(m_hashPressTrigger);
            }
            player.Respawn(false, willFall);
        }
    }

    public IEnumerator WaitUntilAnimation(int i_hashAnimationTag)
    {
        yield return new WaitWhile(() => floorAnimator.GetCurrentAnimatorStateInfo(0).tagHash == i_hashAnimationTag);
        while(floorAnimator.GetCurrentAnimatorStateInfo(0).tagHash != i_hashAnimationTag)
            yield return null;
        OnPlayerEnterEvent.Invoke();
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
        if (activated)
            OnPlayerExitEvent.Invoke();
    }

    public void Win(Player player)
    {
        if(player.enterWinScene == true)
        {
            UIManager.UIInstance.gameObject.SetActive(false);
            VII.SceneManager.instance.LoadScene(VII.SceneType.WinScene);
        }
    }
}
