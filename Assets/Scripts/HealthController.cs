using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float            hpSpawnDuration = 0.5f;
    [SerializeField] private GameController   gameController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject       hudHealthPrefab;

    private List<GameObject> healthPoints = new List<GameObject>();

    public  int currentHP { get; private set; } = -1;
    private Cooldown hpSpawnTimer = new Cooldown(0.5f);

    void Start()
    {
        if (hpSpawnDuration >= 0)
            hpSpawnTimer.ChangeDuration(hpSpawnDuration);

        gameController.playerDamaged.AddListener(OnPlayerDamaged);
        gameController.playerHealed .AddListener(OnPlayerHealed);
        gameController.reloadScene  .AddListener(OnReloadScene);

        StartCoroutine(AddAllHP());
    }

    void Update()
    {
    }
    
    IEnumerator AddHP()
    {
        while (true)
        {
            // Add a new health point.
            if (hpSpawnTimer.Counter == hpSpawnTimer.Duration)
            {
                GameObject newHp = Instantiate(hudHealthPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
                newHp.GetComponent<HudHealthPoint>().SetIndex(healthPoints.Count);
                newHp.transform.localScale = new Vector3(0, 0, 1);
                healthPoints.Add(newHp);
                currentHP++;
            }

            hpSpawnTimer.Update(Time.deltaTime);

            // Update the current health point's scale.
            if (!hpSpawnTimer.HasEnded())
            {
                float ratio = 1 - hpSpawnTimer.CompletionRatio();
                healthPoints[currentHP].transform.localScale = new Vector3(ratio, ratio, 1);
            }

            // When the timer ends, reset it and end the coroutine.
            else
            {
                hpSpawnTimer.Reset();
                break;
            }

            yield return null;
        }

        StopCoroutine(AddHP());
    }

    IEnumerator AddAllHP()
    {
        // Wait until the scene opening transition hase ended.
        while (!gameController.transitionTimer.HasEnded())
            yield return null;

        // Spawn all of the player's health points.
        for (int i = 0; i < playerController.Health; i++)
        {
            // Wait until it's the first frame of the spawn timer.
            while (!(hpSpawnTimer.Counter == hpSpawnTimer.Duration))
            {
                yield return null;
            }

            // Add a new health point.
            StartCoroutine(AddHP());
            yield return null;
        }

        StopCoroutine(AddAllHP());
    }

    IEnumerator DelHP()
    {
        while (currentHP >= 0)
        {
            // Update the current health point's scale.
            if (!hpSpawnTimer.Update(Time.deltaTime))
            {
                float ratio = hpSpawnTimer.CompletionRatio();
                healthPoints[currentHP].transform.localScale = new Vector3(ratio, ratio, 1);
            }

            // When the timer ends, reset it and destroy the current hp.
            else
            {
                currentHP--;
                hpSpawnTimer.Reset();
                Destroy(healthPoints[healthPoints.Count-1]);
                healthPoints.Remove(healthPoints[healthPoints.Count-1]);
                break;
            }

            yield return null;
        }

        StopCoroutine(DelHP());
    }

    IEnumerator DelAllHP()
    {
        while (true)
        {
            // Update all of the health points' scales.
            if (!hpSpawnTimer.Update(Time.deltaTime))
            {
                float ratio = hpSpawnTimer.CompletionRatio();
                foreach (GameObject healthPoint in healthPoints)
                    healthPoint.transform.localScale = new Vector3(ratio, ratio, 1);
            }

            // When the timer ends, reset it and destroy all the health points.
            else
            {
                hpSpawnTimer.Reset();
                for (int i = healthPoints.Count; i > 0; i--)
                {
                    Destroy(healthPoints[healthPoints.Count-1]);
                    healthPoints.Remove(healthPoints[healthPoints.Count-1]);
                }
                break;
            }

            yield return null;
        }

        StopCoroutine(DelAllHP());
    }

    void OnPlayerDamaged()
    {
        StartCoroutine(DelHP());
    }

    void OnPlayerHealed()
    {
        StartCoroutine(AddHP());
    }

    void OnReloadScene()
    {
        StartCoroutine(DelAllHP());
    }
}
