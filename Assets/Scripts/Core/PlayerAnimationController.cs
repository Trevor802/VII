using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VII
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private Animator m_animator;

        private readonly int m_hashRespawnTrigger = Animator.StringToHash("Respawn");
        private readonly int m_hashLocomotionTag = Animator.StringToHash("Locomotion");
        private readonly int m_hashRespawningTag = Animator.StringToHash("Respawning");

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public void PlaySpawnAnimation()
        {
            m_animator.SetTrigger(m_hashRespawnTrigger);
        }

        public bool IsLocomotion
        {
            get
            {
                return m_animator.GetCurrentAnimatorStateInfo(0).tagHash == m_hashLocomotionTag;
            }
        }

        public bool IsRespawning
        {
            get
            {
                return m_animator.GetCurrentAnimatorStateInfo(0).tagHash == m_hashRespawningTag;
            }
        }
    }
}
