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
    [HideInInspector] public UnityEvent gameEnded;

    public static bool     isGameFinished { get; private set; } = false;
    public static float    gameTimer      { get; private set; } = 0f;
    public static int      respawnCount   { get; private set; } = 0;

    private bool     transitionOpening  = true;
    private float    transitionMaxScale = 14;

    private Animator transitionAnimator;
    public  Cooldown transitionTimer { get; private set; } = new Cooldown(1f);

    void Start()
    {
        if (playerHealed  == null) playerHealed  = new UnityEvent();
        if (playerDamaged == null) playerDamaged = new UnityEvent();
        if (playerDied    == null) playerDied    = new UnityEvent();
        if (reloadScene   == null) reloadScene   = new UnityEvent();
        if (gameEnded     == null) gameEnded     = new UnityEvent();

        playerDied.AddListener(StartTransition);
        gameEnded .AddListener(OnGameEnd);

        if (galaxyTransition != null) transitionAnimator = galaxyTransition.GetComponent<Animator>();
        if (transitionDuration > 0)   transitionTimer.ChangeDuration(transitionDuration);

        Cursor.visible = false;
    }

    void Update()
    {
        if (!isGameFinished)
            gameTimer += Time.deltaTime;
        transitionTimer.Update(Time.deltaTime);

        // At the end of the transition, reload the scene.
        if (transitionTimer.HasEnded())
        {
            if (!transitionOpening)
                ReloadScene();

            else
                galaxyTransition.transform.localScale = new Vector2(0, 0);
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
            if (isGameFinished)
                ResetGame();
            StartTransition();
            reloadScene.Invoke();
        }

        // If the user pressed Esc, quit the game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary> Start the scene closing transition. </summary>
    void StartTransition()
    {
        transitionOpening = false;
        transitionTimer.Reset();
    }

    /// <summary> Reload the game scene. </summary>
    void ReloadScene()
    {
        respawnCount++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary> Ends the game and enables the player to restart. </summary>
    void OnGameEnd()
    {
        isGameFinished = true;
    }

    /// <summary> Resets the game counters and player spawn position. </summary>
    void ResetGame()
    {
        PlayerController.spawnPos = new Vector2(0, 0);
        gameTimer = 0f;
        respawnCount = 0;
    }
}
