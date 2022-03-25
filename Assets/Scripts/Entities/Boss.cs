using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private float minShootCooldown = 1f;
    [SerializeField] private float maxShootCooldown = 2f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject bossProjectilePrefab;

    private Cooldown shootCooldown = new Cooldown(2.5f);

    private Rigidbody2D       rigidBody;
    private CapsuleCollider2D capsule;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        capsule   = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        rigidBody.velocity = new Vector2(0, 0);
        Flip();
        Shoot();
    }

    void Shoot()
    {
        if (shootCooldown.Update(Time.deltaTime))
        {
            shootCooldown.ChangeDuration(Random.Range(minShootCooldown, maxShootCooldown));
            GameObject newProjectile = Instantiate(bossProjectilePrefab, transform.position, Quaternion.identity, transform.parent);
            newProjectile.GetComponent<BossProjectile>().SetDir(transform.localScale.x);
        }
    }

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
        if (other.gameObject.tag == "Enemy")
            Physics2D.IgnoreCollision(capsule, other.gameObject.GetComponent<CapsuleCollider2D>());
    }
}
