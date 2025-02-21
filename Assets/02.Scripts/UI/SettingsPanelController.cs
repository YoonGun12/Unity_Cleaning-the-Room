using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SettingsPanelController : MonoBehaviour
{
    private RectTransform settingsPanelRect;
    private CanvasGroup settingsCanvasGroup;
    private Vector2 settingsPanelPosOrigin;
    
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject mainButton;

    private bool isPanelVisible = false;
    
    private void Awake()
    {
        settingsPanelRect = GetComponent<RectTransform>();
        settingsCanvasGroup = GetComponent<CanvasGroup>();
        
        settingsPanelPosOrigin = gameObject.GetComponent<RectTransform>().anchoredPosition;
        settingsCanvasGroup.alpha = 0;
    }

    private void Update()
    {
        if (GameManager.Instance.isPlay && Input.GetKeyDown(KeyCode.Escape))
        {
            OnOffSettingsPanel();
        }
    }

    public void OnOffSettingsPanel()
    {
        if (isPanelVisible)
        {
            OnClickCloseSettingPanelBtn();
        }
        else
        {
            OnClickOpenSettingPanelBtn();
        }
    }
    
    public void OnClickOpenSettingPanelBtn()
    {
        isPanelVisible = true;
        
        if (GameManager.Instance.isPlay)
        {
            retryButton.SetActive(true);
            mainButton.SetActive(true);
        }
        else
        {
            retryButton.SetActive(false);
            mainButton.SetActive(false);
        }

        settingsPanelRect.DOAnchorPosY(0, 0.5f);
        settingsCanvasGroup.DOFade(1, 0.5f);
    }
    
    public void OnClickCloseSettingPanelBtn()
    {
        settingsPanelRect.DOAnchorPosY(settingsPanelPosOrigin.y, 0.5f);
        settingsCanvasGroup.DOFade(0, 0.5f);
    }

    public void OnClickSFXBtn()
    {
        
    }

    public void OnClickBGMBtn()
    {
        
    }
    public void OnClickRetryBtn()
    {
        
    }

    public void OnClickMainBtn()
    {
        
    }
}
