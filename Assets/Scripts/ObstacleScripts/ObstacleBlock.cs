using System;
using UnityEngine;

public class ObstacleBlock : MonoBehaviour
{
    public static float speed;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        if (transform.position.x < -15)
        {
            Destroy(gameObject);
        }
    }
}
