﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerController2D : MonoBehaviour
{
    private CharacterController2D _controller;
    private SpriteRenderer _spriteRenderer;
    
    private Animator _animator;

    public float speed = 6.0f;
    public float maxJumpHeight = 4.0f;
    public float minJumpHeight = 1.0f;
    public float jumpTime = 0.4f;

    public float accelerationTimeAir = 0.2f;
    public float accelerationTimeGrounded = 0.1f;

    public float speedAcceleratorFactor = 2.0f;

    public float lives = 6.0f;
    public bool invulnerable;

    private float _gravity;
    private float _maxJumpVelocity;
    private float _minJumpVelocity;
    
    private Vector2 _velocity;
    private float _velocityXSmoothing;

    private bool _doubleJump;
    public bool _dead;

    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        CalculateGravity();
    }
    
    void Update()
    {
        var color = _spriteRenderer.color;
        if (_dead)
        {
            color.a = 1;
            _spriteRenderer.color = color;
            return;
        }

        if (lives == 0)
        {
            _animator.SetBool("dead", true);
            invulnerable = false;
            _dead = true;
            return;
        }

        _animator.SetBool("dead", false);

        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        var scale = _spriteRenderer.transform.localScale;
        scale.x = _controller._facingDirection >= 0 ? 1 : -1;
        _spriteRenderer.transform.localScale = scale;

        color.a = invulnerable && color.a == 1 ? 0 : 1;
        _spriteRenderer.color = color;

        _animator.SetBool("walking", input.x != 0);
        _animator.SetBool("jumping", _velocity.y != 0);

        if (Input.GetButtonDown("Jump") && !_doubleJump)
        {
            _doubleJump = !_controller.collisions.below;

            if (_controller.collisions.slidingDownMaxSlope)
            {
                // not jumping against the max slope
                if (Mathf.Sign(input.x) != -Mathf.Sign(_controller.collisions.slopeNormal.x))
                {
                    _velocity.x = _maxJumpVelocity * _controller.collisions.slopeNormal.x;
                    _velocity.y = _maxJumpVelocity * _controller.collisions.slopeNormal.y;
                }
            }
            else
            {
                _velocity.y = _maxJumpVelocity;
            }

            AudioManager.Play(GameAudioClip.Jump);
        }

        if (Input.GetButtonUp("Jump"))
        {
            if(_velocity.y > _minJumpVelocity)
            {
                _velocity.y = _minJumpVelocity;
            }
        }

        var targetVelocityX = input.x * speed;

        if (Input.GetButton("Fire3"))
        {
            _animator.SetBool("running", input.x != 0);
            targetVelocityX *= speedAcceleratorFactor;
        }
        else
        {
            _animator.SetBool("running", false);
        }

        var smoothTime = _controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAir;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, smoothTime);

        _velocity.y += _gravity * Time.deltaTime;

        _controller.Move(_velocity * Time.deltaTime, input);

        if (_controller.collisions.above || _controller.collisions.below)
        {
            if (_controller.collisions.slidingDownMaxSlope)
            {
                _velocity.y += _controller.collisions.slopeNormal.y * -_gravity * Time.deltaTime;
            }
            else
            {
                _velocity.y = 0;
            }

            if (_controller.collisions.below)
            {
                _doubleJump = false;
            }
        }
        
        PrintDebugInfo(input);
    }

    public void CalculateGravity()
    {
        _gravity = (-2 * maxJumpHeight) / (jumpTime * jumpTime);
        _maxJumpVelocity = Mathf.Abs(_gravity) * jumpTime;
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(_gravity) * minJumpHeight);

        Debug.Log("Gravity: " + _gravity);
        Debug.Log("Jump velocity: " + _maxJumpVelocity);
    }
    
    private void PrintDebugInfo(Vector2 input)
    {
        var debugInfo = new StringBuilder();
        debugInfo.AppendLine("Player Info");

        debugInfo.AppendFormat("input: {0}", input);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("velocity: {0}", _velocity);
        debugInfo.AppendLine();

        var collisions = new List<string>();
        if (_controller.collisions.left) collisions.Add("left");
        if (_controller.collisions.above) collisions.Add("above");
        if (_controller.collisions.right) collisions.Add("right");
        if (_controller.collisions.below) collisions.Add("below");
        if (collisions.Count == 0) collisions.Add("none");

        debugInfo.AppendFormat("collisions: {0}", string.Join(", ", collisions.ToArray()));
        debugInfo.AppendLine();

        debugInfo.AppendFormat("climbing slope: {0}", _controller.collisions.climbingSlope);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("descending slope: {0}", _controller.collisions.descendingSlope);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("sliding slope: {0}", _controller.collisions.slidingDownMaxSlope);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("slope angle: {0}", _controller.collisions.slopeAngle);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("slope normal: {0}", _controller.collisions.slopeNormal);
        debugInfo.AppendLine();

        debugInfo.AppendFormat("falling through: {0}", _controller.collisions.fallingThroughPlatform);
        debugInfo.AppendLine();

        DebugEx.ClearStatic();
        DebugEx.LogStatic(debugInfo.ToString());
    }
}
