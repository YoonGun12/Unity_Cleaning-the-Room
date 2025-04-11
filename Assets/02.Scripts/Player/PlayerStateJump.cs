using UnityEngine;

public class PlayerStateJump : IPlayerState
{
    private PlayerController _player;
    private bool isReachTop = false;
    public void OnEnter(PlayerController playerController)
    {
        _player = playerController;
        _player.anim.SetTrigger("Jump");
        _player.Rigid.AddForce(Vector3.up * _player.JumpPower, ForceMode.Impulse);
        _player.isGround = false;
        isReachTop = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
    }

    public void Update()
    {
        float rayLength = _player.transform.localScale.y * 0.2f;
        Debug.DrawRay(_player.PlayerPivot.position, Vector3.down *rayLength, Color.red);
        _player.isGround = Physics.Raycast(_player.PlayerPivot.position, Vector3.down, rayLength);
        _player.anim.SetBool("isGround", _player.isGround);
        
        if (_player.Rigid.velocity.y < 0 && !isReachTop)
        {
            isReachTop = true;
        }
        
        if (_player.isGround && _player.Rigid.velocity.y >= -0.1f && isReachTop)
        {
            _player.SetState(PlayerState.Idle);
        }
        
    }

    public void OnExit()
    {
        
    }
}
