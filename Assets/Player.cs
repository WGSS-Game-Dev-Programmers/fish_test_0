using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private bool timeSlowed = false;
    [SerializeField] private float whatSpeed = 1;

    [Header("Character Attributes")]
    [SerializeField] private Transform _groundCheck;
    [HideInInspector] public LayerMask groundMask;

    private bool _slipping;
    private CharacterController _controller;
    private RaycastHit _slopeHit;
    private Vector2 _velocity;
    private Vector2 _directionalInput;
    
    readonly float r_moveSpeed = 6.5f;
    readonly float r_jumpHeight = 4;
    readonly float r_radius = 0.2f;
    readonly float r_gravity = -9.81f * 7f;
    readonly float r_edgeForce = 10;


    void Start()
    {
        _controller = GetComponent<CharacterController>();
        groundMask = 1 << LayerMask.NameToLayer("Ground");
    }
    void FixedUpdate()
    {
        IsTouchingRoof();

        ApplyGravity();
        if (!OnSlope())
        {
            EdgeSlide();
        }
    }
    private void ApplyGravity()
    {
        if (IsGrounded() && _velocity.y < 0)
        {
            _velocity.y = 0;
        }

        _velocity.y += r_gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (_slipping || (IsJumping() || IsFalling()) && CloseToWall()) return;
        _controller.Move(r_moveSpeed * Time.deltaTime * Vector3.ProjectOnPlane(_directionalInput, _slopeHit.normal));
    }

    // might revisit later in another project***
    bool CloseToWall()
    {
        if (Physics.Raycast(transform.position + Vector3.down, Vector3.right * _directionalInput.x, out RaycastHit hit, 1, groundMask))
        {
            Vector3 pointPlusSkin = new Vector3(hit.point.x - _controller.skinWidth * _directionalInput.x, transform.position.y - 1, transform.position.z);
            float distance = Vector3.Distance(transform.position + Vector3.down, pointPlusSkin);
            float inconsistancy = 0.05f;
            return distance <= _controller.radius + inconsistancy && distance >= _controller.radius - inconsistancy;
        }
        return false;
    }

    public bool IsTouchingRoof()
    {
        bool condition = Physics.Raycast(transform.position + Vector3.right * _controller.radius + Vector3.up, Vector3.up, 
            out _, _controller.skinWidth, groundMask);
        if (condition)
        {
            _velocity.y = -1;
        }
        return condition;
    }
    public bool IsGrounded()
    {
        bool grounded = Physics.CheckSphere(_groundCheck.position, r_radius, groundMask) && !IsJumping();
        return grounded;
    }
    bool OnSlope()
    {
        if (!IsGrounded()) return false;
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, 5))
        {
            if (_slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsJumping()
    {
        return _velocity.y > 0;
    }
    private bool IsFalling()
    {
        return !IsGrounded() && _velocity.y < 0.5f;
    }

    public void OnJump() => _velocity.y = Mathf.Sqrt(r_jumpHeight * -2 * r_gravity);

    public void SetDirectionalInput(Vector2 input) => _directionalInput = input;
    public void SetTimeScale() => Time.timeScale = timeSlowed ? Time.timeScale = whatSpeed : Time.timeScale = 1;
    private void EdgeSlide()
    {
        if (Physics.SphereCast(transform.position + _controller.center, _controller.radius - _controller.skinWidth, Vector3.down, out RaycastHit hitInfo, _controller.height, groundMask))
        {
            Vector3 _relativeHitPoint = hitInfo.point - (transform.position + _controller.center);
            if (_relativeHitPoint.magnitude < 0.93f)
            {
                _slipping = true;
                _relativeHitPoint = new Vector3(_relativeHitPoint.x, 0) / r_edgeForce;
                _controller.Move(-_relativeHitPoint);
            }
        }
        else
        {
            _slipping = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, r_radius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_groundCheck.position, r_radius);
    }
}
