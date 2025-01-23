using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("플레이어 이동")] 
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    private Vector2 moveInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private bool isGround = true;
    private Rigidbody rigid;

    private void Awake()
    {
        UnityEngine.InputSystem.PlayerInput Input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        rigid = GetComponent<Rigidbody>();
        moveAction = Input.actions["Move"];
        //jumpAction = Input.actions["Jump"];
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        rigid.velocity = new Vector3(moveInput.x * moveSpeed, rigid.velocity.y, moveInput.y * moveSpeed);


        float rayLength = transform.localScale.y * 0.1f;
        Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.green);
        isGround = Physics.Raycast(transform.position, Vector3.down, rayLength);
        if (/*jumpAction.triggered*/ Input.GetKeyDown(KeyCode.Space) /*&& isGround*/)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGround = false;
        }
        
    }
    
    
}
