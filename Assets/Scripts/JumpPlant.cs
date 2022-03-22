using UnityEngine;

public class JumpPlant : MonoBehaviour
{
    private bool     boopedPlayer = false;
    private Cooldown boopCooldown = new Cooldown(0.5f);
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boopCooldown.Update(Time.deltaTime) && boopedPlayer) 
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
            if (boopCooldown.HasEnded())
            {
                boopCooldown.Reset();
                boopedPlayer = true;
                animator.SetBool("BoopedPlayer", boopedPlayer);
            }
        }
    }
}
