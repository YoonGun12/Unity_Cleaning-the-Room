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
                AudioManager.instance.PlayBgm(AudioManager.Bgm.InGame3,false);
                AudioManager.instance.PlayBgm(AudioManager.Bgm.Result,true);
                //AudioManager.instance.PlaySfx(AudioManager.Sfx.TimeOut);
                StartCoroutine(GameOverResult());
            }

            gameScoreText.text = $"점수 : "+ GameManager.Instance.gameScore.ToString();
        }
    }

    IEnumerator GameOverResult()
    {
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0;
    }

    public void UpdateCooldownUI(PlayerController.AttackType attackType, float fillAmount)
    {
        switch (attackType)
        {
            case PlayerController.AttackType.L1:
                if(L1Image != null)
                    L1Image.fillAmount = fillAmount;
                break;
            case PlayerController.AttackType.L2:
                if(L2Image != null)
                    L2Image.fillAmount = fillAmount;
                break;
            case PlayerController.AttackType.R1:
                if(R1Image != null)
                    R1Image.fillAmount = fillAmount;
                break;
            case PlayerController.AttackType.R2:
                if(R2Image != null)
                    R2Image.fillAmount = fillAmount;
                break;
            case PlayerController.AttackType.DropKick:
                if(DropKickImage != null)
                    DropKickImage.fillAmount = fillAmount;
                break;
            case PlayerController.AttackType.HurricaneKick:
                if(HurricaneKickImage != null)
                    HurricaneKickImage.fillAmount = fillAmount;
                break;
        }
    }

    private void UpdateGameTime()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        gameTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (gameTime <= 120f && gameTime > 30f)
        {
            AudioManager.instance.PlayBgm(AudioManager.Bgm.InGame1, false);
            AudioManager.instance.PlayBgm(AudioManager.Bgm.InGame2, true);
        }

        if (gameTime <= 30f)
        {
            AudioManager.instance.PlayBgm(AudioManager.Bgm.InGame2, false);
            AudioManager.instance.PlayBgm(AudioManager.Bgm.InGame3, true);
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Timeup);
            gameTimeText.color = Color.red;
        }
    }

    public void AddTime(float addTime)
    {
        gameTime += addTime;
    }
    
    
    public void SetGameTime(float time)
    {
        gameTime = time;
    }
}