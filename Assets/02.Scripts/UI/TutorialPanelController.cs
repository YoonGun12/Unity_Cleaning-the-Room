using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialPanelController : MonoBehaviour
{
    private RectTransform tutorialPanelRectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 tutorialPanelPosOrigin;

    private void Awake()
    {
        tutorialPanelRectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        
        tutorialPanelPosOrigin = gameObject.GetComponent<RectTransform>().anchoredPosition;
        _canvasGroup.alpha = 0;
    }

    public void OnClickTutorialBtn()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonClick);
        tutorialPanelRectTransform.DOAnchorPos(Vector2.zero, 0.5f);
        _canvasGroup.DOFade(1, 0.5f);
    }

    public void OnClickCloseTutorialBtn()
    {
        tutorialPanelRectTransform.DOAnchorPos(tutorialPanelPosOrigin, 0.5f);
        _canvasGroup.DOFade(0, 0.5f);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonClick);
    }
}
