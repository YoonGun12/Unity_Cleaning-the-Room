using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject door;
    private Animator anim;
    private bool isOpen = false;
    private string lastOpenTrigger = "Open";

    private void Awake()
    {
        anim = door.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isOpen)
        {
            Vector3 playerPosition = other.transform.position;
            Vector3 doorPosition = door.transform.position;

            if (Vector3.Dot(transform.forward, playerPosition - doorPosition) > 0)
            {
                lastOpenTrigger = "OpenBackward";
            }
            else
            {
                lastOpenTrigger = "Open";
            }
            anim.SetTrigger(lastOpenTrigger);
            isOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isOpen)
        {
            string closeTrigger = (lastOpenTrigger == "Open") ? "Close" : "CloseBackward";
            anim.SetTrigger(closeTrigger);
            isOpen = false;
        }
    }
}
