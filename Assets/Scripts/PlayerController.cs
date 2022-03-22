using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float Health            = 3;
    [SerializeField] private float walkSpeed         = 7;
    [SerializeField] private float jumpSpeed         = 7;
    [SerializeField] private float fallAcceleration  = 0.1f;
    [SerializeField] private float maxFallSpeed      = 5;
    [SerializeField] private LayerMask collisionLayer;

    private float    targetFPS     = 75;
    private bool     isGrounded    = true;
    private Cooldown jumpTime      = new Cooldown(0.35f);
    private Cooldown knockBackTime = new Cooldown(0.2f);
    private Vector2  knockBackDir  = new Vector2 (0, 0);

    private Rigidbody2D       rigidBody;
    private CapsuleCollider2D capsule;
    private Animator          animator;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        capsule   = GetComponent<CapsuleCollider2D>();
        animator  = GetComponent<Animator>();
        rigidBody.isKinematic = false;

        jumpTime.Counter = 0;
        knockBackTime.Counter = 0;
    }

    void Update()
    {
        CheckGrounded();
        Walk();
        UpdateJump();
        KnockBack();
        Flip();
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
    

    /// <summary> Updates isGrounded variable by doing a box cast downwards. </summary>
    void CheckGrounded()
    {
        isGrounded = Physics2D.CapsuleCast(capsule.bounds.center, capsule.bounds.size, capsule.direction, 0, Vector2.down, 0.1f, collisionLayer).collider != null;
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
                Health--;
                knockBackTime.Reset();
                if (transform.position.x > other.gameObject.transform.position.x)
                    knockBackDir = new Vector2( 80, 5);
                else
                    knockBackDir = new Vector2(-80, 5);
            }
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
                knockBackTime.Reset();
                knockBackDir = Vector2.up * 10;
            }
        }

        // Bounce on jump plants.
        if (other.gameObject.tag == "JumpPlant")
        {
            if (rigidBody.velocity.y < -1f) 
            {
                knockBackTime.Reset();
                knockBackDir = Vector2.up * 17;
                /*
                // Could be used if I wanted the jump plant to push in any direction.
                knockBackDir = new Vector2(Mathf.Cos(other.gameObject.transform.rotation.z + 90), 
                                           Mathf.Sin(other.gameObject.transform.rotation.z + 90)) * 17;
                */
            }
        }
    }
}
