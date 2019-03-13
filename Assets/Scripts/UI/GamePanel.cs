using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour {
    private Button btn_Pause;
    private Button btn_Play;
    private Text txt_Score;
    private Text txt_DiamondCount;

    private void Awake()
    {

        EventCenter.AddListener(EventType.ShowGamePanel, Show);
        EventCenter.AddListener<int>(EventType.UpdateScoreText, UpdateScoreText);
        EventCenter.AddListener<int>(EventType.UpdateDiamondText, UpdateDiamondText);
        Init();


    }
    private void Init()
    {
        btn_Pause = transform.Find("btn_Pause").GetComponent<Button>();
        btn_Pause.onClick.AddListener(OnPauseButtonClick);
        btn_Play = transform.Find("btn_Play").GetComponent<Button>();
        btn_Play.onClick.AddListener(OnPlayButtonClick);
        txt_Score = transform.Find("txt_Score").GetComponent<Text>();
        txt_DiamondCount = transform.Find("Diamond/txt_DiamondCount").GetComponent<Text>();


        btn_Play.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.ShowGamePanel, Show);
        EventCenter.RemoveListener<int>(EventType.UpdateScoreText, UpdateScoreText);
        EventCenter.RemoveListener<int>(EventType.UpdateDiamondText, UpdateDiamondText);

    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 更新分数文本
    /// </summary>
    /// <param name="score"></param>
    private void UpdateScoreText(int score)
    {
        txt_Score.text = score.ToString();
    }
    /// <summary>
    /// 更新钻石数量
    /// </summary>
    /// <param name="diamondCount"></param>
    private void UpdateDiamondText(int diamondCount)
    {
        txt_DiamondCount.text = diamondCount.ToString();
    }
    /// <summary>
    /// 暂停按钮点击
    /// </summary>
    private void OnPauseButtonClick()
    {
        btn_Pause.gameObject.SetActive(false);
        btn_Play.gameObject.SetActive(true);
        //游戏暂停
        Time.timeScale = 0;
        GameManager.Instance.IsGamePause = true;
    }
    /// <summary>
    /// 开始按钮点击
    /// </summary>
    private void OnPlayButtonClick()
    {
        btn_Pause.gameObject.SetActive(true);
        btn_Play.gameObject.SetActive(false);
        //继续游戏
        Time.timeScale = 1;
        GameManager.Instance.IsGamePause = false;

    }
}
