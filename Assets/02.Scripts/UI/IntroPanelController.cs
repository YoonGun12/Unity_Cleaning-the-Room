using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class IntroPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text introText;
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject inGamePanel;
    
    private string[] introLines = new string[]
    {
        "202X년 XX월 XX일...어느 지저분한 방...\n집 주인 욱이는 집을 아주 더럽게 사용하고 있었다.. ",
        "그러던 어느 날, 욱이는 한 경품 이벤트에 당첨되었다!\n상품은 바로 개발 중인 인간형 로봇청소기!!",
        "마침 집도 엉망이 된 상태에서 욱이는 매우 기뻐했다!",
        "이 작은 강아지 크기의 로봇 청소기는 설명서에 따르면,\n크기를 자유자재로 조절하며 방을 치울 수 있다고 적혀 있었다.",
        "욱이는 로봇에게 이름을 지어주기로 했다.\n지금까지 '방치'된 자신의 집을 보고, 자신의 성을 따와 '김방치'라 지었다.",
        "그렇게 욱이는 방치를 가동하고 잠깐 외출한다.",
        "가동된 방치는 집을 청소하기 시작하는데...",
        "욱이가 돌아오기까지 남은 시간은 단 5분!\n방치로 모든 집안 물건들을 청소하자!!!",
    };
    
    private int currentLine = 0;
    private bool isTyping = false;
    private float textSpeed = 0.05f;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartIntro()
    {
        transform.localScale = Vector2.zero;
        currentLine = 0;
        _canvasGroup.alpha = 0;
        
        transform.DOScale(Vector2.one, 0.2f);
        _canvasGroup.DOFade(1f, 0.2f);
        StartCoroutine(TypeText());
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTyping)
        {
            NextLine();
        }
    }
    
    private IEnumerator TypeText()
    {
        isTyping = true;
        introText.text = "";
        foreach (var letter in introLines[currentLine].ToCharArray())
        {
            introText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    private void NextLine()
    {
        if (currentLine < introLines.Length - 1)
        {
            currentLine++;
            StartCoroutine(TypeText());
            AudioManager.instance.PlaySfx(AudioManager.Sfx.ButtonClick);
        }
        else
        {
            EndIntro();
        }
    }

    private void EndIntro()
    {
        titlePanel.SetActive(false);
        introPanel.SetActive(false);
        inGamePanel.SetActive(true);
        GameManager.Instance.isPlay = true;
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Title, false);
        AudioManager.instance.PlayBgm(AudioManager.Bgm.InGame1, true);
    }
}
