using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float            hpSpawnDuration = 0.5f;
    [SerializeField] private GameController   gameController;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject       hudHealthPrefab;

    private List<GameObject> healthPoints  = new List<GameObject>();

    private Cooldown hpSpawnTimer = new Cooldown(0.5f);
    public  int        spawningHp = 0;

    void Start()
    {
        if (hpSpawnDuration >= 0)
            hpSpawnTimer.ChangeDuration(hpSpawnDuration);

        for (int i = 0; i < playerController.Health; i++)
            AddHp();

        gameController.playerDamaged.AddListener(RemoveHp);
        gameController.playerHealed .AddListener(AddHp);
    }

    void Update()
    {
        if (gameController.transitionTimer.HasEnded())
        {
            if (spawningHp < healthPoints.Count)
            {
                if (!hpSpawnTimer.Update(Time.deltaTime))
                {
                    healthPoints[spawningHp].transform.localScale = new Vector3(1 - hpSpawnTimer.CompletionRatio(), 
                                                                                1 - hpSpawnTimer.CompletionRatio(), 1);
                }
                else
                {
                    hpSpawnTimer.Reset();
                    spawningHp++;
                }
            }
        }
    }

    void RemoveHp()
    {
        spawningHp--;
        Destroy(healthPoints[healthPoints.Count-1]);
        healthPoints.Remove(healthPoints[healthPoints.Count-1]);
    }

    void AddHp()
    {
        GameObject newHp = Instantiate(hudHealthPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
        newHp.GetComponent<HudHealthPoint>().SetIndex(healthPoints.Count);
        newHp.transform.localScale = new Vector3(0, 0, 1);
        healthPoints.Add(newHp);
    }
}
