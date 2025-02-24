using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

enum DestructibleObjectType
{
    Food, SmallFurniture,BigFurniture, Book, WasteBag, Waste, SmallObject
}

public class DesturctibleObject : MonoBehaviour
{
    [SerializeField] private DestructibleObjectType _objectType;
    [SerializeField] private int hp;
    [SerializeField] private int score;
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private Transform itemObjectParent;

    private void Start()
    {
        InitObject(_objectType);
    }

    private void InitObject(DestructibleObjectType objectType)
    {
        switch (objectType)
        {
            case DestructibleObjectType.Food:
                hp = Random.Range(15, 26);
                score = hp * 8;
                break;
            case DestructibleObjectType.SmallFurniture:
                hp = Random.Range(80, 121);
                score = hp * 6;
                break;
            case DestructibleObjectType.BigFurniture:
                hp = Random.Range(180, 251);
                score = hp * 4;
                break;
            case DestructibleObjectType.Book:
                hp = Random.Range(20, 41);
                score = hp * 7;
                break;
            case DestructibleObjectType.WasteBag:
                hp = Random.Range(90, 181);
                score = hp * 5;
                break;
            case DestructibleObjectType.Waste:
                hp = Random.Range(30, 51);
                score = hp * 6;
                break;
            case DestructibleObjectType.SmallObject:
                hp = Random.Range(20, 41);
                score = hp * 7;
                break;
        }

        score = hp * 10;
    }

    public void Damaged(int damage)
    {
        hp -= damage;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.HardObject);

        
        if (hp <= 0)
        {
            //TODO: 사라지는 소리 type별 switch
            StartCoroutine(DestroyObject());
        }
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(1f);
        GameObject item = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], itemObjectParent);
        item.transform.position = transform.position;

        item.transform.DOMoveY(transform.position.y + 0.5f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            item.transform.DOMoveY(transform.position.y, 1f).SetEase(Ease.InQuad);
        });
        GameManager.Instance.gameScore += score;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerAttack"))
        {
            switch (GameManager.Instance.player.attackType)
            {
                case PlayerMove.AttackType.L1:
                    Damaged(10);
                    break;
                case PlayerMove.AttackType.L2:
                    Damaged(25);
                    break;
                case PlayerMove.AttackType.R1:
                    Damaged(15);
                    break;
                case PlayerMove.AttackType.R2:
                    Damaged(30);
                    break;
                case PlayerMove.AttackType.DropKick:
                    Damaged(40);
                    break;
                case PlayerMove.AttackType.HurricaneKick:
                    Damaged(100);
                    break;
            }
        }
    }

    
}
