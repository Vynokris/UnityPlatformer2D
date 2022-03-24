using UnityEngine;

public class HudHealthPoint : MonoBehaviour
{
    public int index = -1;

    private Vector2 originPos;
    private float   movementAmplitude = 10f;
    private float   timeOffset;

    void Start()
    {
        if (index != -1)
        {
            SetIndex(index);
        }
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position = new Vector3(originPos.x, originPos.y + Mathf.Sin((Time.time + timeOffset) * 4) * movementAmplitude, 0);
    }

    public void SetIndex(int _index)
    {
        index = _index;
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(64 + index * 64, -64, 0);
        originPos = transform.position;
        timeOffset = index * 0.2f;
    }
}
