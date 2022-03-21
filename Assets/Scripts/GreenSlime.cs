using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSlime : MonoBehaviour
{
    [SerializeField][Range(-1, 1)] private int walkDir = -1;
    [SerializeField] private float walkSpeed;
    [SerializeField] private LayerMask collisionLayer;

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
        Walk();
    }

    void Walk()
    {
        if (!IsWallFwd() && IsGroundFwd()) 
        {
            rigidBody.velocity = new Vector2(walkDir, rigidBody.velocity.y);
        }
        else 
        {
            walkDir *= -1;
            transform.localScale *= new Vector2(-1, 1);
        }
    }

    /// <summary>
    /// Returns true if a wall is in front of the slime.
    /// </summary>
    bool IsWallFwd()
    {
        return Physics2D.CapsuleCast(capsule.bounds.center + new Vector3(0, 0.1f, 0), capsule.bounds.size, capsule.direction, 0, new Vector2(walkDir, 0), 0.1f, collisionLayer).collider != null;
    }

    /// <summary>
    /// Returns true if there is ground in front of the slime.
    /// </summary>
    bool IsGroundFwd()
    {
        return Physics2D.CapsuleCast(capsule.bounds.center, capsule.bounds.size, capsule.direction, 0, new Vector2(walkDir * 5, -1), 1f, collisionLayer).collider != null;
    }
}
