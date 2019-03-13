﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameOverPanel : MonoBehaviour {
    public Text txt_Score, txt_BestScore, txt_AddDiamondCount;
    public Button btn_Restart, btn_Rank, btn_Home;
    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        btn_Restart.onClick.AddListener(OnRestartButtonClick);
        btn_Rank.onClick.AddListener(OnRankButtonClick);
        btn_Home.onClick.AddListener(OnHomeButtonClick);
        EventCenter.AddListener(EventType.ShowGameOverPanel, Show);
        gameObject.SetActive(false);
    }
    
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowGameOverPanel, Show);

    }
    private void Show()
    {
        txt_Score.text = GameManager.Instance.GetGameScore().ToString();
        txt_AddDiamondCount.text = "+" + GameManager.Instance.GetGameDiamondCount().ToString();
        gameObject.SetActive(true);

    }
    /// <summary>
    /// 再来一局按钮点击
    /// </summary>
    private void OnRestartButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.IsAgainGame = true;
    }
    /// <summary>
    /// 排行榜按钮点击
    /// </summary>
    private void OnRankButtonClick()
    {

    }
    /// <summary>
    /// 主界面按钮点击
    /// </summary>
    private void OnHomeButtonClick()
    {
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameData.IsAgainGame = false;
    }
}
