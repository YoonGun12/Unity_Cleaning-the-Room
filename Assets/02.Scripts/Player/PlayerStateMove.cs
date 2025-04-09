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
        Vector2 input = _player.moveAction.ReadValue<Vector2>();

        if (input.sqrMagnitude < 0.01f)
        {
            _player.SetState(PlayerState.Idle);
            return;
        }

        Vector3 forward = _player.cameraTransform.forward;
        Vector3 right = _player.cameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * input.y + right * input.x;
        float speed = _player.runAction.IsPressed() ? _player.runSpeed : _player.walkSpeed;

        _player.rigid.velocity = new Vector3(moveDir.x * speed, _player.rigid.velocity.y, moveDir.z * speed);

        // 회전
        float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(
            _player.transform.eulerAngles.y,
            targetAngle,
            ref rotationVelocity,
            _player.rotationSmoothTime);

        _player.transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

        _player.anim.SetBool("isWalk", true);
        _player.anim.SetBool("isRun", _player.runAction.IsPressed());

        if (_player.jumpAction.triggered && _player.isGround)
        {
            _player.SetState(PlayerState.Jump);
        }
    }

    public void OnExit()
    {
        _player.anim.SetBool("isWalk", false);
        _player.anim.SetBool("isRun", false);
    }
}
