using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    None, Idle, Move, Jump, Attack
}

[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(MotionTrail))]
public class PlayerController : MonoBehaviour
{
    [Header("플레이어 이동")]
    [SerializeField] private float walkSpeed; 
    [SerializeField] private float runSpeed; 
    [SerializeField] private float jumpPower; 
    [SerializeField] private Transform playerPivot;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField]private float damageMultiplier = 1f;

    [Header("카메라")] 
    [SerializeField] private Transform cameraTransform;
    
    [Header("공격 콜라이더")] 
    [SerializeField] private Collider leftFootCollider;
    [SerializeField] private Collider rightFootCollider;
    
    public Animator anim { get; private set; }
    private Rigidbody rigid;
    private MotionTrail _motionTrail;
    private KickCollision _kickCollision;
    
    public enum AttackType{None,L1, L2, R1, R2, DropKick, HurricaneKick}
    public AttackType attackType = AttackType.L1;
    
    public bool isGround;
    public bool _isMove = true;
    
    
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;

    private PlayerInput _input;

    #region 읽기 전용 프로퍼티

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;
    public float JumpPower => jumpPower;
    public Transform PlayerPivot => playerPivot;
    public float RotationSmoothTime => rotationSmoothTime;
    public Transform CameraTransform => cameraTransform;
    
    public Rigidbody Rigid => rigid;
    
    public float DamageMultiplier => damageMultiplier;
    
    public InputAction MoveAction => moveAction;
    public InputAction RunAction => runAction;
    public InputAction JumpAction => jumpAction;

    

    //상태관련
    private PlayerStateIdle _playerStateIdle;
    private PlayerStateMove _playerStateMove;
    private PlayerStateJump _playerStateJump;
    private PlayerStateAttack _playerStateAttack;
    
    public PlayerState CurrentState { get; private set; }
    private Dictionary<PlayerState, IPlayerState> _playerStates;
    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInput>();
        _motionTrail = GetComponent<MotionTrail>();
        _kickCollision = GetComponent<KickCollision>();

        moveAction = _input.actions["Move"];
        runAction = _input.actions["Run"];
        jumpAction = _input.actions["Jump"];
    }

    private void Start()
    {
        //상태 초기화
        _playerStateIdle = new PlayerStateIdle();
        _playerStateMove = new PlayerStateMove();
        _playerStateJump = new PlayerStateJump();
        _playerStateAttack = new PlayerStateAttack();

        _playerStates = new Dictionary<PlayerState, IPlayerState>
        {
            { PlayerState.Idle, _playerStateIdle },
            { PlayerState.Move, _playerStateMove },
            { PlayerState.Jump, _playerStateJump },
            { PlayerState.Attack, _playerStateAttack }
        };
        
        isGround = Physics.Raycast(PlayerPivot.position, Vector3.down, transform.localScale.y * 0.2f);
        SetState(PlayerState.Idle);
        
    }

    private void Update()
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].Update();
        }
    }

    public void SetState(PlayerState state)
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].OnExit();
        }

        CurrentState = state;
        _playerStates[CurrentState].OnEnter(this);
    }

    #region 공격 이벤트

    public void EnableAttackCollider(AttackType type)
    {
        switch (type)
        {
            case AttackType.L1:
            case AttackType.L2:
                leftFootCollider.enabled = true;
                break;
            case AttackType.R1:
            case AttackType.R2:
            case AttackType.HurricaneKick:
                rightFootCollider.enabled = true;
                break;
            case AttackType.DropKick:
                leftFootCollider.enabled = true;
                rightFootCollider.enabled = true;
                break;
        }
    }
    
    public void DisableAttackCollider()
    {
        leftFootCollider.enabled = false;
        rightFootCollider.enabled = false;
        attackType = AttackType.None;
    }

    #endregion

    #region 애니메이션 이벤트

    private void OnLand(AnimationEvent animationEvent)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Landing);
    }

    private void OnFootstep()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.FootStep);
    }

    #endregion

    #region 아이템

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
            case Item.ItemType.SpeedUp:
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Speedup);
                StartCoroutine(SpeedUpEffect());
                break;
            case Item.ItemType.TimeExtension:
                GameManager.Instance.inGamePanelController.AddTime(20f);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.AddTime);
                break;
            case Item.ItemType.Magnet:
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Magnet);
                StartCoroutine(MagnetEffect());
                break;
            case Item.ItemType.PowerUp:
                AudioManager.instance.PlaySfx(AudioManager.Sfx.PowerUp);
                StartCoroutine(PowerUpEffect());
                break;
            case Item.ItemType.SizeDown:
                AudioManager.instance.PlaySfx(AudioManager.Sfx.SizeDown);
                ChangeSize(0.8f);
                break;
            case Item.ItemType.SizeUp:
                AudioManager.instance.PlaySfx(AudioManager.Sfx.SizeUp);
                ChangeSize(1.5f);
                break;
        }
    }

    IEnumerator SpeedUpEffect()
    {
        if (_motionTrail == null) yield break;

        _motionTrail.StartMotionTrail();
        float originalWalk = walkSpeed;
        float originalRun = runSpeed;

        walkSpeed *= 1.5f;
        runSpeed *= 1.5f;

        yield return new WaitForSeconds(5f);

        walkSpeed = originalWalk;
        runSpeed = originalRun;
    }

    IEnumerator MagnetEffect()
    {
        var magnetRadius = 7f;
        var magnetSpeed = 3f;
        var elapsedTime = 0f;

        while (elapsedTime < 5f)
        {
            Collider[] destructibles = Physics.OverlapSphere(transform.position, magnetRadius);
            foreach (var destructible in destructibles)
            {
                if (destructible.CompareTag("Destructible"))
                {
                    destructible.transform.position = Vector3.MoveTowards(
                        destructible.transform.position,
                        transform.position,
                        magnetSpeed * Time.deltaTime);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PowerUpEffect()
    {
        damageMultiplier = 2f;
        yield return new WaitForSeconds(5f);
        damageMultiplier = 1f;
    }

    private void ChangeSize(float scaleMultiplier)
    {
        Vector3 newScale = transform.localScale * scaleMultiplier;

        newScale.x = Mathf.Clamp(newScale.x, 0.5f, 5f);
        newScale.y = Mathf.Clamp(newScale.y, 0.5f, 5f);
        newScale.z = Mathf.Clamp(newScale.z, 0.5f, 5f);

        transform.DOScale(newScale, 1f);

        walkSpeed *= scaleMultiplier;
        runSpeed *= scaleMultiplier;
        jumpPower *= scaleMultiplier;

        walkSpeed = Mathf.Clamp(walkSpeed, 1.5f, 7.5f);
        runSpeed = Mathf.Clamp(runSpeed, 3.5f, 16.5f);
        jumpPower = Mathf.Clamp(jumpPower, 40f, 120f);

        cameraTransform.GetComponent<CameraFollow>()?.ChangeDistanceCamera(newScale.x);
    }

    #endregion
    
}
