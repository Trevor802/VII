using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VII
{
    public static class VIIEvents
    {
        public static UnityEvent TickStart = new UnityEvent();
        public static UnityEvent TickEnd = new UnityEvent();
        public static TargetTriggerEvent LevelFinish = new TargetTriggerEvent();
        public static PlayerEvent PlayerRespawnStart = new PlayerEvent();
        public static PlayerEvent PlayerRespawnEnd = new PlayerEvent();
        public static PlayerEvent PlayerRegisterEnd = new PlayerEvent();
        public static LanguageEvent LanguageSwitch = new LanguageEvent();
    }

    [System.Serializable]
    public class PlayerEvent : UnityEvent<Player> { }
    [System.Serializable]
    public class VectorEvent : UnityEvent<Vector3> { }
    [System.Serializable]
    public class TargetTriggerEvent : UnityEvent<GameObject, Player> { }
    [System.Serializable]
    public class LanguageEvent : UnityEvent<VII.Language> { }
}