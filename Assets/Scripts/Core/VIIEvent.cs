﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VII
{
    public static class VIIEvents
    {
        public static UnityEvent TickStart = new UnityEvent();
        public static UnityEvent TickEnd = new UnityEvent();
        public static PlayerEvent LevelFinish = new PlayerEvent();
        public static PlayerEvent PlayerRespawnStart = new PlayerEvent();
        public static PlayerEvent PlayerRespawnEnd = new PlayerEvent();
    }

    [System.Serializable]
    public class PlayerEvent : UnityEvent<Player> { }
    [System.Serializable]
    public class VectorEvent : UnityEvent<Vector3> { }
}