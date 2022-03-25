using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    [SerializeField] private float      transitionDuration = 1f;
    [SerializeField] private GameObject galaxyTransition;

    [HideInInspector] public UnityEvent playerHealed;
    [HideInInspector] public UnityEvent playerDamaged;
    [HideInInspector] public UnityEvent playerDied;
    [HideInInspector] public UnityEvent reloadScene;

    private bool     transitionOpening  = true;
    private bool     promptRestart      = true;
    private float    transitionMaxScale = 14;
    private Animator transitionAnimator;
    public  Cooldown transitionTimer { get; private set; } = new Cooldown(1f);

    void Start()
    {
        if (playerHealed  == null) playerHealed  = new UnityEvent();
        if (playerDamaged == null) playerDamaged = new UnityEvent();
        if (playerDied    == null) playerDied    = new UnityEvent();
        if (reloadScene   == null) reloadScene   = new UnityEvent();

        playerDied.AddListener(StartTransition);

        if (galaxyTransition != null) transitionAnimator = galaxyTransition.GetComponent<Animator>();
        if (transitionDuration > 0)   transitionTimer.ChangeDuration(transitionDuration);
    }

    void Update()
    {
        transitionTimer.Update(Time.deltaTime);

        // At the end of the transition, reload the scene.
        if (transitionTimer.HasEnded())
        {
            if (!transitionOpening)
            {
                if (promptRestart) {
                    if (Input.GetKeyDown(KeyCode.Space))
                        ReloadScene();
                }
                else {
                    ReloadScene();
                }
            }
            else
            {
                galaxyTransition.transform.localScale = new Vector2(0, 0);
            }
        }

        // Update the galaxy's scale.
        else
        {
            float completionRatio = transitionTimer.CompletionRatio() * transitionMaxScale;
            if (!transitionOpening)
                completionRatio = transitionMaxScale - completionRatio;
            
            galaxyTransition.transform.localScale = new Vector2(completionRatio, completionRatio);
        }

        // If the user pressed R, reload the scene.
        if (Input.GetKeyDown(KeyCode.R) && transitionTimer.HasEnded())
        {
            StartTransition();
            promptRestart = false;
            reloadScene.Invoke();
        }
    }

    // Start the scene closing transition.
    void StartTransition()
    {
        promptRestart = true;
        transitionOpening = false;
        transitionTimer.Reset();
    }

    // Reload the game scene.
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
