using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text recordText;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text resultRecordText;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;
    [SerializeField] private GameObject soundButton;
    [SerializeField] private GameObject musicButton;

    [Header("UI Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    public static UIManager instance;

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

    private void OnEnable()
    {
        GameManager.instance.OnGameReset += ResetScore;
        GameManager.instance.OnGameStateChanged += HandleGameStateChanged;

        GameManager.instance.OnCountdownTick += UpdateCountdown;
        GameManager.instance.OnCountdownFinished += HideCountdown;

        if (Player.instance != null)
        {
            Player.instance.OnDied += UpdateScoresOnDeath;
            Player.instance.OnScoreCollected += HandleScoreCollected;
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.OnGameReset -= ResetScore;
            GameManager.instance.OnGameStateChanged -= HandleGameStateChanged;

            GameManager.instance.OnCountdownTick -= UpdateCountdown;
            GameManager.instance.OnCountdownFinished -= HideCountdown;
        }
        if (Player.instance != null)
        {
            Player.instance.OnDied -= UpdateScoresOnDeath;
            Player.instance.OnScoreCollected -= HandleScoreCollected;
        }
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Menu:
                ShowMenu();
                break;
            case GameManager.GameState.Countdown:
                ShowСountdown();
                break;
            case GameManager.GameState.Playing:
                ShowGameplayUI();
                break;
            case GameManager.GameState.Paused:
                ShowPauseMenu();
                break;
            case GameManager.GameState.GameOver:
                ShowGameOverScreen();
                break;
        }
    }

    private void ShowMenu()
    {
        menuPanel.SetActive(true);
        countdownPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void ShowСountdown()
    {
        menuPanel.SetActive(false);
        countdownPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void UpdateCountdown(int number)
    {
        countdownText.text = number.ToString();
    }

    private void HideCountdown()
    {
        countdownPanel.SetActive(false);
    }

    private void ShowGameplayUI()
    {
        menuPanel.SetActive(false);
        countdownPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void ShowPauseMenu()
    {
        menuPanel.SetActive(false);
        countdownPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    private void ShowGameOverScreen()
    {
        menuPanel.SetActive(false);
        countdownPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private void ResetScore()
    {
        UpdateScores(0, ScoreManager.instance.GetRecord());
    }

    private void UpdateScoresOnDeath()
    {
        UpdateScores(ScoreManager.instance.GetCurrentScore(), ScoreManager.instance.GetRecord());
    }

    public void UpdateScores(int current, int best)
    {
        scoreText.text = current.ToString();
        recordText.text = $"Record: {best}";
        resultText.text = $"Result: {current}";
        resultRecordText.text = $"Record: {best}";
    }

    private void HandleScoreCollected()
    {
        int currentScore = ScoreManager.instance.GetCurrentScore();
        int record = ScoreManager.instance.GetRecord();
        UpdateScores(currentScore, record);
    }

    public void ToggleSound()
    {
        AudioManager.instance.ToggleSound();
        soundButton.GetComponent<Image>().sprite = AudioManager.instance.IsSFXOn
            ? soundOnSprite
            : soundOffSprite;
    }

    public void ToggleMusic()
    {
        AudioManager.instance.ToggleMusic();
        musicButton.GetComponent<Image>().sprite = AudioManager.instance.IsMusicOn
            ? musicOnSprite
            : musicOffSprite;
    }
}
