using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float timeToDespawn = 3f;

    private bool     hasHitWall   = false;
    private bool     falling      = false;
    private Cooldown despawnTimer = new Cooldown();

    private Rigidbody2D       rigidBody;
    private CapsuleCollider2D capsule;
    private Renderer          objectRenderer;

    void Start()
    {
        if (timeToDespawn >= 0)
            despawnTimer.ChangeDuration(timeToDespawn);

        rigidBody      = GetComponent<Rigidbody2D>();
        capsule        = GetComponent<CapsuleCollider2D>();
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Move();
        Fade();
    }

    void Move()
    {
        if (falling)
            rigidBody.velocity = new Vector2(0, -2);
        else if (hasHitWall)
            rigidBody.velocity = new Vector2(0, 0);
        else
            rigidBody.velocity = new Vector2(speed, 0);
    }

    void Fade()
    {
        despawnTimer.Update(Time.deltaTime);
        objectRenderer.material.SetColor("_Color", new Color(1, 1, 1, despawnTimer.CompletionRatio()));
        if (despawnTimer.HasEnded())
            Destroy(this.gameObject);
    }

    public void SetDir(float dir)
    {
        speed *= dir;
        transform.localScale = new Vector3(dir, 1, 1);
    }


    /// <summary> Checks collisions and behaves accordingly. </summary>
    void OnCollisionEnter2D(Collision2D other)
    {
        // Ignore collisions with other enemies.
        if (other.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(capsule, other.gameObject.GetComponent<CapsuleCollider2D>());
        }

        // Fall when the player is hit, and ignore collisions with him.
        if (other.gameObject.tag == "Player")
        {
            falling = true;
            Physics2D.IgnoreCollision(capsule, other.gameObject.GetComponents<BoxCollider2D>()[0]);
            Physics2D.IgnoreCollision(capsule, other.gameObject.GetComponents<BoxCollider2D>()[1]);
        }

        // Stop when a wall is hit.
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            other.gameObject.layer == LayerMask.NameToLayer("Decorations"))
        {
            hasHitWall = true;
            if (!falling)
                Destroy(this.capsule);  
        }
    }
}
