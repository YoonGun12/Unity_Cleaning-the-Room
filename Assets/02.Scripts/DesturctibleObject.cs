using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum DestructibleObjectType
{
    Food, Furniture, ETC
}

public class DesturctibleObject : MonoBehaviour
{
    [SerializeField] private int hp;

    public void Damaged(int damage)
    {
        hp -= damage;
        //TODO: 데미지 받는 소리 type별 switch
        
        if (hp <= 0)
        {
            //TODO: 사라지는 소리 type별 switch
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            var attack = other.GetComponent<KickCollision>();
            if (attack != null)
            {
                //ApplyDamagedByType(attack.AttackType);
            }
        }
    }

    private void ApplyDamagedByType(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.Punch:
                Damaged(20);
                break;
            case AttackType.Kick:
                Damaged(20);
                break;
            case AttackType.Special:
                Damaged(50);
                break;
        }
    }
}
