using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager UIInstance = null;

    private void Awake()
    {
        if (UIInstance == null)
        {
            UIInstance = this;
            InitUI();
        }
        else if (UIInstance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
    #endregion

    public GameObject lifeIcon;
    public RectTransform lifeIconHolder;
    public int maxLives = 10;
    public Text levelIndexText;

    private List<RawImage> m_lifeIcons;
    private List<RawImage> m_crossIcons;

    public int startLevelIndex = 0;
    public int startRespawnIndex = 0;
    public int startLives = 7;

    public void UpdateUI()
    {
        //Player player = FindObjectOfType<Player>();
        //VII.PlayerData playerData = player.GetComponent<Player>().PlayerData;
        for (int i = 0; i < Player.Instance.initLives; i++)
        {
            m_lifeIcons[i].enabled = true;
            m_crossIcons[i].enabled = i >= Player.Instance.GetLives();
        }
        for (int i = Player.Instance.initLives; i < maxLives; i++)
        {
            m_crossIcons[i].enabled = false;
            m_lifeIcons[i].enabled = false;
        }
        levelIndexText.text = "level_" + Player.Instance.GetRespawnPosIndex();
    }

    public void ClearUI()
    {
        m_lifeIcons.ForEach(x => x.enabled = false);
        m_crossIcons.ForEach(x => x.enabled = false);
        levelIndexText.enabled = false;
    }

    public void InitUI()
    {
        m_lifeIcons = new List<RawImage>();
        m_crossIcons = new List<RawImage>();

        //for (int i = 0; i < Player.Instance.initLives; i++)
        for (int i = 0; i < maxLives; i++)
        {
            var life = Instantiate(lifeIcon, lifeIconHolder);
            m_lifeIcons.Add(life.GetComponent<RawImage>());
            m_crossIcons.Add(life.transform.GetChild(0).GetComponent<RawImage>());
        }
        m_lifeIcons.ForEach(x => x.enabled = true);
        m_crossIcons.ForEach(x => x.enabled = true);
        levelIndexText.text = (Player.Instance.GetRespawnPosIndex() + 1).ToString();
    }
}
