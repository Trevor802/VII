using System;
using UnityEngine;

namespace VII
{
    public enum PlayerState
    {
        IDLE = 0,
        MOVING = 1,
        RESPAWNING = 2,
        ENDING = 3
    }

    public class PlayerData
    {
        private bool m_hasKey;
        public PlayerState playerState;
        public Vector3 respawnPosition;
        public int steps;
        public int lives;

        public PlayerData(int i_initLifes, int i_initSteps, Vector3 i_initRespawnPosition) =>
            (lives, steps, respawnPosition) =
            (i_initLifes, i_initSteps, i_initRespawnPosition);

        #region Setter
        public bool HasKey() { return true; }
        #endregion
    }
}

