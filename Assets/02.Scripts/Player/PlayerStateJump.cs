using UnityEngine;

public class PlayerStateJump : IPlayerState
{
    private PlayerController _player;
    public void OnEnter(PlayerController playerController)
    {
        _player = playerController;
        _player.anim.SetTrigger("Jump");
        _player.rigid.AddForce(Vector3.up * _player.jumpPower, ForceMode.Impulse);
        _player.isGround = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
    }

    public void Update()
    {
        float rayLength = _player.transform.localScale.y * 0.2f;
        _player.isGround = Physics.Raycast(_player.playerPivot.position, Vector3.down, rayLength);
        _player.anim.SetBool("isGround", _player.isGround);

        if (_player.isGround && _player.rigid.velocity.y <= 0.1f)
        {
            _player.SetState(PlayerState.Idle);
        }
    }

    public void OnExit()
    {
        
    }
}
