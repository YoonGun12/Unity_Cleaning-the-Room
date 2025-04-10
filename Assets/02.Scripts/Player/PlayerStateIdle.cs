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
        Vector2 input = _player.MoveAction.ReadValue<Vector2>();
        if (input.sqrMagnitude > 0.01f)
        {
            _player.SetState(PlayerState.Move);
        }

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
    }
}
