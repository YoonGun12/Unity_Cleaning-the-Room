using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TitlePanelController : MonoBehaviour
{
    [SerializeField] private RectTransform title1;
    [SerializeField] private RectTransform title2;
    [SerializeField] private RectTransform title3;
    [SerializeField] private RectTransform title4;
    [SerializeField] private GameObject menuBtn;
    [SerializeField] private RectTransform settingsPanelRectTransform;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject introPanel;

    
    private void Start()
    {
        PlayTitleAnimation();
    }

    private void PlayTitleAnimation()
    {
        var startY = 700f; // 시작 위치 (화면 위)
        var endY = 0f; // 최종 위치
        var bounceHeight = 100f; // 튀는 높이
        var duration = 0.5f; // 기본 내려오는 시간
        var bounceDuration = 0.5f; // 튀는 시간
        var delayBetweenLetters = 0.4f; // 각 글자의 시작 시간 간격


        AnimateTitleLetter(title1, startY, endY, bounceHeight, duration, bounceDuration, 0 * delayBetweenLetters);
        AnimateTitleLetter(title2, startY, endY, bounceHeight, duration, bounceDuration, 1 * delayBetweenLetters);
        AnimateTitleLetter(title3, startY, endY, bounceHeight, duration, bounceDuration, 2 * delayBetweenLetters);
        AnimateTitleLetter(title4, startY, endY, bounceHeight, duration, bounceDuration, 3 * delayBetweenLetters);
        StartCoroutine(ShowMenuBtn());
    }

    private void AnimateTitleLetter(RectTransform titleLetter, float startY, float endY, float bounceHeight, float duration, float bounceDuration, float delay)
    {
        if (titleLetter == null) return;

        Vector2 startPos = titleLetter.anchoredPosition;
        startPos.y = startY;
        titleLetter.anchoredPosition = startPos;

        titleLetter.DOAnchorPosY(endY, duration).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(() =>
        {
            titleLetter.DOAnchorPosY(endY + bounceHeight, bounceDuration).SetEase(Ease.OutQuad)
                .OnComplete(() => titleLetter.DOAnchorPosY(endY, bounceDuration).SetEase(Ease.InQuad));
        });
    }

    IEnumerator ShowMenuBtn()
    {
        yield return new WaitForSeconds(2f);
        menuBtn.GetComponent<RectTransform>().DOAnchorPosY(-300f, 0f);
        menuBtn.GetComponent<RectTransform>().DOAnchorPosY(80f, 1f);
    }

    public void OnClickStartBtn()
    {
        introPanel.SetActive(true);
        GameManager.Instance.introPanelController.StartIntro();
    }

}
