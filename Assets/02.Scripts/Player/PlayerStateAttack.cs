using System.Collections;
using UnityEngine;

public class PlayerStateAttack : IPlayerState
{
    private PlayerController _player;
    
    private bool leftClick;
    private bool rightClick;
    private float leftClickTime;
    private float rightClickTime;
    private float doubleClickThreshold = 0.1f;

    private bool canAttack_L1 = true;
    private bool canAttack_L2 = true;
    private bool canAttack_R1 = true;
    private bool canAttack_R2 = true;
    private bool canAttack_DropKick = true;
    private bool canAttack_HurricaneKick = true;

    private float cooldown_L1 = 0.5f;
    private float cooldown_L2 = 1.5f;
    private float cooldown_R1 = 1f;
    private float cooldown_R2 = 2f;
    private float cooldown_DropKick = 3f;
    private float cooldown_HurricaneKick = 5f;
    
   public void OnEnter(PlayerController playerController)
    {
        _player = playerController;
        // 초기화 (예: 상태 전환 시 공격 입력 재확인 등)
    }

    public void Update()
    {
        bool isEPressed = Input.GetKey(KeyCode.E);

        if (Input.GetMouseButtonDown(0))
        {
            leftClick = true;
            leftClickTime = Time.time;
            _player.StartCoroutine(ResetClick("left"));
        }
        if (Input.GetMouseButtonDown(1))
        {
            rightClick = true;
            rightClickTime = Time.time;
            _player.StartCoroutine(ResetClick("right"));
        }

        if (leftClick && rightClick && Mathf.Abs(leftClickTime - rightClickTime) <= doubleClickThreshold)
        {
            leftClick = rightClick = false;
            if (isEPressed && canAttack_HurricaneKick)
            {
                ExecuteAttack(PlayerController.AttackType.HurricaneKick, "HurricaneKick", cooldown_HurricaneKick);
                _player.StartCoroutine(RotateHurricaneKick());
            }
            else if (canAttack_DropKick)
            {
                ExecuteAttack(PlayerController.AttackType.DropKick, "DropKick", cooldown_DropKick);
            }

            return;
        }

        if (leftClick && !rightClick && Input.GetMouseButtonDown(0))
        {
            if (isEPressed && canAttack_L2)
                ExecuteAttack(PlayerController.AttackType.L2, "Kick_L2", cooldown_L2);
            else if (canAttack_L1)
                ExecuteAttack(PlayerController.AttackType.L1, "Kick_L1", cooldown_L1);
        }

        if (!leftClick && rightClick && Input.GetMouseButtonDown(1))
        {
            if (isEPressed && canAttack_R2)
                ExecuteAttack(PlayerController.AttackType.R2, "Kick_R2", cooldown_R2);
            else if (canAttack_R1)
                ExecuteAttack(PlayerController.AttackType.R1, "Kick_R1", cooldown_R1);
        }
    }

    private void ExecuteAttack(PlayerController.AttackType type, string triggerName, float cooldown)
    {
        _player.attackType = type;
        _player.anim.SetTrigger(triggerName);
        _player.StartCoroutine(AttackCooldown(cooldown, type));
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Kick);
    }

    private IEnumerator AttackCooldown(float cooldown, PlayerController.AttackType attackType)
    {
        SetAttackAvailability(attackType, false);

        float timer = cooldown;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            GameManager.Instance.inGamePanelController.UpdateCooldownUI(attackType, timer / cooldown);
            yield return null;
        }

        SetAttackAvailability(attackType, true);
        GameManager.Instance.inGamePanelController.UpdateCooldownUI(attackType, 0);
    }

    private void SetAttackAvailability(PlayerController.AttackType type, bool canAttack)
    {
        switch (type)
        {
            case PlayerController.AttackType.L1: canAttack_L1 = canAttack; break;
            case PlayerController.AttackType.L2: canAttack_L2 = canAttack; break;
            case PlayerController.AttackType.R1: canAttack_R1 = canAttack; break;
            case PlayerController.AttackType.R2: canAttack_R2 = canAttack; break;
            case PlayerController.AttackType.DropKick: canAttack_DropKick = canAttack; break;
            case PlayerController.AttackType.HurricaneKick: canAttack_HurricaneKick = canAttack; break;
        }
    }

    private IEnumerator ResetClick(string button)
    {
        yield return new WaitForSeconds(doubleClickThreshold);
        if (button == "left") leftClick = false;
        else if (button == "right") rightClick = false;
    }

    private IEnumerator RotateHurricaneKick()
    {
        _player._isMove = false;
        float elapsedTime = 0f;
        float rotationSpeed = 1440f;
        while (elapsedTime < 1f)
        {
            _player.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _player.DisableAttackCollider();
        _player._isMove = true;
    }

    public void OnExit()
    {
        _player.DisableAttackCollider();
    }
    
}
