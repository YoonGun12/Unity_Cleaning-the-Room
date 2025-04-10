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
    [SerializeField] private float itemDropChance = 0.5f;


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
            StartCoroutine(DestroyObject());
        }
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(1f);
        if (Random.value <= itemDropChance && itemPrefabs.Length > 0)
        {
            GameObject item = Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], itemObjectParent);
            item.transform.position = transform.position;

            item.transform.DOMoveY(transform.position.y + 0.5f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                item.transform.DOMoveY(transform.position.y, 1f).SetEase(Ease.InQuad);
            });
        }
        
        GameManager.Instance.gameScore += score;
        GameManager.Instance.destroyObjectCount++;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerAttack"))
        {
            float multiplier = GameManager.Instance.player.DamageMultiplier;
            switch (GameManager.Instance.player.attackType)
            {
                case PlayerController.AttackType.L1:
                    Damaged((int)(10 * multiplier));
                    break;
                case PlayerController.AttackType.L2:
                    Damaged((int)(25 * multiplier));
                    break;
                case PlayerController.AttackType.R1:
                    Damaged((int)(15 * multiplier));
                    break;
                case PlayerController.AttackType.R2:
                    Damaged((int)(30 * multiplier));
                    break;
                case PlayerController.AttackType.DropKick:
                    Damaged((int)(40 * multiplier));
                    break;
                case PlayerController.AttackType.HurricaneKick:
                    Damaged((int)(100 * multiplier));
                    break;
            }
        }
    }

    
}
