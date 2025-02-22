using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    [Header("게임 설정")] 
    public InGamePanelController inGamePanelController; // UIManager 참조 추가
    public PlayerMove player;

    public bool isPlay = false;
    public int gameScore = 0;

    private void Start()
    {
        isPlay = false;
        if (inGamePanelController != null)
        {
            inGamePanelController.SetGameTime(300f); // 초기 게임 시간 설정
        }
    }
    

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}