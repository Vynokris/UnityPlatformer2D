using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 7;
    [SerializeField] private float jumpSpeed = 7;
    [SerializeField] private float jumpDuration  = 0.35f;
    [SerializeField] private float fallAcceleration = 0.1f;
    [SerializeField] private float maxFallSpeed = 5;
    [SerializeField] private LayerMask collisionLayer;

    private bool  isGrounded = true;
    private bool  isJumping  = false;
    private float jumpTimeCounter = 0;

    private Rigidbody2D       rigidBody;
    private CapsuleCollider2D capsule;
    private Animator          animator;


    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        capsule   = GetComponent<CapsuleCollider2D>();
        animator  = GetComponent<Animator>();
        rigidBody.isKinematic = false;
    }

    void Update()
    {
        CheckGrounded();
        Walk();
        Jump();
        Flip();
    }
    

    /// <summary>
    /// Listens to horizontal input and moves accodingly.
    /// </summary>
    void Walk()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rigidBody.velocity = new Vector2(moveInput * walkSpeed, rigidBody.velocity.y);
        animator.SetFloat("Velocity", rigidBody.velocity.magnitude);
    }

    /// <summary>
    /// Listens to spacebar input and jumps accordingly.
    /// </summary>
    void Jump()
    {
        // Initiate jump from the ground.
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) 
        {
            isJumping = true;
            jumpTimeCounter = jumpDuration;
        }

        // Continue jump in the air & end after duration.
        if (Input.GetKey(KeyCode.Space))
        {
            if (jumpTimeCounter > 0 && isJumping)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpSpeed);
                jumpTimeCounter -= Time.deltaTime;
            }
            else 
            {
                isJumping = false;
            }
        }

        // End jump prematurely.
        if (Input.GetKeyUp(KeyCode.Space)) 
        {
            isJumping = false;
        }

        // Fall down.
        if (!isJumping && !isGrounded && rigidBody.velocity.y > -maxFallSpeed)
        {
            rigidBody.velocity -= new Vector2(0, fallAcceleration);
        }

        animator.SetBool("IsJumping", isJumping);
    }
    

    /// <summary>
    /// Updates isGrounded variable by doing a box cast downwards.
    /// </summary>
    void CheckGrounded()
    {
        isGrounded = Physics2D.CapsuleCast(capsule.bounds.center, capsule.bounds.size, capsule.direction, 0, Vector2.down, 0.1f, collisionLayer).collider != null;
        animator.SetBool("IsGrounded", isGrounded);
    }


    /// <summary>
    /// Flips the sprite depending on the player's direction.
    /// </summary>
    void Flip()
    {
        if (rigidBody.velocity.x < 0 && transform.localScale.x > 0)
            transform.localScale *= new Vector2(-1, 1);
        else if (rigidBody.velocity.x > 0 && transform.localScale.x < 0)
            transform.localScale *= new Vector2(-1, 1);
    }
}
