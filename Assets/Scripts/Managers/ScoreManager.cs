using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    private int currentScore = 0;
    private int record = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        record = PlayerPrefs.GetInt("Record", 0);
    }

    private void OnEnable()
    {
        GameManager.instance.OnGameReset += ResetScore;
        if (Player.instance != null)
        {
            Player.instance.OnScoreCollected += AddScore;
        }
    }

    private void OnDisable()
    {
        GameManager.instance.OnGameReset -= ResetScore;
        if (Player.instance != null)
        {
            Player.instance.OnScoreCollected -= AddScore;
        }
    }

    public void AddScore()
    {
        currentScore += 1;

        if (currentScore > record)
        {
            record = currentScore;
            PlayerPrefs.SetInt("Record", record);
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
    }

    public int GetCurrentScore() => currentScore;
    public int GetRecord() => record;
}
