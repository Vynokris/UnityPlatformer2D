using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private float timeBeforeFalling = 2f;
    [SerializeField] private float timeToRespawn     = 4f;

    private bool     touchedByPlayer = false;
    private bool     falling         = false;
    private Cooldown fallCooldown    = new Cooldown(2f);
    private Cooldown respawnCooldown = new Cooldown(4f);

    private Vector3    originalPos;
    private Quaternion originalRot;

    private Rigidbody2D   rigidBody;
    private BoxCollider2D boxCollider;
    private Renderer      renderer;

    void Start()
    {
        if (timeBeforeFalling >= 0)
            fallCooldown.ChangeDuration(timeBeforeFalling);
        if (timeToRespawn >= 0)
            respawnCooldown.ChangeDuration(timeToRespawn);

        originalPos = transform.position;
        originalRot = transform.rotation;

        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody   = GetComponent<Rigidbody2D>();
        renderer    = GetComponent<Renderer>();
    }

    void Update()
    {
        if (!falling)
        {
            rigidBody.velocity = new Vector2(0, 0);

            if (touchedByPlayer && fallCooldown.Update(Time.deltaTime))
                StartFalling();
        }
        else
        {
            if (respawnCooldown.Update(Time.deltaTime))
                Reset();
        }
    }


    /// <summary> Enables gravity effects on the platform. </summary>
    void StartFalling()
    {
        rigidBody.gravityScale = 1f;
        rigidBody.mass = 1f;
        falling = true;
    }

    /// <summary> Hides the platform, stops its movement and disable collisions. </summary>
    void Hide()
    {
        boxCollider.enabled = false;
        rigidBody.gravityScale = 0f;
        rigidBody.velocity = new Vector2(0, 0);
        rigidBody.angularVelocity = 0;
        renderer.material.SetColor("_Color", new Color(1, 1, 1, 0));
    }

    /// <summary> Resets the platform to its original state and transform. </summary>
    void Reset()
    {
        transform.position     = originalPos;
        transform.rotation     = originalRot;
        rigidBody.gravityScale = 0f;
        rigidBody.mass         = 10000f;
        touchedByPlayer        = false;
        falling                = false;
        boxCollider.enabled    = true;
        fallCooldown.Reset();
        respawnCooldown.Reset();
        renderer.material.SetColor("_Color", new Color(1, 1, 1, 1));
    }


    /// <summary> Handles collisions with the player and ground. </summary>
    void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player")
            touchedByPlayer = true;

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            Hide();
    }
}
