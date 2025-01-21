using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class FishingMinigameConfig
{
    public float barSpeed = 5f;
    public float fishMovementSpeed = 3f;
    public float fishRandomness = 0.5f;
    public float catchZoneSize = 100f;
    public float barSize = 50f;
    public float minigameHeight = 300f;
}

public class FishingMinigame : MonoBehaviour
{
    public static FishingMinigame Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject minigamePanelPrefab;

    [Header("Canvas")]
    [SerializeField] private Canvas gameCanvas;

    [Header("Configuration")]
    [SerializeField] private FishingMinigameConfig config;

    private FishingMinigameUI activeMinigame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartMinigame(Action onSuccess, Action onFail)
    {
        if (activeMinigame != null)
        {
            Destroy(activeMinigame.gameObject);
        }

        GameObject minigameObj = Instantiate(minigamePanelPrefab, gameCanvas.transform);
        activeMinigame = minigameObj.GetComponent<FishingMinigameUI>();

        if (activeMinigame != null)
        {
            activeMinigame.Initialize(config, onSuccess, onFail);
        }
    }

    public void StopMinigame()
    {
        if (activeMinigame != null)
        {
            Destroy(activeMinigame.gameObject);
            activeMinigame = null;
        }
    }
}

[Serializable]
public class FishingMinigameUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform catchBar;
    [SerializeField] private RectTransform fishIcon;
    [SerializeField] private Image progressFill;

    private FishingMinigameConfig config;
    private float catchProgress;
    private float targetFishPosition;
    private float currentFishPosition;
    private float barPosition;
    private bool isActive;
    private float barVelocity;
    private float fishNoiseTime;

    private Action onSuccess;
    private Action onFail;

    public void Initialize(FishingMinigameConfig config, Action onSuccess, Action onFail)
    {
        this.config = config;
        this.onSuccess = onSuccess;
        this.onFail = onFail;

        isActive = true;
        catchProgress = 0.5f;
        barPosition = config.minigameHeight / 2f;
        currentFishPosition = config.minigameHeight / 2f;
        targetFishPosition = currentFishPosition;
        fishNoiseTime = 0f;

        UpdateUI();
    }

    private void Update()
    {
        if (!isActive) return;

        UpdateBarPosition();
        UpdateFishPosition();
        UpdateCatchProgress();
        UpdateUI();

        CheckWinLoseConditions();
    }

    private void UpdateBarPosition()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            barVelocity += config.barSpeed * Time.deltaTime;
        }
        else
        {
            barVelocity -= config.barSpeed * Time.deltaTime;
        }

        barVelocity = Mathf.Clamp(barVelocity, -config.barSpeed, config.barSpeed);
        barPosition += barVelocity;
        barPosition = Mathf.Clamp(barPosition, 0f, config.minigameHeight);
    }

    private void UpdateFishPosition()
    {
        fishNoiseTime += Time.deltaTime * config.fishRandomness;
        targetFishPosition = Mathf.PerlinNoise(fishNoiseTime, 0f) * config.minigameHeight;

        currentFishPosition = Mathf.Lerp(
            currentFishPosition,
            targetFishPosition,
            Time.deltaTime * config.fishMovementSpeed
        );
    }

    private void UpdateCatchProgress()
    {
        float distance = Mathf.Abs(barPosition - currentFishPosition);

        if (distance < config.catchZoneSize / 2f)
        {
            catchProgress += Time.deltaTime * 0.25f;
        }
        else
        {
            catchProgress -= Time.deltaTime * 0.5f;
        }

        catchProgress = Mathf.Clamp01(catchProgress);
    }

    private void UpdateUI()
    {
        catchBar.anchoredPosition = new Vector2(
            catchBar.anchoredPosition.x,
            barPosition - config.barSize / 2f
        );

        fishIcon.anchoredPosition = new Vector2(
            fishIcon.anchoredPosition.x,
            currentFishPosition - config.barSize / 2f
        );

        progressFill.fillAmount = catchProgress;
    }

    private void CheckWinLoseConditions()
    {
        if (catchProgress >= 1f)
        {
            EndMinigame(true);
        }
        else if (catchProgress <= 0f)
        {
            EndMinigame(false);
        }
    }

    private void EndMinigame(bool success)
    {
        isActive = false;

        if (success)
        {
            onSuccess?.Invoke();
        }
        else
        {
            onFail?.Invoke();
        }

        FishingMinigame.Instance.StopMinigame();
    }
}