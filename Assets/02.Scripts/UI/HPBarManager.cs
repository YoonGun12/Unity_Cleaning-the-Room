using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HPBarManager : Singleton<HPBarManager>
{
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private int poolSize = 30;

    private Queue<HPBarController> pool = new Queue<HPBarController>();

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var barObj = Instantiate(hpBarPrefab, transform);
            var bar = barObj.GetComponent<HPBarController>();
            barObj.SetActive(false);
            pool.Enqueue(bar);
        }
    }

    public void ShowHPBar(Transform target, float hpRatio)
    {
        if (pool.Count == 0) return;

        var bar = pool.Dequeue();
        bar.gameObject.SetActive(true);
        bar.Setup(target);
        bar.SetHP(hpRatio);

        StartCoroutine(DisableHpBar(bar, 3f));
    }

    private IEnumerator DisableHpBar(HPBarController bar, float delay)
    {
        yield return new WaitForSeconds(delay);
        bar.gameObject.SetActive(false);
        pool.Enqueue(bar);
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode){ }
}
