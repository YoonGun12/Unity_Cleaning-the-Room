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
    //킥
    [SerializeField] private Collider kickCollider;
    private float doubleClickThreshold = 0.1f;
    private bool leftClick = false;
    private bool rightClick = false;
    private float leftClickTime;
    private float rightClickTime;
    
    private Rigidbody rigid;

    [Header("카메라")] 
    [SerializeField] private Transform cameraTransform;

    [Header("애니메이션")] 
    private Animator anim;
    private MotionTrail motionTrail;
    
    private void Awake()
    {
        PlayerInput Input = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody>();
        moveAction = Input.actions["Move"];
        runAction = Input.actions["Run"];
        jumpAction = Input.actions["Jump"];
        anim = GetComponent<Animator>();
        motionTrail = GetComponent<MotionTrail>();
    }

    private void Update()
    {
        Move();
        Jump();
        Kick();
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


    private void Kick()
    {
        var isEPressed = Input.GetKey(KeyCode.E);
        
        if (Input.GetMouseButtonDown(0))
        {
            leftClick = true;
            leftClickTime = Time.time;
            StartCoroutine(ResetClick("left"));
        }
        if (Input.GetMouseButtonDown(1))
        {
            rightClick = true;
            rightClickTime = Time.time;
            StartCoroutine(ResetClick("right"));
        }
        if (leftClick && rightClick && Mathf.Abs(leftClickTime - rightClickTime) <= doubleClickThreshold)
        {
            leftClick = false;
            rightClick = false;

            anim.SetTrigger(isEPressed ? "HurricaneKick" : "DropKick");
            return;
        }

        if (leftClick && !rightClick && Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger(isEPressed ? "Kick_L2" : "Kick_L1");
        }
        
        if (!leftClick && rightClick && Input.GetMouseButtonDown(1))
        {
            anim.SetTrigger(isEPressed ? "Kick_R2" : "Kick_R1");
        }
    }

    private IEnumerator ResetClick(string button)
    {
        yield return new WaitForSeconds(doubleClickThreshold);

        switch (button)
        {
            case "left":
                leftClick = false;
                break;
            case "right":
                rightClick = false;
                break;
        }
    }
    


    private void OnLand(AnimationEvent animationEvent)
    {
        //TODO: 점프 후 착지 소리
    }

    private void OnFootstep()
    {
        //TODO: 발걸음 소리
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                ApplyItemEffect(item.itemType);
                //TODO: 아이템이 사라지는 효과
            }
        }
    }

    private void ApplyItemEffect(Item.ItemType itemType)
    {
        switch (itemType)
        {
            case Item.ItemType.SpeedUp :
                StartCoroutine(SpeedUpEffect());
                break;
            case Item.ItemType.TimeExtension :
                GameManager.Instance.inGamePanelController.AddTime(30f);
                break;
            case Item.ItemType.Magnet :
                StartCoroutine(MagnetEffect());
                break;
            case Item.ItemType.PowerUp :

                break;
            case Item.ItemType.SizeDown :

                break;
            case Item.ItemType.SizeUp :
                
                break;
        }
    }

    IEnumerator SpeedUpEffect()
    {
        //TODO: 중복 스피드업 금지
        var originalWalkSpeed = walkSpeed;
        var originalRunSpeed = runSpeed;

        motionTrail.StartMotionTrail();
        walkSpeed *= 1.5f;
        runSpeed *= 1.5f;
        yield return new WaitForSeconds(5f);
        walkSpeed = originalWalkSpeed;
        runSpeed = originalRunSpeed;
    }

    IEnumerator MagnetEffect()
    {
        var magnetRadius = 5f;
        var magnetSpeed = 2f;
        var elapsedTime = 0f;

        while (elapsedTime < 5f)
        {
            Collider[] destructibles = Physics.OverlapSphere(transform.position, magnetRadius);
            foreach (var destructible in destructibles )
            {
                if (destructible.CompareTag("Destrictible"))
                {
                    destructible.transform.position = Vector3.MoveTowards(destructible.transform.position,
                        transform.position, magnetSpeed * Time.deltaTime);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
