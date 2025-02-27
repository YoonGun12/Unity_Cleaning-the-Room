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
    [SerializeField] private TextMeshProUGUI wookisComment;

    private Vector2 resultPanelRectOrigin;

    private void Awake()
    {
        resultPanelRectOrigin = resultPanel.GetComponent<RectTransform>().anchoredPosition;
    }

    private void Update()
    {
        if (GameManager.Instance.destroyObjectCount < 100)
        {
            wookisComment.text = "욱이가 집에 도착했습니다.\n 욱이 : 이 정도면 내가 직접 치우는게 낫겠는데...?";
        }
        else if (GameManager.Instance.destroyObjectCount >= 100 && GameManager.Instance.destroyObjectCount < 500)
        {
            wookisComment.text = "욱이가 집에 도착했습니다.\n 욱이 : 뭐야, 방치야 쓰레기랑 가구는 구분해야지!!!!!!!";
        }
        else
        {
            wookisComment.text = "욱이가 집에 도착했습니다!!!\n 욱이 : ??? 저 XX 당장 쫒아내!!";
        }
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
