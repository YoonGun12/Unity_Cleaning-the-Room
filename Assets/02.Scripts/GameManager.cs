using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("능력게이지")]
    [SerializeField] private Slider powerSlider;
    [SerializeField] private float increasePowerSpeed;
    [SerializeField] private float decreasePowerSpeed;
    private bool isMousePressed;
    
    [Header("게임 설정")] 
    [SerializeField] private TextMeshProUGUI gameTimeText;
    private float gameTime = 60f;

    public int gameScore = 0;
    
    private void Update()
    {
        if (gameTime > 0)
        {
            gameTime -= Time.deltaTime;
            UpdateGameTime();
        }
        else
        {
            gameTime = 0;
            gameTimeText.text = "00:00";
            gameTimeText.color = Color.red;
        }

        if (Input.GetMouseButton(0))
        {
            isMousePressed = true;
            DecreasePower();
        }
        else
        {
            isMousePressed = false;
            IncreasePower();
        }
    }

    private void UpdateGameTime()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (gameTime <= 30f)
        {
            gameTimeText.color = Color.red;
        }
    }

    private void IncreasePower()
    {
        if (!isMousePressed && powerSlider.value < powerSlider.maxValue)
        {
            powerSlider.value += increasePowerSpeed * Time.deltaTime;
        }
    }

    private void DecreasePower()
    {
        powerSlider.value -= decreasePowerSpeed * Time.deltaTime;
    }
}
