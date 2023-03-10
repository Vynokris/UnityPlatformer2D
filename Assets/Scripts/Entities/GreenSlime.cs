using UnityEngine;

public class GreenSlime : MonoBehaviour
{
    [SerializeField][Range(-1, 1)] private int walkDir = -1;
    [SerializeField] private float      walkSpeed;
    [SerializeField] private Transform  playerTransform;
    [SerializeField] private GameObject healthOrbPrefab;
    [SerializeField] private LayerMask  collisionLayer;

    private bool              isDead    = false;
    private float             targetFPS = 75;
    private Rigidbody2D       rigidBody;
    private CapsuleCollider2D capsule;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        capsule   = GetComponent<CapsuleCollider2D>();
        transform.localScale *= new Vector2(walkDir, 1);
    }

    void Update()
    {
        if (isDead) 
        {
            if (healthOrbPrefab != null && Random.Range(0, 10) < 3) 
            {
                GameObject newHealthOrb = Instantiate(healthOrbPrefab, transform.position, Quaternion.identity);
                newHealthOrb.GetComponent<HealthOrb>().playerTransform = playerTransform;
            }
            Destroy(this.gameObject);
        }
        Walk();
    }


    void Walk()
    {
        if (!IsWallFwd() && IsGroundFwd()) 
        {
            rigidBody.velocity = new Vector2(walkDir * Time.deltaTime * targetFPS, rigidBody.velocity.y);
        }
        else 
        {
            Turn();
        }
    }

    public void Turn()
    {
        walkDir *= -1;
        transform.localScale *= new Vector2(-1, 1);
    }


    /// <summary> Returns true if a wall is in front of the slime. </summary>
    bool IsWallFwd()
    {
        return Physics2D.CapsuleCast(capsule.bounds.center + new Vector3(0, 0.1f, 0), capsule.bounds.size, capsule.direction, 0, new Vector2(walkDir, 0), 0.1f, collisionLayer).collider != null;
    }

    /// <summary> Returns true if there is ground in front of the slime. </summary>
    bool IsGroundFwd()
    {
        return Physics2D.CapsuleCast(capsule.bounds.center, capsule.bounds.size, capsule.direction, 0, new Vector2(walkDir * 5, -1), 1f, collisionLayer).collider != null;
    }


    /// <summary> Checks collisions with other enemies and ignores them. </summary>
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
            Physics2D.IgnoreCollision(capsule, other.gameObject.GetComponent<CapsuleCollider2D>());
    }

    /// <summary> Checks collisions with the player's trigger and kills itself accordingly. </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            if (other.gameObject.GetComponent<Rigidbody2D>().velocity.y < -1f)
                isDead = true;
    }
}
