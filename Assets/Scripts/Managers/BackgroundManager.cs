using UnityEngine;
using System.Collections;

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager instance;

    [Header("Background Sprites")]
    [SerializeField] private Sprite classicBackground;
    [SerializeField] private Sprite invertedGravityBackground;
    [SerializeField] private Sprite iceBackground;
    [SerializeField] private Sprite narrowPassagesBackground;
    [SerializeField] private Sprite earthquakeBackground;
    [SerializeField] private Sprite movingObstaclesBackground;
    [SerializeField] private Sprite angledPassagesBackground;
    [SerializeField] private Sprite playerSizeChangeBackground;

    [Header("References")]
    [SerializeField] private ParallaxLoop parallaxLoop;

    private SpriteRenderer sr;

    private Coroutine transitionCoroutine;
    private float transitionDuration = 0.5f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        sr = GetComponent<SpriteRenderer>();
    }

    public IEnumerator SetBackgroundCoroutine(GameplayPhase phase)
    {
        Sprite newSprite = GetSpriteForPhase(phase);

        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        transitionCoroutine = StartCoroutine(FadeTransition(newSprite));
        yield return transitionCoroutine;
    }

    public void SetBackgroundInstant(GameplayPhase phase)
    {
        Sprite newSprite = GetSpriteForPhase(phase);

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        var layer1 = parallaxLoop.GetLayer1();
        var layer2 = parallaxLoop.GetLayer2();

        parallaxLoop.SetLayerSprite(layer1, newSprite);
        parallaxLoop.SetLayerSprite(layer2, newSprite);

        parallaxLoop.SetLayerAlpha(layer1, 1f);
        parallaxLoop.SetLayerAlpha(layer2, 1f);

        if (sr != null)
            sr.sprite = newSprite;
    }



    private Sprite GetSpriteForPhase(GameplayPhase phase)
    {
        switch (phase)
        {
            case GameplayPhase.Classic: return classicBackground;
            case GameplayPhase.InvertedGravity: return invertedGravityBackground;
            case GameplayPhase.Ice: return iceBackground;
            case GameplayPhase.NarrowPassages: return narrowPassagesBackground;
            case GameplayPhase.Earthquake: return earthquakeBackground;
            case GameplayPhase.MovingObstacles: return movingObstaclesBackground;
            case GameplayPhase.AngledPassages: return angledPassagesBackground;
            case GameplayPhase.PlayerSizeChange: return playerSizeChangeBackground;
            default: return classicBackground;
        }
    }

    private IEnumerator FadeTransition(Sprite newSprite)
    {
        var layer1 = parallaxLoop.GetLayer1();
        var layer2 = parallaxLoop.GetLayer2();

        parallaxLoop.SetLayerAlpha(layer2, 0f);

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / transitionDuration);
            parallaxLoop.SetLayerAlpha(layer1, alpha);
            yield return null;
        }

        parallaxLoop.SetLayerSprite(layer2, newSprite);

        elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / transitionDuration);
            parallaxLoop.SetLayerAlpha(layer2, alpha);
            yield return null;
        }

        parallaxLoop.SetLayerAlpha(layer1, 0f);
        parallaxLoop.SetLayerAlpha(layer2, 1f);

        parallaxLoop.SwapLayers();
    }
}
