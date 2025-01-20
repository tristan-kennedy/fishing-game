using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFishing : MonoBehaviour
{
    private float CLICK_WINDOW = 1.5f; // Time the player has to click
    private const float WAIT_TIME = 3f;
    public LayerMask waterLayer; // Set this to the water layer in the Inspector
    public GameObject cursorPrefab; // Assign a prefab for the cursor (e.g., a circle or target marker)
    private GameObject cursorInstance;
    public GameObject exclamationPrefab; // Assign a prefab for the exclamation mark
    private GameObject exclamationInstance;
    private bool fishOnLine = false;
    private FishingArea currentFishingArea;
    private Coroutine fishingRoutine;

    private Vector2 cursorPosition; // Input from Unity's new Input System

    public void OnLook(InputAction.CallbackContext context)
    {
        cursorPosition = context.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentFishingArea != null)
            {
                HandleStopFishing();
                return;
            }
            else
            {
                CastLine();
            }
        }
    }

    void Start()
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


    void Update()
    {
        if (Camera.main == null || currentFishingArea is not null) return;

        UpdateCursor();
    }

    void CastLine()
    {
        Ray ray = Camera.main.ScreenPointToRay(cursorPosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if((waterLayer & 1 << hit.collider.gameObject.layer) == 0)
            {
                Debug.Log("The cast did not hit any water.");
                return;
            }
            FishingArea area = hit.collider.GetComponent<FishingArea>();
            if (area != null)
            {
                currentFishingArea = area;
                fishingRoutine = StartCoroutine(FishingRoutine());
            }
        }
        else
        {
            Debug.Log("The cast did not hit any water.");
        }
    }

    IEnumerator FishingRoutine()
    {
        while (currentFishingArea is not null)
        {
            Debug.Log("Fishing...");
            yield return new WaitForSeconds(WAIT_TIME);

            // A fish is on the line
            fishOnLine = true;
            if (exclamationInstance != null)
            {
                exclamationInstance.SetActive(true);
                exclamationInstance.transform.position = cursorInstance.transform.position;
            }

            float elapsedTime = 0f;

            while (elapsedTime < CLICK_WINDOW)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (fishOnLine)
            {
                Debug.Log("The fish got away!");
            }

            if (exclamationInstance != null)
            {
                exclamationInstance.SetActive(false);
            }
            fishOnLine = false;
        }
    }

    void HandleStopFishing()
    {

        if (fishOnLine)
        {
            Fish fish = currentFishingArea.GetRandomFish();
            Debug.Log($"Caught a {fish.fishName}!");
        }
        else
        {
            Debug.Log("No fish were caught.");
        }

        currentFishingArea = null;
        CleanupFishing();
    }

    void CleanupFishing()
    {
        StopCoroutine(fishingRoutine);
        currentFishingArea = null;
        if (exclamationInstance != null)
        {
            exclamationInstance.SetActive(false);
        }
        fishOnLine = false;
    }


    void UpdateCursor()
    {
        if (cursorInstance == null || Camera.main == null) return;

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
}
