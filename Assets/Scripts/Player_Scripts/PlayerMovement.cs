using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System.IO;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    private float baseSpeed = 5f;
    private float speed;
    private float jumpingPower = 10f;
    private float lowJumpingPower = 6f;
    private bool isFacingRight = true;

    // Coyote Time

    private float coyoteTime = 0.07f;
    private float coyoteTimeCounter;

    // Jump Buffer

    private float jumpBufferTime = 0.05f;
    private float jumpBufferCounter;

    // Wall Jump Buffer

    private float wallJumpBufferTime = 0.05f;
    private float wallJumpBufferCounter;

    // Wall Slide

    private float wallSlidingSpeed = 2.5f;

    // Jump

    private bool jumped = false;
    private bool isJumping = false;
    private int dynamicJumpAmplitudeCount = 3;
    private float originalDynamicJumpInterval = 0.03f;
    private float initialDynamicJumpInterval = 0.066f;
    private float dynamicJumpInterval;

    // Wall Jump

    private bool canWallJump;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.07f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.08f;
    private bool canWallBounce;
    private Vector2 wallJumpingPower = new Vector2(4f, 10f);

    // Dash

    private int baseDashAmount = 1;
    private int dashAmount;
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 16f;
    private float dashingTime = 0.18f;
    private float dashingCoolDown = 0.1f;

    // Gravity

    private float originalGravity;

    // Spring

    private bool gotSpringed = false;
    private float springedDuration = 0.25f;
    private float springedCounter;

    // Coroutines

    private IEnumerator DynamicJumpCoroutine;
    private IEnumerator DashCoroutine;
    private IEnumerator RespawnCameraShit;

    // respawn

    public bool dead;
    public bool startedDieing;
    private Vector2 respawnPosition;
    private GameObject parent;
    private GameObject CMCamera;

    private CinemachineConfiner2D CMConfiner;
    private CinemachineVirtualCamera CMVCamera;

    // Force Zones

    private bool isInForceZone;
    private float forceZoneForceX;
    private float forceZoneForceY;

    // Coin Shit

    private bool isInCoinBlockZone;
     
    // Ice Garbage

    private bool isIced;
    private float baseGroundedSpeedCap = 1.05f;
    private float iceGroundedSpeedCap = 1.4f;
    private float groundedSpeedcap;

    private bool isWallIced;

    // Wind

    private float X_wind_speed = 0;
    private float Y_wind_speed = 0;
    private float X_wind_speed_direction = 0;

    //

    private SpriteRenderer spriteRenderer;

    // Stuff

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Animator anim;

    [SerializeField] private PlatformGroundCheck platformGroundCheck;
    [SerializeField] private Transform groundCheck1;
    [SerializeField] private Transform groundCheck2;

    [SerializeField] private PlatformWallCheck platformWallCheck;
    [SerializeField] private Transform wallCheck1;
    [SerializeField] private Transform wallCheck2;

    [SerializeField] private Transform platformAnticheck1;
    [SerializeField] private Transform platformAnticheck2;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask IceLayer;

    [SerializeField] private TrailRenderer tr;

    [SerializeField] private InputManager inputManager;

    [SerializeField] private ParticleSystem JumpParticles;

    [SerializeField] private Material NoDashColorChangeMaterial;

    //

    private bool CameraReSet;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        X_wind_speed = 0;
        Y_wind_speed = 0;
        X_wind_speed_direction = 0;
        isInCoinBlockZone = false;
        CameraReSet = false;
        canWallBounce = false;
        dead = true; //git gud
        startedDieing = false;
        CoinFollower.PlayerDied = false;
        respawnPosition = transform.position;
        springedCounter = springedDuration;
        originalGravity = rb.gravityScale;
        dynamicJumpInterval = originalDynamicJumpInterval;
        DynamicJumpCoroutine = DynamicJump();
        DashCoroutine = Dash();
        RespawnCameraShit = DeathCameraLockOn();
    }
    private void Update()
    {
        //Debug.Log(X_wind_speed);
        if (!CameraReSet)
        {
            try
            {
                if (transform.parent.gameObject != null)
                {
                    StartCoroutine(RespawnCameraShit);
                    CameraReSet = true;
                }
            }
            catch (Exception) { }
        }
        //too lazy to change
        speed = baseSpeed;

        isGroundedBool = isGrounded();
        if (isIced)
        {
            groundedSpeedcap = iceGroundedSpeedCap;
        }
        else
        {
            groundedSpeedcap = baseGroundedSpeedCap;
        }
        horizontal = inputManager.HorizontalInput();
        vertical = inputManager.VerticalInput();
        if (gotSpringed)
        {
            springedCounter -= Time.deltaTime;
            if (springedCounter <= 0)
            {
                gotSpringed = false;
                springedCounter = springedDuration;
            }
        }
        Jump();
        WallSlide();
        if (!isJumping)
        {
            WallJump();
        }
        if (isDashing)
        {
            SpriteUpdate();
            return;
        }
        if (!isWallJumping)
        {
            Flip();
        }
        foreach (KeyCode key in inputManager.inputObject.Dash)
        {
            if (Input.GetKeyDown(key) && canDash && dashAmount > 0)
            {
                DashSpriteUpdated = false;
                StartCoroutine(DashCoroutine);
            }
        }
        SpriteUpdate();
    }

    private int AnimatorState = -1;
    private bool DashSpriteUpdated;

    private bool AnimIsIdle = false;
    private bool AnimWallSlide = false;
    private bool AnimIsWalking = false;
    private bool AnimIsAirborne = false;
    private bool AnimIsDashing = false;

    private float AnimYVelocity;

    [SerializeField] private Material SpriteDefaultMaterial;

    private void SpriteUpdate()
    {
        // Dash Color Swap
        if(!DashSpriteUpdated)
        {
            if (!canDash || dashAmount <= 0)
            {
                if(spriteRenderer.material != NoDashColorChangeMaterial)
                {
                    spriteRenderer.material = NoDashColorChangeMaterial;
                }
            }
            else
            {
                spriteRenderer.material = SpriteDefaultMaterial;
            }
            DashSpriteUpdated = true;
        }
        // script start reset
        if(AnimatorState == -1)
        {
            anim.Rebind();
            AnimatorState = 0;
        }
        // Airborne check
        if (isDashing)
        {
            if (!AnimIsDashing)
            {
                //spriteRenderer.material = NoDashColorChangeMaterial;
                anim.SetTrigger("DashTrigger");
                AnimIsDashing = true;
            }
        }
        else
        {
            if (AnimIsDashing)
            {
                anim.ResetTrigger("DashTrigger");
            }
            AnimIsDashing = false;
        }
        if (!isGroundedBool && !isWallSliding && !AnimIsDashing)
        {
            anim.SetBool("isAirborne", true);
            AnimIsAirborne = true;
        }
        else
        {
            anim.SetBool("isAirborne", false);
            AnimIsAirborne = false;
        }
        // Set Y Velocity
        AnimYVelocity = rb.velocity.y;
        anim.SetFloat("Y_Velocity", AnimYVelocity);
        //
        if(horizontal != 0 && !AnimIsAirborne && !AnimIsDashing && Mathf.Abs(AnimYVelocity) < 0.05)
        {
            if (!AnimIsWalking)
            {
                DashSpriteUpdated = false;
                anim.SetTrigger("isWalking");
                AnimIsWalking = true;

            }
        }
        else
        {
            if (AnimIsWalking)
            {
                anim.ResetTrigger("isWalking");
            }
            AnimIsWalking = false;
        }
        if (isWallSliding && !AnimIsDashing)
        {
            if (!AnimWallSlide)
            {
                anim.SetTrigger("WallSlide");
                AnimWallSlide = true;
            }
        }
        else
        {
            if(AnimWallSlide)
            {
                anim.ResetTrigger("WallSlide");
            }
            AnimWallSlide = false;
        }
        if(!AnimWallSlide && !AnimIsWalking && !AnimIsAirborne && !AnimIsDashing && Mathf.Abs(AnimYVelocity) < 0.05)
        {
            if(!AnimIsIdle)
            {
                DashSpriteUpdated = false;
                anim.SetTrigger("isIdle");
                AnimIsIdle = true;
            }
        }
        else
        {
            if(AnimIsIdle)
            {
                anim.ResetTrigger("isIdle");
            }
            AnimIsIdle = false;
        }
    }
    private void AnimatorDie()
    {
        anim.SetBool("isDead", true);
    }
    private void AnimatorLive()
    {
        anim.SetBool("isDead", false);
    }

    // Triggered in animator - wallSlide
    private void SpawnWallSlideParticles()
    {

    }

    public void SetWindSpeed(float newXWindSpeed, float newYWindSpeed)
    {
        X_wind_speed = newXWindSpeed;
        Y_wind_speed = newYWindSpeed;

        X_wind_speed_direction = X_wind_speed / Mathf.Abs(X_wind_speed);
    }
    public bool isXWinded()
    {
        if (X_wind_speed == 0)
        {
            return false;
        }
        return true;
    }
    public bool isYWinded()
    {
        if (Y_wind_speed == 0)
        {
            return false;
        }
        return true;
    }

    bool isGroundedBool;
    float velocityX;
    float direction;


    private void FixedUpdate()
    {
        if (isDashing || isWallJumping || gotSpringed || rb.bodyType == RigidbodyType2D.Static)
        {
            return;
        }
        rb.velocity = new Vector2(rb.velocity.x - X_wind_speed, rb.velocity.y - Y_wind_speed);
        // put here so the "isIced" variable is updated
        isGroundedBool = isGrounded();

        velocityX = rb.velocity.x;
        direction = velocityX / Mathf.Abs(velocityX);
        // Horiontal movement
        if (horizontal == 0 && Mathf.Abs(velocityX) > 0 && !gotSpringed)
        {
            if (isIced && Mathf.Abs(velocityX) < 1.5f && !isXWinded())
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else if (!isIced && Mathf.Abs(velocityX) < 3 && !isXWinded())
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else if (!isIced) 
            {
                rb.velocity -= new Vector2(direction * (speed / 2f), 0f);
            }
        }
        else if (Mathf.Abs(velocityX) < (speed * 0.925f * groundedSpeedcap) || direction == -horizontal)
        {
            if (isIced)
            {
                rb.velocity += new Vector2((horizontal * (speed / 24f)), 0);
            }
            else
            {
                rb.velocity += new Vector2((horizontal * (speed / 4f)), 0);
            }
        }
        //groundedSpeedcap makes sure you dont go too fast while walking 
        //which cound happen when using acceleration based movement logic (like me)
        else if (Mathf.Abs(velocityX) > (speed * groundedSpeedcap))
        {
            if (isGroundedBool && !isIced)
            {
                rb.velocity -= new Vector2(horizontal * (speed / 2f) , 0);
            }
            else if (Mathf.Abs(velocityX) > (dashingPower / 9f) * 8f)
            {
                if (isXWinded() && horizontal == -X_wind_speed_direction)
                {
                    rb.velocity -= new Vector2(horizontal * (speed / 2f), 0);
                }
                else
                {
                    rb.velocity -= new Vector2(horizontal * (speed / 24f), 0);
                }
            }
            else
            {
                rb.velocity -= new Vector2(horizontal * (speed / 8f), 0);
            }
        }
        // Vertical movement
        if (vertical < 0 && rb.velocity.y <= -15f)
        {
            rb.velocity = new Vector2(rb.velocity.x, -20f);
        }
        else if (rb.velocity.y < -12.5f)
        {
            rb.velocity = new Vector2(rb.velocity.x, -15f);
        }
        //force zone stuff
        if (isInForceZone)
        {
            rb.AddForce(new Vector2(forceZoneForceX, forceZoneForceY));
        }
        rb.velocity = new Vector2(rb.velocity.x + X_wind_speed, rb.velocity.y + Y_wind_speed);
    }
    public void CoinBlock()
    {
        isInCoinBlockZone = true;
    }
    public void CoinUnBlock()
    {
        isInCoinBlockZone = false;
    }

    public bool CanCollectCoin()
    {
        if (!isInCoinBlockZone && isGrounded())
        {
            return true;
        }
        return false;
    }

    public void GetSpringed()
    {
    
        gotSpringed = true;
    }

    public bool isGrounded()
    {
        if (rb.velocity.y <= 0.01 && rb.bodyType != RigidbodyType2D.Static)
        {
            isIced = false;
            if (Physics2D.OverlapArea(groundCheck1.position, groundCheck2.position, groundLayer))
            {
                //Player is On Ground
                return true;
            }
            if (Physics2D.OverlapArea(groundCheck1.position, groundCheck2.position, IceLayer))
            {
                //Player is On Ice
                isIced = true;
                return true;
            }
            if (Physics2D.OverlapArea(platformAnticheck1.position, platformAnticheck2.position, platformLayer))
            {
                //platform is inside player
                return false;
            }
            //player is on platform
            return platformGroundCheck.IsItGround();
        }
        //player is airborne
        return false;
    }

    private bool isWallSliding;

    public bool isWalled()
    {
        isWallIced = false;
        //player is next to a wall
        if(Physics2D.OverlapArea(wallCheck1.position, wallCheck2.position, groundLayer))
        { 
            return true; 
        }
        if (Physics2D.OverlapArea(wallCheck1.position, wallCheck2.position, IceLayer))
        {
            isWallIced = true;
            return true;
        }
        if (platformWallCheck.GetPlatformRotation() == transform.localScale.x)
        {
            return true;
        }

        return false;
    }
    private bool isWalledBool;
    private void WallSlide()
    {
        isWallSliding = false;
        isWalledBool = isWalled();
        if (isWalledBool && !isGrounded())
        {
            canWallJump = true;
            if (horizontal != 0)
            {
                if (Math.Abs(rb.velocity.x) < 0.01 && rb.bodyType != RigidbodyType2D.Static && !isWallIced)
                {
                    isWallSliding = true;
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
                }
            }
        }
        else
        {
            canWallJump = false;
        }
    }

    private void Jump()
    {
        if (Math.Abs(rb.velocity.y) < 0.5 && rb.gravityScale == originalGravity)
        {
            rb.gravityScale = originalGravity / 2;
        }
        else if (Math.Abs(rb.velocity.y) > 0.5)
        {
            rb.gravityScale = originalGravity;
        }
        {

        }
        if (Time.timeScale == 0)
        {
            dashAmount = baseDashAmount;
        }
        if (isGrounded())
        {
            //reset jump values when player is grounded
            jumped = false;
            coyoteTimeCounter = coyoteTime;
            dashAmount = baseDashAmount;
            StopCoroutine(DynamicJumpCoroutine);
            dynamicJumpInterval = originalDynamicJumpInterval;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        foreach (KeyCode key in inputManager.inputObject.Jump)
        {
            if (Input.GetKeyDown(key))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }
        }
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
        {
            //jumping logic
            JumpParticles.GetComponent<ParticleSystemRenderer>().flip = transform.localScale;
            JumpParticles.Play();
            jumped = true;
            isJumping = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            StartCoroutine(DynamicJumpCoroutine);
            jumpBufferCounter = 0f;
            if(isDashing)
            {
                if (horizontal != 0)
                {
                    rb.velocity = new Vector2(horizontal * Mathf.Abs(rb.velocity.x), rb.velocity.y);
                }
                //nerf height during a dash-jump
                dynamicJumpInterval /= 2;
                rb.gravityScale = originalGravity;
            }
        }
        if(isJumping && rb.velocity.y < 0f)
        {
            StopJumping();
        }
        foreach (KeyCode key in inputManager.inputObject.Jump)
        {
            if ((Input.GetKeyUp(key) && rb.velocity.y > 0f) || (isJumping && Mathf.Abs(rb.velocity.y) < 0.05f))
            {
                coyoteTimeCounter = 0f;
                StopCoroutine(DynamicJumpCoroutine);
                if (rb.velocity.y > 6f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, lowJumpingPower);
                }
                DynamicJumpCoroutine = DynamicJump();
                StopJumping();
            }
        }
    }

    private void WallJump()
    {
        foreach (KeyCode key in inputManager.inputObject.Jump)
        {
            if (Input.GetKeyDown(key))
            {
                wallJumpBufferCounter = wallJumpBufferTime;
            }
            else
            {
                wallJumpBufferCounter -= Time.deltaTime;
            }
        }
        if (canWallJump)
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
            ExecuteWallJump();
        }
    }
    private void ExecuteWallJump()
    {
        isWallJumping = true;
        // Dash bounce stopped working so I added this
        float HelperThingo = 1;
        if(canWallBounce)
        {
            HelperThingo = 1.33f;
        }
        canWallBounce = false;
        rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x * HelperThingo, wallJumpingPower.y * HelperThingo);
        wallJumpBufferCounter = 0f;
        wallJumpingCounter = 0f;
        if (transform.localScale.x != wallJumpingDirection)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x;
            transform.localScale = localScale;
        }
        Invoke(nameof(StopWallJumping), wallJumpingDuration);
    }
    public bool isTouchingCrumbleBlock()
    {
        return platformWallCheck.isTouchingCrumbleBlock();
    }


    public void CrumbleBlockBounceBack()
    {
        StopDash();
        rb.velocity = new Vector2(0, 0);
        wallJumpingDirection = -transform.localScale.x;
        canWallBounce = false;
        ExecuteWallJump();
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    private void StopJumping()
    {
        isJumping = false;
    }
    public void GetDash()
    {
        canDash = true;
        DashSpriteUpdated = false;
    }
    public bool GetIsDashing()
    {
        return isDashing;
    }
    public void StopDash()
    {
        //reset all values changed by dashing
        StopCoroutine(DashCoroutine);
        DashCoroutine = Dash();
        GetDash();
        canWallBounce = false;
        StartCoroutine(LockDash(0.05f));
        jumped = false;
        isDashing = false;
        DashSpriteUpdated = false;
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        rb.velocity = new Vector2(0f, 0f);
    }    
     
    public void Flip()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        //changing direction
        if (((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f)) && rb.bodyType != RigidbodyType2D.Static && Time.timeScale > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x;
            transform.localScale = localScale;
        }
    }
    private bool isGroundedBool2;
    private IEnumerator Dash()
    {
        //dash logic...                                                                                                                                                         duh
        DashSpriteUpdated = false;
        jumped = false; 
        dashAmount--;
        canDash = false;
        isDashing = true;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        canWallBounce = true;
        yield return new WaitForSeconds((dashingTime / 4) * 3);
        canWallBounce = false;
        yield return new WaitForSeconds(dashingTime / 4);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        if(Mathf.Abs(rb.velocity.x) > speed && !jumped) 
        {
            isGroundedBool2 = isGrounded();
            float velocityX = rb.velocity.x;
            float direction = velocityX / Mathf.Abs(velocityX);
            if (isGroundedBool2 && isIced)
            {
                rb.velocity = new Vector2(Mathf.Clamp(Math.Abs(rb.velocity.x), speed , speed * iceGroundedSpeedCap) * direction, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(direction * speed, rb.velocity.y);
            }
        }
        yield return new WaitForSeconds(0.05f);
        isDashing = false;
        jumped = false;
        DashCoroutine = Dash();
        yield return new WaitForSeconds(dashingCoolDown);
        GetDash();
    }
    private IEnumerator DynamicJump()
    {
        int loopCounter = 0;
        yield return new WaitForSeconds(initialDynamicJumpInterval);
        while(loopCounter < dynamicJumpAmplitudeCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            yield return new WaitForSeconds(dynamicJumpInterval);
            loopCounter++;
        }
        rb.velocity = new Vector2(rb.velocity.x, lowJumpingPower);
        DynamicJumpCoroutine = DynamicJump();
    }
    private IEnumerator LockDash(float time)
    {
        //disable the ability to dash for some time
        DashSpriteUpdated = false;
        canDash = false;
        yield return new WaitForSeconds(time);
        canDash = true;
    }
    private IEnumerator DeathCameraLockOn()
    {
        parent = transform.parent.gameObject;
        CMConfiner = parent.GetComponentInChildren<CinemachineConfiner2D>();
        if (CMConfiner != null)
        {
            CMConfiner.m_Damping = 0;
        }
        yield return new WaitForSeconds(0.1f);
        if (CMConfiner != null)
        {
            CMConfiner.m_Damping = 0;
        }
        RespawnCameraShit = DeathCameraLockOn();
    }
    private void StartDieing()
    {
        startedDieing = true;
        CoinFollower.PlayerDied = true;
    }
    private void RestartLevel()
    {
        dead = true;
        try
        {
            transform.parent.GetComponent<RoomManager>().ResetRoom();
        }
        catch { }
        transform.position = respawnPosition;
        rb.bodyType = RigidbodyType2D.Dynamic;
        StartCoroutine(RespawnCameraShit);
        anim.Rebind();
        AnimWallSlide = false;
        AnimIsWalking = false;
        AnimIsIdle = false;
        AnimIsAirborne = false;
        AnimIsDashing = false;
        Invoke(nameof(Jesus), 1f);
    }
    private void Jesus()
    {
        dead = false;
        startedDieing = false;
        CoinFollower.PlayerDied = false;
    }
    public void ChangeRespawnPoint(Vector2 newRespawnPoint)
    {
        respawnPosition = newRespawnPoint;
    }
    private void VerticalSpringBounce(Collider2D collision)
    {
        Animator animator = collision.gameObject.GetComponent<Animator>();
        StopDash();
        dashAmount = baseDashAmount;
        rb.velocity = new Vector2(0f, 18f);
        animator.SetTrigger("Activate");
    }
    private void HorizontalSpringBounce(Collider2D collision)
    {
        Animator animator = collision.gameObject.GetComponent<Animator>();
        float direction = -collision.transform.localScale.y;
        GetSpringed();
        StopDash();
        dashAmount = baseDashAmount;
        springedCounter = springedDuration;
        rb.velocity = new Vector2(direction * dashingPower * 0.66f, 10f);
        GetSpringed();
        animator.SetTrigger("Activate");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ForceZone")) 
        {
            isInForceZone = true;
            ForcePlayer forceZone = collision.GetComponent<ForcePlayer>();
            forceZoneForceX = forceZone.getForceX();
            forceZoneForceY = forceZone.getForceY();
        }
        else if(collision.CompareTag("VerticalSpring"))
        {
            VerticalSpringBounce(collision);
        }
        else if(collision.CompareTag("HorizontalSpring"))
        {
            HorizontalSpringBounce(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ForceZone"))
        {
            isInForceZone = false;
            forceZoneForceX = 0;
            forceZoneForceY = 0;
        }
    }
    private GameObject GetVirtualCamera()
    {
        foreach(Transform child in parent.transform)
        {
            if(child.CompareTag("VirtualCamera"))
            {
                return child.gameObject;
            }
        }
        return null;
    }
}
