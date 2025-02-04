using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("게임 설정")] 
    [SerializeField] private UIManager uiManager; // UIManager 참조 추가
    
    public int gameScore = 0;

    private void Start()
    {
        if (uiManager != null)
        {
            uiManager.SetGameTime(60f); // 초기 게임 시간 설정
        }
    }
}