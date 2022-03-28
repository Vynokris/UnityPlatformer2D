using TMPro;
using UnityEngine;

public class GameEndText : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        textMeshPro.text = $"FINAL TIME:\n{Mathf.Round(GameController.gameTimer)}s\n\nRESPAWNED {GameController.respawnCount} TIMES";
    }
}
