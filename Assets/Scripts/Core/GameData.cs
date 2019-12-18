using System;
using UnityEngine;

namespace VII
{
    public static class GameData
    {
        public const float STEP_SIZE = 1f;
        public const float WALL_WIDTH = STEP_SIZE * 0.1f;
        public const float EQUAL_DEVIATION = STEP_SIZE * 0.25f;
        public static readonly Vector3 PLAYER_RESPAWN_POSITION_OFFSET = new Vector3(0, STEP_SIZE * 0.5f, 0);
    }
}
