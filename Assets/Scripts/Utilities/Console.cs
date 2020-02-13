using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VII
{
    public class Console : MonoBehaviour
    {
#if DEBUG
        public Text OutputText;

        private void Awake()
        {
            VIIEvents.PlayerRespawnEnd.AddListener(Output);
            VIIEvents.PlayerRegisterEnd.AddListener(Output);
        }

        private void Output(Player i_Player)
        {
            UpdateUI(GetLevelInfo());
        }

        private string GetLevelInfo()
        {
            var mapID = SceneDataManager.Instance.GetCurrentMapData().GetMapID();
            var levelID = SceneDataManager.Instance.GetCurrentLevelData().GetLevelID();
            return "map-" + mapID + " level-" + levelID;
        }

        private void UpdateUI(string i_str)
        {
            OutputText.text = i_str;
        }

#endif
    }
}