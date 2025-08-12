using UnityEngine;

public class TrailParticle : MonoBehaviour
{
    private float duration = 1f;
    private float elapsed = 0f;
    private SpriteRenderer sr;

    private Vector3 moveDirection;

    void Start()  
    {
        sr = GetComponent<SpriteRenderer>();

        moveDirection = new Vector3(-0.5f, 0, 0);
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        transform.position += moveDirection * Time.deltaTime;

        Color c = sr.color;
        c.a = Mathf.Lerp(1f, 0f, elapsed / duration);
        sr.color = c;

        if (elapsed >= duration)
            Destroy(gameObject);
    }
}
