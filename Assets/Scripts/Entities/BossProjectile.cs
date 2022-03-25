using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float timeToDespawn = 3f;

    private Cooldown despawnTimer = new Cooldown();

    private Rigidbody2D       rigidBody;
    private CapsuleCollider2D capsule;
    private Renderer          renderer;

    void Start()
    {
        if (timeToDespawn >= 0)
            despawnTimer.ChangeDuration(timeToDespawn);

        rigidBody = GetComponent<Rigidbody2D>();
        capsule   = GetComponent<CapsuleCollider2D>();
        renderer  = GetComponent<Renderer>();
    }

    void Update()
    {
        Move();
        
        despawnTimer.Update(Time.deltaTime);
        renderer.material.SetColor("_Color", new Color(1, 1, 1, despawnTimer.CompletionRatio()));
        if (despawnTimer.HasEnded())
            Destroy(this.gameObject);
    }

    void Move()
    {
        rigidBody.velocity = new Vector2(speed, 0);
    }

    public void SetDir(float dir)
    {
        speed *= dir;
        transform.localScale = new Vector3(dir, 1, 1);
    }


    /// <summary> Checks collisions with other enemies and ignores them. </summary>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
            Physics2D.IgnoreCollision(capsule, other.gameObject.GetComponent<CapsuleCollider2D>());
    }
}
