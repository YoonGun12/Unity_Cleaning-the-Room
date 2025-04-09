using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle : IPlayerState
{
    private PlayerController _player;
    public void OnEnter(PlayerController playerController)
    {
        _player = playerController;
        _player.anim.SetBool("isWalk", false);
        _player.anim.SetBool("isRun", false);
    }

    public void Update()
    {
        Vector2 input = _player.moveAction.ReadValue<Vector2>();
        if (input.sqrMagnitude > 0.01f)
        {
            _player.SetState(PlayerState.Move);
        }

        if (_player.jumpAction.triggered && _player.isGround)
        {
            _player.SetState(PlayerState.Jump);
        }
    }

    public void OnExit()
    {
    }
}
