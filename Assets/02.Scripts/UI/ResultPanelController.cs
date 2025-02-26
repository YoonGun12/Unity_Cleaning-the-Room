using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPanelController : MonoBehaviour
{
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private TextMeshProUGUI gameScoreText;
    [SerializeField] private TextMeshProUGUI destroyObjectCountText;


    private void Update()
    {
        gameScoreText.text = $"점수 : {GameManager.Instance.gameScore}";
        destroyObjectCountText.text = $"청소한 물건 : {GameManager.Instance.destroyObjectCount} 개";
    }

    public void OnClickQuitButton()
    {
        SceneManager.LoadScene(0);
    }
}
