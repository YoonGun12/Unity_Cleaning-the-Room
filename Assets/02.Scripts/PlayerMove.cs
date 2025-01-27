using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    [Header("플레이어 이동")] 
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private Transform playerPivot;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    
    private Vector2 moveInput;
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private bool isGround;
    private Rigidbody rigid;
    private float targetRotation;
    private float rotationVelocity;
    private float previousSpeed = 0f;

    [Header("카메라")] 
    [SerializeField] private Transform cameraTransform;

    [Header("애니메이션")] 
    private Animator anim;

    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;

    private void Awake()
    {
        UnityEngine.InputSystem.PlayerInput Input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        rigid = GetComponent<Rigidbody>();
        moveAction = Input.actions["Move"];
        runAction = Input.actions["Run"];
        jumpAction = Input.actions["Jump"];
        anim = GetComponent<Animator>();

        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
    }

    private void Update()
    {
        Move();
        Jump();

        UpdateAnimator();
    }

    private void Move()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        //최종 이동 방향
        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

        float currentSpeed = runAction.IsPressed() ? runSpeed : walkSpeed;
        
        rigid.velocity = new Vector3(moveDirection.x * currentSpeed, rigid.velocity.y, moveDirection.z * currentSpeed);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, smoothRotation, 0);
        }
    }
    
    private void Jump()
    {
        float rayLength = transform.localScale.y * 0.2f;
        Debug.DrawRay(playerPivot.position, Vector3.down * rayLength, Color.green);
        isGround = Physics.Raycast(playerPivot.position, Vector3.down, rayLength);
        
        anim.SetBool(animIDGrounded, isGround);
        
        if (isGround)
        {
            anim.SetBool(animIDFreeFall, false);

            if (jumpAction.triggered)
            {
                rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                isGround = false;
            }
        }
        else
        {
            anim.SetBool(animIDFreeFall, true);
        }

    }

    private void UpdateAnimator()
    {
        float currentSpeed = new Vector3(rigid.velocity.x, 0, rigid.velocity.z).magnitude;

        if (!Mathf.Approximately(previousSpeed, currentSpeed))
        {
            anim.SetFloat(animIDSpeed, currentSpeed);
            previousSpeed = currentSpeed;
        }

        anim.SetBool(animIDGrounded, isGround);
    }
    
    private void OnLand(AnimationEvent animationEvent)
    {
        Debug.Log("착지");
    }
}
