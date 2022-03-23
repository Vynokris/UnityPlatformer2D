using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float Health             = 3;
    [SerializeField] private float walkSpeed          = 7;
    [SerializeField] private float jumpSpeed          = 7;
    [SerializeField] private float jumpDuration       = 0.35f;
    [SerializeField] private float jumpBufferDuration = 0.1f;
    [SerializeField] private float coyoteDuration     = 0.15f;
    [SerializeField] private float fallAcceleration   = 0.1f;
    [SerializeField] private float maxFallSpeed       = 5;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private GameController gameController;

    private float    targetFPS      = 75;
    private bool     isGrounded     = true;
    private Cooldown jumpTime       = new Cooldown(0.35f);
    private Cooldown jumpBufferTime = new Cooldown(0.1f);
    private Cooldown coyoteTime     = new Cooldown(0.15f);
    private Cooldown knockBackTime  = new Cooldown(0.2f);
    private Vector2  knockBackDir   = new Vector2 (0, 0);

    [HideInInspector] public static Vector2 spawnPos = new Vector2(0, 0);

    [HideInInspector] public Rigidbody2D rigidBody { get; private set; }
    private BoxCollider2D box;
    private Animator      animator;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        box       = GetComponents<BoxCollider2D>()[1];
        animator  = GetComponent<Animator>();
        rigidBody.isKinematic = false;

        // Update cooldowns according to inspector values.
        if (jumpDuration > 0)
            jumpTime.ChangeDuration(jumpDuration);
        if (jumpBufferDuration > 0)
            jumpBufferTime.ChangeDuration(jumpBufferDuration);
        if (coyoteDuration > 0)
            coyoteTime.ChangeDuration(coyoteDuration);

        // Move to the last checkpoint used.
        if (spawnPos.x != 0 && spawnPos.y != 0)
            transform.position = spawnPos;

        // End some cooldowns.
        jumpTime.Counter       = 0;
        jumpBufferTime.Counter = 0;
        knockBackTime.Counter  = 0;
    }

    void Update()
    {
        if (Health > 0)
        {
            CheckGrounded();
            Walk();
            UpdateJump();
            KnockBack();
            Flip();
        }
    }
    

    /// <summary> Listens to horizontal input and moves accodingly. </summary>
    void Walk()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rigidBody.velocity = new Vector2(moveInput * walkSpeed * Time.deltaTime * targetFPS, rigidBody.velocity.y);
        animator.SetFloat("Velocity", rigidBody.velocity.magnitude);
    }

    /// <summary> Listens to spacebar input and jumps accordingly. </summary>
    void UpdateJump()
    {
        animator.SetBool("IsJumping", !jumpTime.Update(Time.deltaTime));

        // Initiate jump from the ground.
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) 
        {
            jumpTime.Reset();
        }

        // Buffer a jump while in the air.
        if (!isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTime.Reset();
        }

        // Initiate jump using the jump buffer.
        if (!jumpBufferTime.Update(Time.deltaTime) && isGrounded)
        {
            jumpTime.Reset();
        }

        // Initiate jump using coyote time.
        if (!isGrounded && !coyoteTime.Update(Time.deltaTime) && Input.GetKeyDown(KeyCode.Space))
        {
            jumpTime.Reset();
        }

        // Continue jump in the air & end after duration.
        if (Input.GetKey(KeyCode.Space))
        {
            if (!jumpTime.HasEnded())
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed * Time.deltaTime * targetFPS);
            }
        }

        // End jump prematurely.
        if (Input.GetKeyUp(KeyCode.Space)) 
        {
            jumpTime.Counter = 0;
        }

        // Fall down.
        if (jumpTime.HasEnded() && !isGrounded && rigidBody.velocity.y > -maxFallSpeed)
        {
            rigidBody.velocity -= new Vector2(0, fallAcceleration * Time.deltaTime * targetFPS);
        }

        // Reset coyote time counter.
        if (isGrounded && !coyoteTime.FirstFrame(Time.deltaTime))
        {
            coyoteTime.Reset();
        }
    }

    /// <summary> Initiates a knockback in the given direction for the given duration. </summary>
    public void StartKnockBack(Vector2 dir, float duration = 0.2f)
    {
        knockBackTime.ChangeDuration(duration);
        knockBackDir = dir;
    }

    /// <summary> Applies knockback to the player when the knockback cooldown is not finished. </summary>
    void KnockBack()
    {
        // Reset velocity on the first frame.
        if (knockBackTime.Counter == knockBackTime.Duration)
            rigidBody.velocity = new Vector2(0, 0);

        // Accelerate in the knock back direction in the remaining frames.
        if (!knockBackTime.Update(Time.deltaTime))
            rigidBody.velocity += knockBackDir * 0.2f * Time.deltaTime * targetFPS *  knockBackTime.CompletionRatio();
    }

    /// <summary> Decreases the player's health by the given amount. </summary>
    void DecreaseHealth(int amount = 1)
    {
        Health -= amount;
        if (Health == 0 && gameController != null)
        {
            gameController.playerDied.Invoke();
        }
    }
    

    /// <summary> Updates isGrounded variable by doing a box cast downwards. </summary>
    void CheckGrounded()
    {
        isGrounded = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.down, 0.1f, collisionLayer).collider != null;
        animator.SetBool("IsGrounded", isGrounded);
    }

    /// <summary> Flips the sprite depending on the player's direction. </summary>
    void Flip()
    {
        if (knockBackTime.HasEnded())
        {
            if (rigidBody.velocity.x < 0 && transform.localScale.x > 0 ||
                rigidBody.velocity.x > 0 && transform.localScale.x < 0)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
        }
    }


    /// <summary> Handle collisions with other entities. </summary>
    void OnCollisionEnter2D(Collision2D other)
    {
        // Decrease hp when hit by an enemy.
        if (other.gameObject.tag == "Enemy")
        {
            if (rigidBody.velocity.y > -1f) 
            {
                DecreaseHealth();
                StartKnockBack(transform.position.x > other.gameObject.transform.position.x ? new Vector2(80, 5) : new Vector2(-80, 5));
            }
        }

        // Decrease hp when hitting a harmful decoration.
        if (other.gameObject.layer == LayerMask.NameToLayer("Decorations"))
        {
            DecreaseHealth();
            StartKnockBack(transform.localScale.x < 0 ? new Vector2(80, 5) : new Vector2(-80, 5));
        }
    }

    /// <summary> Handles collisions with other entities' trigger boxes. </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Bounce on enemies.
        if (other.gameObject.tag == "Enemy") 
        {
            if (rigidBody.velocity.y < -1f) 
            {
                StartKnockBack(Vector2.up * 10);
            }
        }
    }
}
