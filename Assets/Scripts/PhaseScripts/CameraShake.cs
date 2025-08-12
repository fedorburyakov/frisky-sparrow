using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [Header("Shake Settings")]
    [SerializeField] private float shakeMagnitude = 0.2f;
    [SerializeField] private float shakeFrequency = 20f;

    private Vector3 originalPosition;
    private Coroutine shakeRoutine;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        originalPosition = transform.localPosition;
    }

    public void StartShaking()
    {
        if (shakeRoutine != null) return;
        shakeRoutine = StartCoroutine(ShakeCoroutine());
    }

    public void StopShaking()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }
        transform.localPosition = originalPosition;
    }

    private IEnumerator ShakeCoroutine()
    {
        while (true)
        {
            Vector3 randomOffset = Random.insideUnitCircle * shakeMagnitude;
            transform.localPosition = originalPosition + randomOffset;

            yield return new WaitForSeconds(1f / shakeFrequency);
        }
    }
}
