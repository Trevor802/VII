using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VII
{
    public enum PlayerAnimationState
    {
        Idling, Moving, Death, Respawning, Transiting, Sliding, Reviving
    }

    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator m_animator;

        // Trigger hash
        private readonly int m_hashMoveTrigger = Animator.StringToHash("Move");
        private readonly int m_hashIdleTrigger = Animator.StringToHash("Idle");
        private readonly int m_hashDeadTrigger = Animator.StringToHash("Dead");
        private readonly int m_hashReviveTrigger = Animator.StringToHash("Revive");
        private readonly int m_hashFallTrigger = Animator.StringToHash("Fall");

        // State hash
        private readonly int m_hashMovingTag = Animator.StringToHash("Moving");
        private readonly int m_hashIdlingTag = Animator.StringToHash("Idling");
        private readonly int m_hashRespawningTag = Animator.StringToHash("Respawning");
        private readonly int m_hashDeathTag = Animator.StringToHash("Death");
        private readonly int m_hashSlidingTag = Animator.StringToHash("Sliding");

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public void TriggerAnimation(PlayerAnimationState i_eState)
        {
            switch (i_eState)
            {
                case PlayerAnimationState.Idling:
                    m_animator.SetTrigger(m_hashIdleTrigger);
                    break;
                case PlayerAnimationState.Moving:
                    RotateModel(Player.Instance.GetMoveDirection());
                    m_animator.SetTrigger(m_hashMoveTrigger);
                    break;
                case PlayerAnimationState.Death:
                    m_animator.SetTrigger(m_hashDeadTrigger);
                    break;
                case PlayerAnimationState.Reviving:
                    m_animator.SetTrigger(m_hashReviveTrigger);
                    break;
                case PlayerAnimationState.Respawning:
                    m_animator.SetTrigger(m_hashFallTrigger);
                    break;
                default:
                    break;
            }
        }

        public PlayerAnimationState GetAnimationState()
        {
            int currentStateTagHash = m_animator.GetCurrentAnimatorStateInfo(0).tagHash;
            if (currentStateTagHash == m_hashIdlingTag)
            {
                return PlayerAnimationState.Idling;
            }
            if (currentStateTagHash == m_hashMovingTag)
            {
                return PlayerAnimationState.Moving;
            }
            if (currentStateTagHash == m_hashDeathTag)
            {
                return PlayerAnimationState.Death;
            }
            if (currentStateTagHash == m_hashRespawningTag)
            {
                return PlayerAnimationState.Respawning;
            }
            if (currentStateTagHash == m_hashSlidingTag)
            {
                return PlayerAnimationState.Sliding;
            }
                return PlayerAnimationState.Transiting;
        }

        public void TriggerSlidingAnimation(bool isSliding)
        {
            m_animator.SetBool("IsSliding", isSliding);
        }

        public void PlayStepSound()
        {
            if (!(AudioManager.instance.soundSource.isPlaying && AudioManager.instance.soundSource.clip.name == "slide"))
                AudioManager.instance.PlaySingle(AudioManager.instance.footStep);
        }

        public void RotateModel(Vector3 i_Direction)
        {
            m_animator.transform.rotation = Quaternion.LookRotation(i_Direction, Vector3.up);
        }
    }
}
