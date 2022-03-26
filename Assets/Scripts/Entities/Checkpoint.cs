using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool playerStartsHere = false;
    private Animator animator;

    void Start()
    {
        if (playerStartsHere && PlayerController.spawnPos == new Vector2(0, 0))
            PlayerController.spawnPos = transform.position;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            animator.SetBool("WasUsed", true);
            PlayerController.spawnPos = transform.position;
        }
    }
}
