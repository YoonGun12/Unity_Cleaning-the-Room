using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsPanelController : MonoBehaviour
{
    private Vector2 settingsPanelPosOrigin;
    private RectTransform settingsPanelRectTransform;
    [SerializeField] private GameObject titlePanel;

    private void Awake()
    {
        settingsPanelRectTransform = GetComponent<RectTransform>();
        settingsPanelPosOrigin = settingsPanelRectTransform.anchoredPosition;
    }
    

    public void OnClickSettingPanelBtn()
    {
        if (titlePanel)
        {
            
        }
        settingsPanelRectTransform.anchoredPosition = Vector2.zero;
    }
    
    public void OnClickCloseSettingPanelBtn()
    {
        settingsPanelRectTransform.anchoredPosition = settingsPanelPosOrigin;
    }
}
