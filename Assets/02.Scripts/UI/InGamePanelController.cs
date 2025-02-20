using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanelController : MonoBehaviour
{
    [Header("능력게이지")]
    [SerializeField] private Slider powerSlider;

    [Header("게임 설정")] 
    [SerializeField] private TextMeshProUGUI gameTimeText;
    
    private float gameTime;
    [SerializeField] private RectTransform resultPanelRectTransform;

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
            resultPanelRectTransform.anchoredPosition = Vector2.zero;
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

    public void AddTime(float addTime)
    {
        gameTime += addTime;
    }
    
    
    public void SetGameTime(float time)
    {
        gameTime += time;
    }
}