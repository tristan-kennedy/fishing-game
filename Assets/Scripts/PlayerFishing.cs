using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerFishing : MonoBehaviour
{
    private const float BITE_WINDOW = 1.5f;
    private const float MIN_FISH_TIME = 2f;
    private const float MAX_FISH_TIME = 5f;

    [Header("References")]
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private GameObject cursorPrefab;
    [SerializeField] private GameObject exclamationPrefab;
    [SerializeField] private PlayerController playerController;

    private GameObject cursorInstance;
    private GameObject exclamationInstance;
    private Vector2 cursorPosition;
    private FishingState currentState = FishingState.Idle;
    private FishingArea currentFishingArea;
    private float stateTimer;
    private float nextFishTime;

    public FishingMinigame minigame;
    public RenderTexture renderTexture;

    private enum FishingState
    {
        Idle,
        Fishing,
        FishBiting,
        FishingMinigame,
        Reeling
    }

    private void Start()
    {
        minigame.onFishCaught.AddListener(() => EndMinigame(true));
        minigame.onFishLost.AddListener(() => EndMinigame(false));
        InitializeObjects();
    }

    private void InitializeObjects()
    {
        if (cursorPrefab != null)
        {
            cursorInstance = Instantiate(cursorPrefab);
            cursorInstance.SetActive(false);
        }

        if (exclamationPrefab != null)
        {
            exclamationInstance = Instantiate(exclamationPrefab);
            exclamationInstance.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateCursorVisibility();
        UpdateFishingState();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 rawPosition = context.ReadValue<Vector2>();

        // Scale the position based on render texture vs screen resolution
        float scaleX = (float)renderTexture.width / Screen.width;
        float scaleY = (float)renderTexture.height / Screen.height;

        cursorPosition = new Vector2(
            rawPosition.x * scaleX,
            rawPosition.y * scaleY
        );
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        switch (currentState)
        {
            case FishingState.Idle:
                TryCastLine();
                break;
            case FishingState.Fishing:
                StopFishing();
                break;
            case FishingState.FishBiting:
                StartMinigame();
                break;
        }
    }

    private void UpdateFishingState()
    {
        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case FishingState.Fishing when stateTimer >= nextFishTime:
                TriggerFishBite();
                break;
            case FishingState.FishBiting when stateTimer >= BITE_WINDOW:
                FishEscaped();
                break;
        }
    }

    private void TryCastLine()
    {
        Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, waterLayer))
        {
            Debug.Log("Cast missed the water!");
            return;
        }

        FishingArea area = hit.collider.GetComponent<FishingArea>();
        if (area == null) return;

        StartFishing(area, hit.point);
    }

    private void StartFishing(FishingArea area, Vector3 position)
    {
        currentFishingArea = area;
        currentState = FishingState.Fishing;
        playerController.canMove = false;
        stateTimer = 0f;
        nextFishTime = UnityEngine.Random.Range(MIN_FISH_TIME, MAX_FISH_TIME);

        if (cursorInstance != null)
        {
            cursorInstance.transform.position = position;
            cursorInstance.SetActive(true);
        }
    }

    private void TriggerFishBite()
    {
        currentState = FishingState.FishBiting;
        stateTimer = 0f;

        if (exclamationInstance != null)
        {
            exclamationInstance.transform.position = cursorInstance.transform.position;
            exclamationInstance.SetActive(true);
        }
    }

    private void CatchFish()
    {
        if (currentFishingArea != null)
        {
            Fish currentFish = currentFishingArea.GetRandomFish();
            Debug.Log($"Caught a {currentFish.fishName}!");
        }

        StopFishing();
    }

    private void FishEscaped()
    {
        Debug.Log("The fish got away!");
        StopFishing();
    }

    private void StopFishing()
    {
        currentState = FishingState.Idle;
        currentFishingArea = null;
        playerController.canMove = true;

        if (exclamationInstance != null)
        {
            exclamationInstance.SetActive(false);
        }
    }

    private void UpdateCursorVisibility()
    {
        if (cursorInstance == null || Camera.main == null || currentState != FishingState.Idle)
            return;

        Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, waterLayer))
        {
            cursorInstance.transform.position = hit.point;
            cursorInstance.SetActive(true);
        }
        else
        {
            cursorInstance.SetActive(false);
        }
    }

    private void StartMinigame()
    {
        currentState = FishingState.FishingMinigame;
        minigame.gameObject.SetActive(true);
    }

    public void EndMinigame(bool success)
    {
        minigame.gameObject.SetActive(false);

        if (success)
        {
            CatchFish();
        }
        else
        {
            FishEscaped();
        }
    }
}