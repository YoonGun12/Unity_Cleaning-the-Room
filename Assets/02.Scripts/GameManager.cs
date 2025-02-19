using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : Singleton<GameManager>
{
    [Header("게임 설정")] 
    [SerializeField] private InGamePanelController inGamePanelController; // UIManager 참조 추가

    public bool isPlay;
    
    public int gameScore = 0;

    private void Start()
    {
        if (inGamePanelController != null)
        {
            inGamePanelController.SetGameTime(10f); // 초기 게임 시간 설정
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}