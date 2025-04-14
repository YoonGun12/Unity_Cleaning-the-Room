using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour
{
    [SerializeField] private Image hpGauge;

    private Transform target;
    

    public void Setup(Transform targetTransform)
    {
        target = targetTransform;
    }

    private void Update()
    {
        if (target)
        {
            transform.position = target.position + Vector3.up * 2f;
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    public void SetHP(float hpRatio)
    {
        hpGauge.fillAmount = hpRatio;
    }
}
