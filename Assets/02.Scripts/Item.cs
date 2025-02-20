using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum ItemType { SpeedUp, TimeExtension, SizeUp, SizeDown, PowerUp, Magnet }
    public ItemType itemType;

    private void Update()
    {
        transform.Rotate(Vector3.up * (30 * Time.deltaTime));
    }
}
