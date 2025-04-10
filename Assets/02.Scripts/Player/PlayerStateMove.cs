using UnityEngine;

public class PlayerStateMove : IPlayerState
{
    private PlayerController _player;
    private float rotationVelocity;
    
    public void OnEnter(PlayerController playerController)
    {
        _player = playerController;
    }

    public void Update()
    {
        Vector2 input = _player.MoveAction.ReadValue<Vector2>();

        if (input.sqrMagnitude < 0.01f)
        {
            _player.SetState(PlayerState.Idle);
            return;
        }

        Vector3 forward = _player.CameraTransform.forward;
        Vector3 right = _player.CameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * input.y + right * input.x;
        float speed = _player.RunAction.IsPressed() ? _player.RunSpeed : _player.WalkSpeed;

        _player.Rigid.velocity = new Vector3(moveDir.x * speed, _player.Rigid.velocity.y, moveDir.z * speed);

        // 회전
        float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(
            _player.transform.eulerAngles.y,
            targetAngle,
            ref rotationVelocity,
            _player.RotationSmoothTime);

        _player.transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

        _player.anim.SetBool("isWalk", true);
        _player.anim.SetBool("isRun", _player.RunAction.IsPressed());

        if (_player.JumpAction.triggered && _player.isGround)
        {
            Debug.Log("점프 키 및 isground 참!");
            _player.SetState(PlayerState.Jump);
        }
        
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            _player.SetState(PlayerState.Attack);
        }
    }

    public void OnExit()
    {
        _player.anim.SetBool("isWalk", false);
        _player.anim.SetBool("isRun", false);
    }
}
