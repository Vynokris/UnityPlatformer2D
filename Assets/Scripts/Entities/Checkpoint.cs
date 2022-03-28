using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private AudioController audioController;
    [SerializeField] private bool playerStartsHere  = false;
    [SerializeField] private bool checkPointEnabled = true;

    private bool     wasUsed = false;
    private Animator animator;
    private Renderer objectRenderer;

    void Start()
    {
        if (playerStartsHere && PlayerController.spawnPos == new Vector2(0, 0))
            PlayerController.spawnPos = transform.position;

        animator       = GetComponent<Animator>();
        objectRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Hide the checkpoint if speedrun mode is enabled.
        if (GameController.speedrunMode && checkPointEnabled) 
        {
            wasUsed = false;
            checkPointEnabled = false;
            objectRenderer.material.SetColor("_Color", new Color(1, 1, 1, 0));
        }

        // Show the checkpoint if speedrun mode is disabled.
        else if (!GameController.speedrunMode && !checkPointEnabled) 
        {
            checkPointEnabled = true;
            objectRenderer.material.SetColor("_Color", new Color(1, 1, 1, 1));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !wasUsed && !GameController.speedrunMode)
        {
            wasUsed = true;
            PlayCheckpointSound();
            animator.SetBool("WasUsed", true);
            PlayerController.spawnPos = transform.position;
        }
    }


    /// <summary> Plays the checkpoint sound. </summary>
    void PlayCheckpointSound()
    {
        audioController.Play("Checkpoint");
    }
}
