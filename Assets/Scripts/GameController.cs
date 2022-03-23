using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    [SerializeField] private float      transitionDuration = 1f;
    [SerializeField] private GameObject galaxyTransition;

    [HideInInspector] public UnityEvent playerDied;
    private bool     transitionOpening = true;
    private float    transitionMaxScale = 12;
    private Animator transitionAnimator;
    private Cooldown transitionTimer = new Cooldown(1f);

    void Start()
    {
        if (playerDied == null) playerDied = new UnityEvent();
        playerDied.AddListener(ReloadScene);

        if (galaxyTransition != null) transitionAnimator = galaxyTransition.GetComponent<Animator>();
        if (transitionDuration > 0)   transitionTimer.ChangeDuration(transitionDuration);
    }

    void Update()
    {
        transitionTimer.Update(Time.deltaTime);

        if (transitionTimer.HasEnded())
        {
            if (!transitionOpening)
            {
                transitionTimer.Reset();
                transitionOpening = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                galaxyTransition.transform.localScale = new Vector2(0, 0);
            }
        }

        else
        {
            float completionRatio = transitionTimer.CompletionRatio() * transitionMaxScale;
            if (!transitionOpening)
                completionRatio = transitionMaxScale - completionRatio;
            
            galaxyTransition.transform.localScale = new Vector2(completionRatio, completionRatio);
        }
    }

    // Reload the game scene.
    void ReloadScene()
    {
        transitionOpening = false;
        transitionTimer.Reset();
    }
}
