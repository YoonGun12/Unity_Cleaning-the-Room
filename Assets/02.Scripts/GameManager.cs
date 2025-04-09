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
    public IntroPanelController introPanelController;
    public PlayerController player;

    public bool isPlay = false;
    public int gameScore = 0;
    public int destroyObjectCount = 0;

    private void Start()
    {
        isPlay = false;
    }

    public void GameQuit()
    {
        Application.Quit();
    }
    
    

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}