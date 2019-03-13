using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    /// <summary>
    /// 游戏是否开始
    /// </summary>
    public bool IsGameStarted { get; set; }
    /// <summary>
    /// 游戏是否结束
    /// </summary>
    public bool IsGameOver { get; set; }
    /// <summary>
    /// 游戏是否暂停
    /// </summary>
    public bool IsGamePause { get; set; }
    /// <summary>
    /// 玩家是否移动
    /// </summary>
    public bool IsPlayerMove { get; set; }
    /// <summary>
    /// 游戏分数
    /// </summary>
    private int gameScore;
    private int gameDiamondCount;
    private void Awake()
    {
        Instance = this;
        EventCenter.AddListener(EventType.AddScore, AddGameScore);
        EventCenter.AddListener(EventType.PlayerMove, PlayerMove);
        EventCenter.AddListener(EventType.AddDiamond, AddGameDiamondCount);

        if (GameData.IsAgainGame == true)
        {
            IsGameStarted = true;

        }
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.AddScore, AddGameScore);
        EventCenter.RemoveListener(EventType.PlayerMove, PlayerMove);
        EventCenter.RemoveListener(EventType.AddDiamond, AddGameDiamondCount);
    }
    /// <summary>
    /// 玩家移动调用的方法
    /// </summary>
    private void PlayerMove()
    {
        IsPlayerMove = true;
    }
    /// <summary>
    /// 获取游戏分数
    /// </summary>
    /// <returns>分数，int类型</returns>
    public int GetGameScore()
    {
        return gameScore;
    }
    /// <summary>
    /// 增加游戏分数，每次只增加1，没有参数
    /// </summary>
    private void AddGameScore()
    {
        if (IsGameOver == true || IsGameStarted == false || IsGamePause == true)
        {
            return;
        }
        gameScore++;
        EventCenter.Broadcast(EventType.UpdateScoreText, gameScore);
    }
    /// <summary>
    /// 获取钻石个数
    /// </summary>
    /// <returns>钻石个数，int类型</returns>
    public int GetGameDiamondCount()
    {
        return gameDiamondCount;
    }
    /// <summary>
    /// 增加钻石个数，每次只增加1，没有参数
    /// </summary>
    private void AddGameDiamondCount()
    {
        if (IsGameOver == true || IsGameStarted == false || IsGamePause == true)
        {
            return;
        }
        gameDiamondCount++;
        EventCenter.Broadcast(EventType.UpdateDiamondText, gameDiamondCount);
    }
    
}
