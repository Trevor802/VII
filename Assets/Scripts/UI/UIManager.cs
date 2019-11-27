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
        }
        else if (UIInstance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
    #endregion

    public RawImage[] lifeIcons;
    public RawImage[] crossIcons;
    public bool gameOver = false;
    public Vector3 restartPos;
    public int levelIndex;

    public void UpdateUI()
    {
        Player player = FindObjectOfType<Player>();
        VII.PlayerData playerData = player.GetComponent<Player>().PlayerData;
        for (int i = 0; i < player.initLives; i++)
        {
            if (i <= playerData.lives - 1)
            {
                lifeIcons[i].enabled = true;
                crossIcons[i].enabled = false;
            }
            else
            {
                lifeIcons[i].enabled = false;
                crossIcons[i].enabled = true;
            }
        }
    }

    public void ClearUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].enabled = false;
            crossIcons[i].enabled = false;
        }
    }

    public void initUI()
    {
        Player player = FindObjectOfType<Player>();
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            crossIcons[i].enabled = false;
            if (i <= player.initLives - 1)
            {
                lifeIcons[i].enabled = true;
            }
            else
            {
                lifeIcons[i].enabled = false;
            }
        }
    }
}
