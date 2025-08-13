using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState
    {
        Menu,
        Countdown,
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnGameStateChanged;
    public event Action<int> OnCountdownTick;
    public event Action OnCountdownFinished;
    public event Action OnGameReset;

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
        if (Player.instance != null)
        {
            Player.instance.OnDied += HandlePlayerDied;
        }
        else
        {
            StartCoroutine(WaitForPlayer());
        }
        ChangeState(GameState.Menu);
    }

    private IEnumerator WaitForPlayer()
    {
        while (Player.instance == null)
            yield return null;
        Player.instance.OnDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        if (Player.instance != null)
        {
            Player.instance.OnDied -= HandlePlayerDied;
        }
    }

    private void HandlePlayerDied()
    {
        EndGame();
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        Time.timeScale = (CurrentState == GameState.Playing) ? 1f : 0f;

        OnGameStateChanged?.Invoke(CurrentState);
    }

    public void StartGame()
    {
        ResetGame();
        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine()
    {
        ChangeState(GameState.Countdown);

        int counter = 3;
        while (counter > 0)
        {
            OnCountdownTick?.Invoke(counter);
            yield return new WaitForSecondsRealtime(1f);
            counter--;
        }

        OnCountdownFinished?.Invoke();
        ChangeState(GameState.Playing);
    }

    public void PauseGame() => ChangeState(GameState.Paused);
    public void ResumeGame() => ChangeState(GameState.Playing);
    public void EndGame() => ChangeState(GameState.GameOver);
    public void BackToMenu() => ChangeState(GameState.Menu);

    private void ResetGame()
    {
        OnGameReset?.Invoke();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
