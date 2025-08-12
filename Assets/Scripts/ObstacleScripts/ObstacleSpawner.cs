using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner instance;

    [Header("References")]
    [SerializeField] private GameObject[] obstacleVariants;

    [Header("Spawn Attributes")]
    [SerializeField] private float spawnCooldown = 2f;
    [SerializeField] private float verticalSpawnRange = 1.5f;
    [SerializeField] private float defaultGap = 2.2f;

    [Header("Phase Attributes")]
    [SerializeField] private float narrowGap = 1.3f;
    [SerializeField] private float angleMax = 10f;
    [SerializeField] private float moveAmplitude = 0.4f;
    [SerializeField] private float moveSpeed = 1f;

    private float cooldownTimer;
    private bool narrowPassages = false;
    private bool movingObstacles = false;
    private bool angledPassages = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameManager.instance.OnGameReset += ClearObstacles;
    }

    private void OnDisable()
    {
        GameManager.instance.OnGameReset -= ClearObstacles;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            SpawnObstacle();
            cooldownTimer = spawnCooldown;
        }
    }

    private void SpawnObstacle()
    {
        int randomIndex = Random.Range(0, obstacleVariants.Length);
        GameObject obstacle = Instantiate(obstacleVariants[randomIndex], transform.position, Quaternion.identity);

        float randomY = Random.Range(-verticalSpawnRange, verticalSpawnRange);
        obstacle.transform.position += new Vector3(0, randomY, 0);

        float gap = narrowPassages ? narrowGap : defaultGap;
        ScaleObstacleGap(obstacle, gap);

        if (angledPassages)
        {
            float randomAngle = Random.Range(-angleMax, angleMax);
            obstacle.transform.rotation = Quaternion.Euler(0, 0, randomAngle);
        }

        if (movingObstacles)
        {
            var mover = obstacle.AddComponent<ObstacleMover>();
            mover.SetMovement(moveAmplitude, moveSpeed);
        }

        obstacle.transform.SetParent(transform);
    }

    private void ScaleObstacleGap(GameObject obstacle, float gap)
    {
        Transform top = obstacle.transform.Find("Top");
        Transform bottom = obstacle.transform.Find("Bottom");

        if (top != null && bottom != null)
        {
            float middleY = (top.position.y + bottom.position.y) / 2f;
            top.position = new Vector3(top.position.x, middleY + gap / 2f, top.position.z);
            bottom.position = new Vector3(bottom.position.x, middleY - gap / 2f, bottom.position.z);
        }
    }
    public void SetNarrowPassages(bool value)
    {
        narrowPassages = value;
    }

    public void EnableMovingObstacles(bool value)
    {
        movingObstacles = value;
    }

    public void EnableAngledPassages(bool value)
    {
        angledPassages = value;
    }

private void ClearObstacles()
{
    foreach (var obstacle in FindObjectsByType<ObstacleBlock>(FindObjectsSortMode.None))
        Destroy(obstacle.gameObject);
}
}
