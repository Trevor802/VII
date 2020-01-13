using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : Tile
{
    public ByteSheep.Events.AdvancedEvent OnPlayerRespawnEndEvent;
    public int bestLifeCost = VII.GameData.PLAYER_DEFAULT_LIVES;

    private bool invokeEvents = false;

    public Animator floorAnimator;
    public Animator baseAnimator;

    public static readonly int hashIdleTag = Animator.StringToHash("Idle");

    private readonly int m_hashFallTrigger = Animator.StringToHash("Fall");
    private readonly int m_hashPressTrigger = Animator.StringToHash("Press");
    private readonly int m_hashOpenBool = Animator.StringToHash("Open");

    protected override void OnPlayerRespawnEnd(Player player)
    {
        base.OnPlayerRespawnEnd(player);
        if (playerInside && !invokeEvents)
        {
            OnPlayerRespawnEndEvent.Invoke();
            invokeEvents = true;
        }
        //text stuff
        if (player.mapIndex == 0 && player.levelIndex == 1 && player.diedInLevel1 == true)
        {
            player.makeSentence.EnableLevel1_Sentence2();
        }
        if (VII.SceneDataManager.Instance.GetCurrentLevelData().GetRespawnPoint() == this)
        {
            SetBaseAnimator(true);
            SetFloorAnimator(m_hashPressTrigger);
        }
    }

    public void SetBaseAnimator(bool i_bOpen)
    {
        if (baseAnimator.isActiveAndEnabled && baseAnimator.GetBool(m_hashOpenBool) != i_bOpen)
            baseAnimator.SetBool(m_hashOpenBool, i_bOpen);
    }

    private void SetFloorAnimator(int i_hashAnimationTrigger)
    {
        if (floorAnimator.isActiveAndEnabled)
            floorAnimator.SetTrigger(i_hashAnimationTrigger);
    }
}
