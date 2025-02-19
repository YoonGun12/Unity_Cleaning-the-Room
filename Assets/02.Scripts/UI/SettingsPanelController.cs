using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanelController : MonoBehaviour
{
    private Vector2 settingsPanelPosOrigin;
    private RectTransform settingsPanelRectTransform;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject mainButton;

    private void Awake()
    {
        settingsPanelRectTransform = GetComponent<RectTransform>();
        settingsPanelPosOrigin = settingsPanelRectTransform.anchoredPosition;
    }
    

    public void OnClickSettingPanelBtn()
    {
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
        settingsPanelRectTransform.anchoredPosition = Vector2.zero;
    }
    
    public void OnClickCloseSettingPanelBtn()
    {
        settingsPanelRectTransform.anchoredPosition = settingsPanelPosOrigin;
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
