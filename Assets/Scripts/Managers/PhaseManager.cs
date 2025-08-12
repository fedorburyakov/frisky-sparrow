using UnityEngine;
using System.Collections;

public class PhaseManager : MonoBehaviour
{
    public static PhaseManager instance;

    [SerializeField] private float phaseDuration = 15f;

    private GameplayPhase currentPhase;
    private Coroutine phaseCoroutine;
    private bool isPlayerInDangerZone;

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

    void Start()
    {
        phaseCoroutine = StartCoroutine(PhaseRoutine());
    }

    private void OnEnable()
    {
        GameManager.instance.OnGameReset += ResetToClassicPhase;
        if (Player.instance != null)
        {
            Player.instance.OnDangerZoneChanged += OnDangerZoneChanged;
        }
    }

    private void OnDisable()
    {
        GameManager.instance.OnGameReset -= ResetToClassicPhase;
        if (Player.instance != null)
        {
            Player.instance.OnDangerZoneChanged -= OnDangerZoneChanged;
        }
    }

    IEnumerator PhaseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(phaseDuration);

            GameplayPhase newPhase;
            do
            {
                newPhase = (GameplayPhase)Random.Range(0, System.Enum.GetValues(typeof(GameplayPhase)).Length);
            }
            while (newPhase == currentPhase);

            yield return StartCoroutine(PhaseIndicator.instance.ShowPhaseAndWait(newPhase));

            yield return StartCoroutine(WaitForSafeZone());

            StartCoroutine(SetPhaseRoutine(newPhase));
        }
    }


    private IEnumerator WaitForSafeZone()
    {
        while (isPlayerInDangerZone)
        {
            yield return null;
        }
    }

    IEnumerator SetPhaseRoutine(GameplayPhase phase)
    {
        yield return BackgroundManager.instance.SetBackgroundCoroutine(phase);

        SetPhaseEffects(phase);
    }

    void SetPhaseEffects(GameplayPhase phase)
    {
        currentPhase = phase;


        ResetAllEffects();

        switch (phase)
        {
            case GameplayPhase.Classic:
                break;

            case GameplayPhase.InvertedGravity:
                Player.instance.SetInvertedGravity(true);
                break;

            case GameplayPhase.Ice:
                Player.instance.SetIceMode(true);
                break;

            case GameplayPhase.NarrowPassages:
                ObstacleSpawner.instance.SetNarrowPassages(true);
                break;

            case GameplayPhase.Earthquake:
                CameraShake.instance.StartShaking();
                break;

            case GameplayPhase.MovingObstacles:
                ObstacleSpawner.instance.EnableMovingObstacles(true);
                break;

            case GameplayPhase.AngledPassages:
                ObstacleSpawner.instance.EnableAngledPassages(true);
                break;

            case GameplayPhase.PlayerSizeChange:
                Player.instance.RandomSize();
                break;
        }
    }

    public void ResetAllEffects()
    {
        Player.instance.SetInvertedGravity(false);
        Player.instance.SetIceMode(false);
        Player.instance.ResetSize();

        ObstacleSpawner.instance.SetNarrowPassages(false);
        ObstacleSpawner.instance.EnableMovingObstacles(false);
        ObstacleSpawner.instance.EnableAngledPassages(false);

        CameraShake.instance.StopShaking();
    }

    public void ResetToClassicPhase()
    {
        if (phaseCoroutine != null)
        {
            StopCoroutine(phaseCoroutine);
            phaseCoroutine = null;
        }

        ResetAllEffects();
        currentPhase = GameplayPhase.Classic;

        BackgroundManager.instance.SetBackgroundInstant(currentPhase);
        PhaseIndicator.instance.HideInstant();

        phaseCoroutine = StartCoroutine(PhaseRoutine());
    }

    private void OnDangerZoneChanged(bool inDanger)
{
    isPlayerInDangerZone = inDanger;
}
}
