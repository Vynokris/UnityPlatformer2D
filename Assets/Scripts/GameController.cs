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
    [HideInInspector] public UnityEvent showGameOver;

    private bool     transitionOpening = true;
    private float    transitionMaxScale = 14;
    private Animator transitionAnimator;
    public  Cooldown transitionTimer { get; private set; } = new Cooldown(1f);

    void Start()
    {
        if (playerHealed  == null) playerHealed  = new UnityEvent();
        if (playerDamaged == null) playerDamaged = new UnityEvent();
        if (playerDied    == null) playerDied    = new UnityEvent();
        if (showGameOver  == null) showGameOver  = new UnityEvent();

        playerDied.AddListener(StartTransition);

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
                if (Input.GetKeyDown(KeyCode.Space))
                    ReloadScene();
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
    void StartTransition()
    {
        transitionOpening = false;
        transitionTimer.Reset();
    }

    // Reload the game scene.
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
