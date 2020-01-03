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
        public Inventory Inventory;
        public PlayerState playerState;
        public int respawnPositionIndex;
        public int steps;
        public int lives;

        public PlayerData(int i_initLifes, int i_initSteps)
        {
            lives = i_initLifes;
            steps = i_initSteps;
            Inventory = new Inventory();
        }
    }
}

