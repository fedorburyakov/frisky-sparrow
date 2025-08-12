using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    private float amplitude;
    private float speed;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    public void SetMovement(float amplitude, float speed)
    {
        this.amplitude = amplitude;
        this.speed = speed;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
