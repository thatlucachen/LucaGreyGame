// This script is attached to the player and allows the player to run, jump, and dash in the game. 
// The script also controls the player's interactions with game mechanics and other game objects.

using System.Collections;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    // Creating variables for game components 
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    private FrameInputs playerInputs;
    [SerializeField] private ParticleSystem jumpParticles;
    private Animator animator;

    private void Start()
    {
        // Referencing game components so they can be modified in the code
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        jumpParticles = GetComponentInChildren<ParticleSystem>();
        animator = GetComponent<Animator>();
        CinemachineVirtualCamera virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    // Update is executed every in-game frame
    private void Update()
    {
        // Calling all player movement functions every in-game frame
        Run();
        Jump();
        Dash();
        PhysicsControl();
        AnimationControl();
    }

    // Declaring running-related variables
    [SerializeField] float runSpeed = 8;
    bool isGrounded = false;
    [SerializeField] float acceleration = 2;
    bool isFacingRight = true;
    bool canMove = true;

    // Function for running-related movement
    private void Run()
    {
        // The player cannot run during the first 0.15 seconds after starting their dash 
        if ((isDashing && !regainsControl) || !canMove)
        {
            return;
        }

        // Movement code for running left and right, with acceleration towards the player's top speed at the start of their run
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (rigidBody.velocity.x > 0)
            {
                playerInputs.X = 0;
            }
            playerInputs.X = Mathf.MoveTowards(playerInputs.X, -1, acceleration * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (rigidBody.velocity.x < 0)
            {
                playerInputs.X = 0;
            }
            playerInputs.X = Mathf.MoveTowards(playerInputs.X, 1, acceleration * Time.deltaTime);
        }
        else
        {
            playerInputs.X = Mathf.MoveTowards(playerInputs.X, 0, acceleration * 2 * Time.deltaTime);
        }

        var idealVelocity = new Vector3(playerInputs.X * runSpeed, rigidBody.velocity.y);
        rigidBody.velocity = Vector3.MoveTowards(rigidBody.velocity, idealVelocity, 100 * Time.deltaTime);

        // Determines whether the player is facing left or right
        if (rigidBody.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
            isFacingRight = false;
        }
        else if (rigidBody.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
            isFacingRight = true;
        }
    }

    // Respawn points
    Vector2 currentRespawnPoint = new Vector2(0, 0);
    Vector2 room1RespawnPoint = new Vector2(8, -6);
    Vector2 room2RespawnPoint = new Vector2(131, -6);
    Vector2 room3RespawnPoint = new Vector2(150, -6);
    Vector2 room4RespawnPoint = new Vector2(170, 15);
    Vector2 room5RespawnPoint = new Vector2(189, 7);
    Vector2 room6RespawnPoint = new Vector2(199, -3);
    Vector2 room7RespawnPoint = new Vector2(232, 18);
    Vector2 room10RespawnPoint = new Vector2(275, 9);
    Vector2 room11RespawnPoint = new Vector2(304.5f, 21);
    Vector2 room12RespawnPoint = new Vector2(321.5f, 27);
    Vector2 room13RespawnPoint = new Vector2(311, 29);
    Vector2 room14RespawnPoint = new Vector2(291, 38);
    Vector2 room15RespawnPoint = new Vector2(285, 29);
    Vector2 room16RespawnPoint = new Vector2(267, 29);
    Vector2 room17RespawnPoint = new Vector2(273, 39);

    // Executes while the player is touching a collider
    private void OnTriggerStay2D(Collider2D other)
    {
        // Set the isGrounded variable to true if the player's feet are touching a platform
        if ((other.CompareTag("Platform") || other.CompareTag("Jump Pad Up") || other.CompareTag("Jump Pad Left") || other.CompareTag("Jump Pad Right")) && boxCollider.IsTouching(other))
        {
            isGrounded = true;
            if (other.CompareTag("Platform"))
            {
                onJumpPad = false;
            }
            if (duringJump)
            {
                duringJump = false;
            }
        }

        // Send the player back to the start of the room if they touch an obstacle or border
        if (other.gameObject.CompareTag("Obstacle") || (other.gameObject.CompareTag("Lazer") && !isDashing))
        {
            rigidBody.velocity = Vector3.zero;
            Physics2D.gravity = new Vector3(Physics2D.gravity.x, 0);
            isDashing = false;
            trailRenderer.emitting = false;
            canMove = false;
            canDash = false;
            transform.position = currentRespawnPoint;
            StartCoroutine(DeathTimer());
        }

        // Send the player back to the right respawn point when they die
        if (other.CompareTag("Room 1")) currentRespawnPoint = room1RespawnPoint;
        if (other.CompareTag("Room 2")) currentRespawnPoint = room2RespawnPoint;
        if (other.CompareTag("Room 3")) currentRespawnPoint = room3RespawnPoint;
        if (other.CompareTag("Room 4")) currentRespawnPoint = room4RespawnPoint;
        if (other.CompareTag("Room 5")) currentRespawnPoint = room5RespawnPoint;
        if (other.CompareTag("Room 6")) currentRespawnPoint = room6RespawnPoint;
        if (other.CompareTag("Room 7")) currentRespawnPoint = room7RespawnPoint;
        if (other.CompareTag("Room 10")) currentRespawnPoint = room10RespawnPoint;
        if (other.CompareTag("Room 11")) currentRespawnPoint = room11RespawnPoint;
        if (other.CompareTag("Room 12")) currentRespawnPoint = room12RespawnPoint;
        if (other.CompareTag("Room 13")) currentRespawnPoint = room13RespawnPoint;
        if (other.CompareTag("Room 14")) currentRespawnPoint = room14RespawnPoint;
        if (other.CompareTag("Room 15")) currentRespawnPoint = room15RespawnPoint;
        if (other.CompareTag("Room 17")) currentRespawnPoint = room17RespawnPoint;
    }

    // Death timer
    float deathTimer = 0.05f;
    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(deathTimer);
        Physics2D.gravity = new Vector3(Physics2D.gravity.x, -9.81f);
        canMove = true;
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Send the player back to the start of the room if they touch an obstacle or border
        if (other.gameObject.CompareTag("Obstacle") || (other.gameObject.CompareTag("Lazer") && !isDashing))
        {
            rigidBody.velocity = Vector3.zero;
            Physics2D.gravity = new Vector3(Physics2D.gravity.x, 0);
            isDashing = false;
            trailRenderer.emitting = false;
            canMove = false;
            canDash = false;
            transform.position = currentRespawnPoint;
            StartCoroutine(DeathTimer());
        }

        // Jump pad code
        if (other.gameObject.CompareTag("Jump Pad Up") || other.gameObject.CompareTag("Jump Pad Left") || other.gameObject.CompareTag("Jump Pad Right"))
        {
            isJumping = true;
            canDash = true;
            onJumpPad = true;
            if (other.gameObject.CompareTag("Jump Pad Up")) { rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpPadVerticalBoost); }
            else if (other.gameObject.CompareTag("Jump Pad Left")) { rigidBody.velocity = new Vector2(-jumpPadHorizontalBoost, rigidBody.velocity.y); }
            else if (other.gameObject.CompareTag("Jump Pad Right")) { rigidBody.velocity = new Vector2(jumpPadHorizontalBoost, rigidBody.velocity.y); }
        }

        // Didn't use the code below in the finalized game, but it allows the player to replenish their dash if they touch a dash crystal
        if (other.gameObject.CompareTag("Dash Crystal"))
        {
            if (!canDash)
            {
                other.gameObject.SetActive(false);
                canDash = true;
                dashCooldownValue = 0;
                StartCoroutine(DashCooldownBypass());
                StartCoroutine(CollisionDashCrystalTimer(other));
            }
        }

        // The player can no longer move once they collect the medal and beat the game
        if (other.gameObject.CompareTag("Medal"))
        {
            other.gameObject.SetActive(false);
            Physics2D.gravity = new Vector3(0, 0);
            rigidBody.velocity = Vector3.zero;
            isDashing = false;
            trailRenderer.emitting = false;
            canMove = false;
            canDash = false;
        }
    }

    // Declaring variables used for the jump pad
    float jumpPadVerticalBoost = 15;
    float jumpPadHorizontalBoost = 45;
    float jumpPadHorizontalUpwardsBoost = 5;
    bool onJumpPad = false;
    
    // Executes once when the player enters a collider
    // The code below is a repeat of the code in the OnCollisionEnter2D function. Since Unity treats trigger colliders and non-trigger colliders differently, the code must be rewritten twice.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Send the player back to the start of the room if they touch an obstacle or border
        if (other.CompareTag("Obstacle") || (other.CompareTag("Lazer") && !isDashing))
        {
            rigidBody.velocity = Vector3.zero;
            Physics2D.gravity = new Vector3(Physics2D.gravity.x, -9.81f);
            isDashing = false;
            trailRenderer.emitting = false;
            transform.position = currentRespawnPoint;
        }

        // Jump pad code
        if (other.gameObject.CompareTag("Jump Pad Up") || other.gameObject.CompareTag("Jump Pad Left") || other.gameObject.CompareTag("Jump Pad Right"))
        {
            isJumping = true;
            canDash = true;
            onJumpPad = true;
            if (other.gameObject.CompareTag("Jump Pad Up")) { rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpPadVerticalBoost); }
            else if (other.gameObject.CompareTag("Jump Pad Left")) { rigidBody.velocity = new Vector2(-jumpPadHorizontalBoost, rigidBody.velocity.y + jumpPadHorizontalUpwardsBoost); }
            else if (other.gameObject.CompareTag("Jump Pad Right")) { rigidBody.velocity = new Vector2(jumpPadHorizontalBoost, rigidBody.velocity.y + jumpPadHorizontalUpwardsBoost); }
        }

        // Didn't use the code below in the finalized game, but it allows the player to replenish their dash if they touch a dash crystal
        if (other.gameObject.CompareTag("Dash Crystal"))
        {
            if (!canDash)
            {
                other.gameObject.SetActive(false);
                canDash = true;
                dashCooldownValue = 0;
                StartCoroutine(DashCooldownBypass());
                StartCoroutine(ColliderDashCrystalTimer(other));
            }
        }

        // The player can no longer move once they collect the medal and beat the game
        if (other.CompareTag("Medal"))
        {
            other.gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            Physics2D.gravity = new Vector3(0, 0);
            rigidBody.velocity = Vector3.zero;
            isDashing = false;
            trailRenderer.emitting = false;
            canMove = false;
            canDash = false;
        }
    }

    // Dash crystals re-appear 5 seconds after they were last used (Didn't add dash crystals into the finalized game)
    // Collision2D and Collider2D are different, so the code must be rewritten twice here
    float dashCrystalRespawnTime = 5;
    IEnumerator ColliderDashCrystalTimer(Collider2D other)
    {
        yield return new WaitForSeconds(dashCrystalRespawnTime);
        other.gameObject.SetActive(true);
    }
    IEnumerator CollisionDashCrystalTimer(Collision2D other)
    {
        yield return new WaitForSeconds(dashCrystalRespawnTime);
        other.gameObject.SetActive(true);
    }

    // Executes once when the player exits a collider
    private void OnTriggerExit2D(Collider2D other)
    {
        // Set the isGrounded variable to false if the player's feet are no longer touching a platform
        if (other.CompareTag("Platform") && !boxCollider.IsTouching(other))
        {
            isGrounded = false;
        }
    }

    // Turns off the player's dash cooldown for 0.5 seconds after touching a dash crystal
    IEnumerator DashCooldownBypass()
    {
        yield return new WaitForSeconds(0.5f);
        dashCooldownValue = 0.3f;
    }

    // Declaring jumping-related variables
    bool isJumping = false;
    bool duringJump = false;
    [SerializeField] float jumpSpeed = 12;
    [SerializeField] float fallMultiplier = 4;
    [SerializeField] float jumpVelocityDecrease = 8;
    float jumpCooldownValue = 0.25f;
    bool jumpOnCooldown = false;
    float coyoteTime = 0.15f;
    float coyoteTimeCounter;
    float jumpBufferTime = 0.2f;
    float jumpBufferCounter;

    // Function for jumping-related movement
    private void Jump()
    {
        // The player cannot jump during their dash or while on a jump pad
        if (isDashing)
        {
            return;
        }

        // Manage coyote time
        // (For 0.15 seconds after leaving a platform, the player can still jump.)
        // Coyote time makes the game controls more forgiving to player input mistakes
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Manage jump buffering
        // (Allows the player's jump to be registered for 0.2 seconds before they touch the ground. This jump input is executed when they land.)
        // Jump buffering makes the game controls more forgiving to player input mistakes
        if ((Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.C)) && !jumpOnCooldown && !onJumpPad)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Jumping code
        // The player cannot jump in the air while their dash is on cooldown
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !isJumping && !dashOnCooldown && !onJumpPad)
        {
            jumpParticles.Play();
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
            jumpBufferCounter = 0;
            isJumping = true;
            duringJump = true;
            StartCoroutine(JumpCooldown());
        }
        // Increases player falling speed after a point of speed decrease during the jump
        // Makes the jump feel snappier and more in the player's control
        if (rigidBody.velocity.y < jumpVelocityDecrease || (rigidBody.velocity.y > 0 && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.C) && !onJumpPad))
        {
            rigidBody.velocity += fallMultiplier * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
            isJumping = false;
        }

        if (isJumping)
        {
            coyoteTimeCounter = 0;
        }
    }

    // Jump cooldown timer
    IEnumerator JumpCooldown()
    {
        jumpOnCooldown = true;
        yield return new WaitForSeconds(jumpCooldownValue);
        jumpOnCooldown = false;
    }

    // Declaring dashing-related variables
    bool canDash = true;
    public bool isDashing = false;
    [SerializeField] float dashLength = 24;
    [SerializeField] float dashTime = 0.2f;
    float timeStartedDash;
    bool regainsControl = false;
    float timeToRegainControl = 0.15f;
    [SerializeField] float dashCooldownValue = 0.3f;
    bool dashOnCooldown = false;
    Vector3 dashDirection;

    // Function for dashing-related movement
    private void Dash()
    {
        // The player can always dash if they are on the ground, they aren't dashing, and their dash isn't on cooldown
        if (isGrounded)
        {
            canDash = true;
        }

        // Dashing code
        if ((Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.X)) && canDash && !dashOnCooldown && !isDashing)
        {
            isDashing = true;
            canDash = false;

            // Sets the direction of the player's dash based on what buttons they are pressing
            // Normalizes the dash direction magnitude so diagonal dashes move the player the same distance as horizontal and vertical dashes
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { dashDirection = new Vector3(0, 1).normalized; }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { dashDirection = new Vector3(-1, 0).normalized; }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { dashDirection = new Vector3(0, -1).normalized; }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { dashDirection = new Vector3(1, 0).normalized; }
            if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftArrow))) { dashDirection = new Vector3(-1, 1).normalized; }
            if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.RightArrow))) { dashDirection = new Vector3(1, 1).normalized; }
            if ((Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftArrow))) { dashDirection = new Vector3(-1, -1).normalized; }
            if ((Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D)) || (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.RightArrow))) { dashDirection = new Vector3(1, -1).normalized; }

            // Decides whether to dash left or right based on which side the player is facing, if they are not pressing any other buttons 
            if (dashDirection == Vector3.zero)
            {
                if (isFacingRight)
                {
                    dashDirection = Vector3.right.normalized;
                }
                else
                {
                    dashDirection = Vector3.left.normalized;
                }
            }

            // Starts timing the player's dash
            timeStartedDash = Time.time;
            // Turns off the player's vertical gravity while they are dashing
            Physics2D.gravity = new Vector3(Physics2D.gravity.x, 0);
        }

        // Manages the player's movement during their dash
        if (isDashing)
        {
            // Rapidly accelerate the player in the direction of their dash
            rigidBody.velocity = dashDirection * dashLength;
            // Make the trail renderer emit colour during the dash
            trailRenderer.emitting = true;

            // Gives the player back the ability to move 0.15 seconds after starting their dash
            if (Time.time >= timeStartedDash + timeToRegainControl && !regainsControl)
            {
                regainsControl = true;
            }

            if (Time.time >= timeStartedDash + dashTime)
            {
                EndOfDash();
            }
        }
    }

    // Function to end the player's dash
    public void EndOfDash()
    {
        isDashing = false;
        regainsControl = false;
        // Stop the trail renderer from emitting colour at the end of the dash
        trailRenderer.emitting = false;
        // Start the dash cooldown timer
        StartCoroutine(DashCooldown());
        dashDirection = Vector3.zero;
        rigidBody.velocity = Vector3.zero;
        // Turns the player's vertical gravity back on
        Physics2D.gravity = new Vector3(Physics2D.gravity.x, -9.81f);
    }

    // Dash cooldown timer
    IEnumerator DashCooldown()
    {
        dashOnCooldown = true;
        yield return new WaitForSeconds(dashCooldownValue);
        dashOnCooldown = false;
        trailRenderer.emitting = false;
    }

    // Limits the player's falling speed to a maximum amount, so that the player's falling movement feels predictable
    [SerializeField] float maximumFallSpeed = -20;
    private void PhysicsControl()
    {
        rigidBody.gravityScale = 1.8f;
        if (rigidBody.velocity.y < maximumFallSpeed)
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maximumFallSpeed);
        }
    }

    // Controls the animation variables created in the Unity animator
    public void AnimationControl()
    {
        // Determines if the player is running or idle to play the correct animation
        animator.SetFloat("animatorRunSpeed", Mathf.Abs(rigidBody.velocity.x));

        // Determines if the player is jumping to play the correct animation
        if (rigidBody.velocity.y > 0.01 && duringJump)
        {
            animator.SetBool("animatorIsJumping", true);
        }
        else
        {
            animator.SetBool("animatorIsJumping", false);
        }

        // Determines if the player is falling to play the correct animation
        if (rigidBody.velocity.y < -0.01 && !isGrounded)
        {
            animator.SetBool("animatorIsFalling", true);
        }
        else
        {
            animator.SetBool("animatorIsFalling", false);
        }
    }

    // Declare the X and Y floats to be used in the player movement code
    private struct FrameInputs
    {
        public float X;
        public float Y;
    }
}
