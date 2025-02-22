using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private Collider LeftFootCollider;
    [SerializeField] private Collider RightFootCollider;
    private float doubleClickThreshold = 0.1f;
    private bool leftClick = false;
    private bool rightClick = false;
    private float leftClickTime;
    private float rightClickTime;
    private bool isMove = true;
    
    private Rigidbody rigid;

    [Header("카메라")] 
    [SerializeField] private Transform cameraTransform;

    [Header("애니메이션")] 
    private Animator anim;
    private MotionTrail motionTrail;
    
    public enum AttackType{None,L1, L2, R1, R2, DropKick, HurricaneKick}
    public AttackType attackType = AttackType.L1;

    private float cooldown_L1 = 0.5f;
    private float cooldown_L2 = 1.5f;
    private float cooldown_R1 = 1f;
    private float cooldown_R2 = 2f;
    private float cooldown_DropKick = 3f;
    private float cooldown_HurricaneKick = 5f;

    private bool canAttack_L1 = true;
    private bool canAttack_L2 = true;
    private bool canAttack_R1 = true;
    private bool canAttack_R2 = true;
    private bool canAttack_DropKick = true;
    private bool canAttack_HurricaneKick = true;

    private KickCollision _kickCollision;
    private void Awake()
    {
        PlayerInput Input = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody>();
        moveAction = Input.actions["Move"];
        runAction = Input.actions["Run"];
        jumpAction = Input.actions["Jump"];
        anim = GetComponent<Animator>();
        motionTrail = GetComponent<MotionTrail>();
        _kickCollision = GetComponentInChildren<KickCollision>();

    }

    private void Update()
    {
        if(isMove)
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

            if (isEPressed)
            {
                if (canAttack_HurricaneKick)
                {
                    attackType = AttackType.HurricaneKick;
                    anim.SetTrigger("HurricaneKick");
                    StartCoroutine(RotateHurricaneKick());
                    StartCoroutine(AttackCooldown(cooldown_HurricaneKick, attackType));
                }
            }
            else
            {
                if (canAttack_DropKick)
                {
                    attackType = AttackType.DropKick;
                    anim.SetTrigger("DropKick");
                    StartCoroutine(AttackCooldown(cooldown_DropKick, attackType));
                }
            }
            
            return;
        }

        if (leftClick && !rightClick && Input.GetMouseButtonDown(0))
        {
            if (isEPressed)
            {
                if (canAttack_L2)
                {
                    attackType = AttackType.L2;
                    anim.SetTrigger("Kick_L2");
                    StartCoroutine(AttackCooldown(cooldown_L2, attackType));
                }
            }
            else
            {
                if (canAttack_L1)
                {
                    attackType = AttackType.L1;
                    anim.SetTrigger("Kick_L1");
                    StartCoroutine(AttackCooldown(cooldown_L1, attackType));
                }
                
            }
        }

        if (!leftClick && rightClick && Input.GetMouseButtonDown(1))
        {
            if (isEPressed)
            {
                if (canAttack_R2)
                {
                    attackType = AttackType.R2;
                    anim.SetTrigger("Kick_R2");
                    StartCoroutine(AttackCooldown(cooldown_R2, attackType));
                }

            }
            else
            {
                if (canAttack_R1)
                {
                    attackType = AttackType.R1;
                    anim.SetTrigger("Kick_R1");
                    StartCoroutine(AttackCooldown(cooldown_R1, attackType));
                }

            }
        }
    }

    private IEnumerator AttackCooldown(float cooldown, AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.L1: canAttack_L1 = false; break;
            case AttackType.L2: canAttack_L2 = false; break;
            case AttackType.R1: canAttack_R1 = false; break;
            case AttackType.R2: canAttack_R2 = false; break;
            case AttackType.DropKick: canAttack_DropKick = false; break;
            case AttackType.HurricaneKick: canAttack_HurricaneKick = false; break;
        }

        float timer = cooldown;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            GameManager.Instance.inGamePanelController.UpdateCooldownUI(attackType, timer/cooldown);
            yield return null;
        }

        switch (attackType)
        {
            case AttackType.L1: canAttack_L1 = true; break;
            case AttackType.L2: canAttack_L2 = true; break;
            case AttackType.R1: canAttack_R1 = true; break;
            case AttackType.R2: canAttack_R2 = true; break;
            case AttackType.DropKick: canAttack_DropKick = true; break;
            case AttackType.HurricaneKick: canAttack_HurricaneKick = true; break;
        }
        
        GameManager.Instance.inGamePanelController.UpdateCooldownUI(attackType, 0);
    }

    IEnumerator RotateHurricaneKick()
    {
        isMove = false;
        float elapsedTime = 0f;
        float rotationSpeed = 1440f;
        while (elapsedTime < 1f)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isMove = true;
    }

    public void EnableAttackCollider(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.L1:
            case AttackType.L2:
                LeftFootCollider.enabled = true;
                break;
            case AttackType.DropKick:
                LeftFootCollider.enabled = true;
                RightFootCollider.enabled = true;
                break;
            case AttackType.R1:
            case AttackType.R2:
            case AttackType.HurricaneKick:
                RightFootCollider.enabled = true;
                break;
        }
    }

    public void disableAttackCollider()
    {
        LeftFootCollider.enabled = false;
        RightFootCollider.enabled = false;
        attackType = AttackType.None;
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
                Destroy(other.gameObject);
            }
        }
    }

    private void ApplyItemEffect(Item.ItemType itemType)
    {
        switch (itemType)
        {
            case Item.ItemType.SpeedUp :
                Debug.Log("스피드업");
                StartCoroutine(SpeedUpEffect());
                break;
            case Item.ItemType.TimeExtension :
                Debug.Log("시간추가");
                GameManager.Instance.inGamePanelController.AddTime(30f);
                break;
            case Item.ItemType.Magnet :
                Debug.Log("자석");
                StartCoroutine(MagnetEffect());
                break;
            case Item.ItemType.PowerUp :
                Debug.Log("파워업");
                StartCoroutine(PowerUpEffect());
                break;
            case Item.ItemType.SizeDown :
                Debug.Log("사이즈다운");
                transform.DOScale(transform.localScale * 0.9f, 1f);
                walkSpeed *= 0.9f;
                runSpeed *= 0.9f;
                cameraTransform.GetComponent<CameraFollow>().ChangeDistanceCamera(0.9f);
                break;
            case Item.ItemType.SizeUp :
                Debug.Log("사이즈업");
                transform.DOScale(transform.localScale * 1.2f, 1f);
                walkSpeed *= 1.2f;
                runSpeed *= 1.2f;
                cameraTransform.GetComponent<CameraFollow>().ChangeDistanceCamera(1.1f);
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
                if (destructible.CompareTag("Destructible"))
                {
                    destructible.transform.position = Vector3.MoveTowards(destructible.transform.position,
                        transform.position, magnetSpeed * Time.deltaTime);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PowerUpEffect()
    {
        _kickCollision.GetPowerItem(50);
        yield return new WaitForSeconds(2f);
    }
}
