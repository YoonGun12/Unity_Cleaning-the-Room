using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanelController : MonoBehaviour
{
    [Header("게임 설정")] 
    [SerializeField] private TMP_Text gameTimeText;
    
    private float gameTime;
    [SerializeField] private RectTransform resultPanelRectTransform;
    [SerializeField] private TMP_Text gameScoreText;

    [SerializeField] private Image L1Image;
    [SerializeField] private Image L2Image;
    [SerializeField] private Image R1Image;
    [SerializeField] private Image R2Image;
    [SerializeField] private Image DropKickImage;
    [SerializeField] private Image HurricaneKickImage;

    private void Start()
    {
        L1Image.fillAmount = 0;
        L2Image.fillAmount = 0;
        R1Image.fillAmount = 0;
        R2Image.fillAmount = 0;
        DropKickImage.fillAmount = 0;
        HurricaneKickImage.fillAmount = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.isPlay)
        {
            if (gameTime > 0)
            {
                gameTime -= Time.deltaTime;
                UpdateGameTime();
            }
            else
            {
                GameManager.Instance.isPlay = false;
                gameTime = 0;
                gameTimeText.text = "00:00";
                gameTimeText.color = Color.red;
                resultPanelRectTransform.DOAnchorPos(Vector2.zero, 1f);
                AudioManager.instance.PlayBgm(false);
            }

            gameScoreText.text = $"점수 : "+ GameManager.Instance.gameScore.ToString();
        }
    }

    public void UpdateCooldownUI(PlayerMove.AttackType attackType, float fillAmount)
    {
        switch (attackType)
        {
            case PlayerMove.AttackType.L1:
                L1Image.fillAmount = fillAmount;
                break;
            case PlayerMove.AttackType.L2:
                L2Image.fillAmount = fillAmount;
                break;
            case PlayerMove.AttackType.R1:
                R1Image.fillAmount = fillAmount;
                break;
            case PlayerMove.AttackType.R2:
                R2Image.fillAmount = fillAmount;
                break;
            case PlayerMove.AttackType.DropKick:
                DropKickImage.fillAmount = fillAmount;
                break;
            case PlayerMove.AttackType.HurricaneKick:
                HurricaneKickImage.fillAmount = fillAmount;
                break;
        }
    }

    private void UpdateGameTime()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (gameTime <= 30f)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Timeup);
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