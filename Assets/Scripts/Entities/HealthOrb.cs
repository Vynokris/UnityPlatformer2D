using UnityEngine;

public class HealthOrb : MonoBehaviour
{
    [SerializeField] private float pickUpDistance = 2f;
    [SerializeField] private float attractSpeed = 0.1f;
    public Transform playerTransform;

    private Vector2 originPos;
    private float   movementAmplitude = 0.1f;

    void Start()
    {
        originPos = transform.position;
    }

    void Update()
    {
        Move();
        GoToPlayer();
    }

    void Move()
    {
        transform.position = new Vector2(originPos.x + Mathf.Cos(Time.time) * movementAmplitude, 
                                         originPos.y + Mathf.Sin(Time.time) * movementAmplitude);
    }

    void GoToPlayer()
    {
        if ((playerTransform.position - transform.position).magnitude <= pickUpDistance)
        {
            Vector2 moveVec = (playerTransform.position - transform.position).normalized * attractSpeed;
            originPos += moveVec;
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}
