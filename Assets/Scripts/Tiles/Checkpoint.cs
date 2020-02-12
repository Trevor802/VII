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

    public static readonly int hashActivatedTag = Animator.StringToHash("Activated");

    private readonly int m_hashFallTrigger = Animator.StringToHash("Fall");
    private readonly int m_hashPressTrigger = Animator.StringToHash("Press");
    private readonly int m_hashOpenBool = Animator.StringToHash("Open");

    private float m_timeWin;

    protected override void OnPlayerEnter(Player player)
    {
        base.OnPlayerEnter(player);
        if (player.PlayerData.Inventory.ContainItem(requiredItem) && !activated)
        {
            //Achievement Data
            if (player.mapIndex == 3 && player.levelIndex == 1)
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
            if (player.mapIndex == 13 && player.levelIndex == 0)
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
            VII.SceneDataManager.Instance.GetCurrentLevelData().GetRespawnPoint().SetBaseAnimator(false);
            GetPreviousCheckpoint()?.baseAnimator.SetBool(m_hashOpenBool, false);
            VII.VIIEvents.LevelFinish.Invoke(gameObject, player);
            player.PlayerData.Inventory.RemoveItem(requiredItem);
            player.SetRespawnPoint(1);
            baseAnimator.SetBool(m_hashOpenBool, true);
            bool gameEnd = VII.SceneDataManager.Instance.GetCurrentMapData().GetMapID() >=
                VII.SceneDataManager.Instance.GetMapData().Count - 1;
            if (!gameEnd)
                VII.SceneDataManager.Instance.GetCurrentMapData().GetMapObject().SetActive(true);
            bool willFall = (VII.SceneDataManager.Instance.GetCurrentLevelData().GetLevelID() ==
                VII.SceneDataManager.Instance.GetCurrentMapData().GetLevelData().Count - 1);
            if (willFall)
            {
                AudioManager.instance.PlaySingle(AudioManager.instance.respawn);
                floorAnimator.SetTrigger(m_hashFallTrigger);
            }
            if (!gameEnd)
                player.Respawn(false, willFall);
            else
            {
                player.PlayerData.playerState = VII.PlayerState.ENDING;
                StartCoroutine(WaitUntilAnimation(hashActivatedTag));
            }
                
        }
    }

    public IEnumerator WaitUntilAnimation(int i_hashAnimationTag)
    {
        yield return new WaitWhile(() => baseAnimator.GetCurrentAnimatorStateInfo(0).tagHash == i_hashAnimationTag);
        while(baseAnimator.GetCurrentAnimatorStateInfo(0).tagHash != i_hashAnimationTag)
            yield return null;
        OnPlayerEnterEvent.Invoke();
    }

    private Checkpoint GetPreviousCheckpoint()
    {
        int levelID = VII.SceneDataManager.Instance.GetCurrentLevelData().GetLevelID();
        if (levelID > 0)
            return VII.SceneDataManager.Instance.GetCurrentMapData().GetLevelData()[levelID - 1].GetCheckpoint();
        return null;
    }

    protected override void OnPlayerExit(Player player)
    {
        base.OnPlayerExit(player);
        if (activated)
            OnPlayerExitEvent.Invoke();
    }

    protected override void OnPlayerRespawnEnd(Player player)
    {
        base.OnPlayerRespawnEnd(player);
        GetPreviousCheckpoint()?.floorAnimator.SetTrigger(m_hashPressTrigger);
    }

    public void Win()
    {
        StartCoroutine(Winning());
    }

    public IEnumerator Winning()
    {
        while (m_timeWin < 1.0f)
        {
            Player.Instance.transform.position += new Vector3(0, -Time.deltaTime * Player.Instance.fallingSpeed, 0);
            m_timeWin += Time.deltaTime;
            yield return null;
        }
        UIManager.UIInstance.gameObject.SetActive(false);
        VII.SceneManager.instance.LoadScene(VII.SceneType.WinScene);
    }
}
