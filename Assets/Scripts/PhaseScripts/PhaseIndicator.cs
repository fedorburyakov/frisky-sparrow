using UnityEngine;
using System.Collections;

public class PhaseIndicator : MonoBehaviour
{
    public static PhaseIndicator instance;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer iconMain;
    [SerializeField] private SpriteRenderer iconTrailPrefab;
    [SerializeField] private Transform trailSpawnPoint;

    [Header("Phase Colors")]
    [SerializeField] private Color classicColor;
    [SerializeField] private Color invertedGravityColor;
    [SerializeField] private Color iceColor;
    [SerializeField] private Color narrowPassagesColor;
    [SerializeField] private Color earthquakeColor;
    [SerializeField] private Color movingObstaclesColor;
    [SerializeField] private Color angledPassagesColor;
    [SerializeField] private Color playerSizeChangeColor;

    [Header("Movement")]
    [SerializeField] private float flySpeed = 5f;
    [SerializeField] private float trailSpawnInterval = 0.1f;

    private Color currentColor;
    private Camera mainCam;
    private Coroutine moveCoroutine;
    private Coroutine trailCoroutine;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        mainCam = Camera.main;
        gameObject.SetActive(false);
        currentColor = classicColor;
    }

    public void ShowPhase(GameplayPhase phase)
    {
        StopAllCoroutines();
        gameObject.SetActive(true);

        moveCoroutine = StartCoroutine(MoveAcrossScreen());
        trailCoroutine = StartCoroutine(SpawnTrail());
    }

    private IEnumerator MoveAcrossScreen()
    {
        transform.localScale = new Vector3(Vector3.one.x, Player.instance.InvertedGravity ? -Vector3.one.y : Vector3.one.y, Vector3.one.z);
        Vector3 start = mainCam.ViewportToWorldPoint(new Vector3(-0.1f, Player.instance.InvertedGravity ? 0.2f : 0.8f, 10f));
        Vector3 end = mainCam.ViewportToWorldPoint(new Vector3(1.1f, Player.instance.InvertedGravity ? 0.2f : 0.8f, 10f));
        start.z = 0;
        end.z = 0;
        transform.position = start;

        while (transform.position.x < end.x)
        {
            transform.position += Vector3.right * flySpeed * Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);

        if (trailCoroutine != null)
            StopCoroutine(trailCoroutine);
    }

    private IEnumerator SpawnTrail()
    {
        while (true)
        {
            Vector3 spawnPos = trailSpawnPoint.position;

            var trailPart = Instantiate(iconTrailPrefab.gameObject, spawnPos, Quaternion.identity).GetComponent<SpriteRenderer>();
            trailPart.color = currentColor;

            trailPart.gameObject.AddComponent<TrailParticle>();

            yield return new WaitForSeconds(trailSpawnInterval);
        }
    }



    public IEnumerator ShowPhaseAndWait(GameplayPhase phase)
    {
        StopAllCoroutines();

        gameObject.SetActive(true);

        currentColor = GetColorForPhase(phase);

        Coroutine moveCoroutine = StartCoroutine(MoveAcrossScreen());
        Coroutine trailCoroutine = StartCoroutine(SpawnTrail());

        yield return moveCoroutine;

        if (trailCoroutine != null)
            StopCoroutine(trailCoroutine);

        gameObject.SetActive(false);
    }


    private Color GetColorForPhase(GameplayPhase phase)
    {
        switch (phase)
        {
            case GameplayPhase.Classic: return classicColor;
            case GameplayPhase.InvertedGravity: return invertedGravityColor;
            case GameplayPhase.Ice: return iceColor;
            case GameplayPhase.NarrowPassages: return narrowPassagesColor;
            case GameplayPhase.Earthquake: return earthquakeColor;
            case GameplayPhase.MovingObstacles: return movingObstaclesColor;
            case GameplayPhase.AngledPassages: return angledPassagesColor;
            case GameplayPhase.PlayerSizeChange: return playerSizeChangeColor;
            default: return classicColor;
        }
    }
    
    public void HideInstant()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

}
