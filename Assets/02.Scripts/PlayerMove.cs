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
    [SerializeField] private float walkSpeed; //걷기 속도
    [SerializeField] private float runSpeed; //달리기 속도
    [SerializeField] private float jumpPower; //점프 파워
    [SerializeField] private Transform playerPivot;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    
    //입력
    private Vector2 moveInput;
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    //점프
    private bool isGround;
    //회전
    private float targetRotation;
    private float rotationVelocity;
    //펀치
    [SerializeField] private Collider punchCollider;
    
    private Rigidbody rigid;

    [Header("카메라")] 
    [SerializeField] private Transform cameraTransform;

    [Header("애니메이션")] 
    private Animator anim;

    private void Awake()
    {
        PlayerInput Input = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody>();
        moveAction = Input.actions["Move"];
        runAction = Input.actions["Run"];
        jumpAction = Input.actions["Jump"];
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        Jump();
        Punch();
    }

    private void Move()
    {
        //이동 입력
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

        //플레이어 회전
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            targetRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity,
                rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, smoothRotation, 0);
        }
        
        //걷기, 달리기 애니메이션
        anim.SetBool("isWalk", moveDirection != Vector3.zero);
        anim.SetBool("isRun", runAction.IsPressed());
    }
    
    private void Jump()
    {
        float rayLength = transform.localScale.y * 0.2f;
        Debug.DrawRay(playerPivot.position, Vector3.down * rayLength, Color.green);
        isGround = Physics.Raycast(playerPivot.position, Vector3.down, rayLength);
        anim.SetBool("isGround", isGround);
        
        if (isGround) //바닥에 닿아있을 때
        {
            if (jumpAction.triggered) //점프 버튼을 누를 때
            {
                rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                isGround = false;
                anim.SetTrigger("Jump");
            }
            
        }
        else //바닥에 닿지 않아 있을 때
        {
            anim.SetTrigger("InAir");
        }

    }


    private void Punch()
    {
        if (Input.GetMouseButton(0))
        {
            anim.SetTrigger("Punch");
        }
    }

    public void EnablePunchCollider()
    {
        punchCollider.enabled = true;
    }
    
    public void DisablePunchCollider()
    {
        punchCollider.enabled = false;
    }


    private void OnLand(AnimationEvent animationEvent)
    {
        //TODO: 점프 후 착지 소리
    }

    private void OnFootstep()
    {
        //TODO: 발걸음 소리
    }
}
