using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    void Start()
    {
        Application.targetFrameRate = 75;
    }

    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}
