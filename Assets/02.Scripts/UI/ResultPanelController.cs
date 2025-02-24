using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPanelController : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject titlePanel;
    
    
    public void OnClickRetryButton()
    {
        GameManager.Instance.isPlay = true;
        resultPanel.SetActive(false);
        inGamePanel.SetActive(true);
        //플레이어 초기화
        //가구들 hp, 점수 초기화
        

    }

    public void OnClickQuitButton()
    {
        
    }
}
