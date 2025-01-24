using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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
    private bool isGround;
    private Rigidbody rigid;
    [SerializeField] private Transform playerPivot;

    private void Awake()
    {
        UnityEngine.InputSystem.PlayerInput Input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        rigid = GetComponent<Rigidbody>();
        moveAction = Input.actions["Move"];
        jumpAction = Input.actions["Jump"];
    }

    private void Update()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        rigid.velocity = new Vector3(moveInput.x * moveSpeed, rigid.velocity.y, moveInput.y * moveSpeed);
    }
    
    private void Jump()
    {
        float rayLength = transform.localScale.y * 0.2f;
        Debug.DrawRay(playerPivot.position, Vector3.down * rayLength, Color.green);
        isGround = Physics.Raycast(playerPivot.position, Vector3.down, rayLength);
        if (jumpAction.triggered && isGround)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isGround = false;
        }

    }
}
