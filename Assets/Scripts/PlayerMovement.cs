using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float speed = 6f;
    private float jumpingPower = 12f;
    private bool isFacingRight = true;

    private float coyoteTime = 0.07f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private float wallJumpBufferTime = 0.1f;
    private float wallJumpBufferCounter;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2.5f;

    private bool isJumping;
    private float jumpingDuration = 0.1f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.07f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.125f;
    private Vector2 wallJumpingPower = new Vector2(6f, 12f);

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        Jump();
        if (!isWallJumping)
        {
            Flip();
        }
        WallSlide();
        if(!isJumping)
        {
            WallJump();
        }
    }
    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }
    private bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, groundLayer);
    }
    private void WallSlide()
    {
        if(isWalled() && !isGrounded() && horizontal != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void Jump()
    {
        if(isGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            jumpBufferCounter = 0f;
            Invoke(nameof(StopJumping), jumpingDuration);
        }
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    private void WallJump()
    {
        if(Input.GetButtonDown("Jump"))
        {
            wallJumpBufferCounter = wallJumpBufferTime;
        }
        else
        {
            wallJumpBufferCounter -= Time.deltaTime;
        }

        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        if(wallJumpBufferCounter > 0f && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpBufferCounter = 0f;
            wallJumpingCounter = 0f;
            if(transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
            }
            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    private void StopJumping()
    {
        isJumping = false;
    }
     
    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x;
            transform.localScale = localScale;
        }
    }
}
