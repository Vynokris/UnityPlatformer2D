using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private int   health           = 3;
    [SerializeField] private float minShootCooldown = 1f;
    [SerializeField] private float maxShootCooldown = 2f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject bossProjectilePrefab;

    private float    targetFPS     = 75;
    private Cooldown shootCooldown = new Cooldown(2.5f);
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
    }

    void Update()
    {
        rigidBody.velocity = new Vector2(0, 0);
        if (health <= 0)
            animator.SetBool("Dead", true);

        Flip();
        Shoot();
        KnockBack();
    }


    /// <summary> Makes the boss shoot at random intervals. </summary>
    void Shoot()
    {
        if (shootCooldown.Counter == shootCooldown.Duration)
        {
            animator.SetBool("Shooting", false);
        }

        shootCooldown.Update(Time.deltaTime);
        
        if (shootCooldown.HasEnded())
        {
            shootCooldown.ChangeDuration(Random.Range(minShootCooldown, maxShootCooldown));
            animator.SetBool("Shooting", true);
        }
    }

    /// <summary> Creates a new boss projectile. Called by the shoot animation. </summary>
    void CreateProjectile()
    {
        GameObject newProjectile = Instantiate(bossProjectilePrefab, transform.position, Quaternion.identity, transform.parent);
        newProjectile.transform.position += new Vector3(transform.localScale.x * 0.8f, -0.03f, 0);
        newProjectile.GetComponent<BossProjectile>().SetDir(transform.localScale.x);
    }

    /// <summary> Initiates a knockback in the given direction for the given duration. </summary>
    public void StartKnockBack(Vector2 dir, float duration = 0.2f)
    {
        knockBackTime.ChangeDuration(duration);
        knockBackDir = dir;
    }

    /// <summary> Applies knockback to the boss when the knockback cooldown is not finished. </summary>
    void KnockBack()
    {
        // Reset velocity on the first frame.
        if (knockBackTime.Counter == knockBackTime.Duration)
            rigidBody.velocity = new Vector2(0, 0);

        // Accelerate in the knock back direction in the remaining frames.
        if (!knockBackTime.Update(Time.deltaTime))
            rigidBody.velocity += knockBackDir * 0.2f * Time.deltaTime * targetFPS *  knockBackTime.CompletionRatio();
    }

    /// <summary> Flips the boss so that it always faces towards the player. </summary>
    void Flip()
    {
        float distToPlayer = playerTransform.position.x - transform.position.x;

        if (distToPlayer < 0 && transform.localScale.x > 0 ||
            distToPlayer > 0 && transform.localScale.x < 0)
        {
            transform.localScale *= new Vector2(-1, 1);
        }
    }


    /// <summary> Checks collisions with other enemies and ignores them. </summary>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" ||
            other.gameObject.tag == "Projectile")
        {
            Physics2D.IgnoreCollision(capsule, other.gameObject.GetComponent<CapsuleCollider2D>());
        }
    }

    /// <summary> Checks collisions with the player's trigger and takes damage accordingly. </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (knockBackTime.HasEnded() && other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<Rigidbody2D>().velocity.y < -1f)
            {
                health--;
                StartKnockBack(new Vector2(-transform.localScale.x * 80, 0), 0.4f);
            }
        }
    }
}
