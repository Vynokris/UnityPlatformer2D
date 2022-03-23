using UnityEngine;

public class JumpPlant : MonoBehaviour
{
    [SerializeField] private float boopForce = 17;
    [SerializeField] private float boopCooldownTime = 0.5f;

    private bool     boopedPlayer = false;
    private Cooldown boopCooldown = new Cooldown(0.5f);
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (boopCooldownTime > 0)
            boopCooldown.ChangeDuration(boopCooldownTime);
    }

    // Update is called once per frame
    void Update()
    {
        boopCooldown.Update(Time.deltaTime);

        // Reset the boopedPlayer variable.
        if (boopCooldown.Duration - boopCooldown.Counter > 0.5f && boopedPlayer) 
        {
            boopedPlayer = false;
            animator.SetBool("BoopedPlayer", boopedPlayer);
        }
    }

    /// <summary>
    /// Checks collisions with the enemies and jumps if an enemy was killed.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();

            if (boopCooldown.HasEnded() && player.rigidBody.velocity.y < -1)
            {
                // Reset the boop cooldown, update the animator and start a knockback on the player.
                boopCooldown.Reset();
                boopedPlayer = true;
                animator.SetBool("BoopedPlayer", boopedPlayer);
                player.StartKnockBack(Vector2.up * boopForce);
            }
        }
    }
}
