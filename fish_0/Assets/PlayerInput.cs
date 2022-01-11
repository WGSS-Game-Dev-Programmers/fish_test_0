using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Player _player;

    void Start() => _player = GetComponent<Player>();

    void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);

        if (Physics.Raycast(transform.position + Vector3.right / 4, Vector3.down, out _, 0.5f * directionalInput.x, _player.groundMask))
        {
            directionalInput.x = 0;
        }

        _player.SetDirectionalInput(directionalInput);

        _player.SetTimeScale();

        if (Input.GetKeyDown(KeyCode.Space) && _player.IsGrounded())
            _player.OnJump();
    }
}
