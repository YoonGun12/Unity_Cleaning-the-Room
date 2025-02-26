using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultPanelController : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private TextMeshProUGUI gameScoreText;
    [SerializeField] private TextMeshProUGUI destroyObjectCountText;

    private Vector2 resultPanelRectOrigin;

    private void Awake()
    {
        resultPanelRectOrigin = resultPanel.GetComponent<RectTransform>().anchoredPosition;
    }

    private void Update()
    {
        gameScoreText.text = $"점수 : {GameManager.Instance.gameScore}";
        destroyObjectCountText.text = $"청소한 물건 : {GameManager.Instance.destroyObjectCount} 개";
    }

    public void OnClickQuitButton()
    {
        Time.timeScale = 1;
        titlePanel.SetActive(true);
        inGamePanel.SetActive(false);
        resultPanel.GetComponent<RectTransform>().anchoredPosition = resultPanelRectOrigin;
        titlePanel.GetComponent<TitlePanelController>().PlayTitleAnimation();
        GameManager.Instance.player.transform.position = new Vector3(0, 0.733f, 0);
    }
}
