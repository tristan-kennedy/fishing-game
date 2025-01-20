using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFishing : MonoBehaviour
{
    public LayerMask waterLayer; // Set this to the water layer in the Inspector
    public GameObject cursorPrefab; // Assign a prefab for the cursor (e.g., a circle or target marker)
    private GameObject cursorInstance;
    private bool isFishing = false;
    private const float WAIT_TIME = 3f;

    private Vector2 cursorPosition; // Input from Unity's new Input System

    public void OnLook(InputAction.CallbackContext context)
    {
        cursorPosition = context.ReadValue<Vector2>();
    }

    void Start()
    {
        // Instantiate the cursor at the start but disable it initially
        if (cursorPrefab != null)
        {
            cursorInstance = Instantiate(cursorPrefab);
            cursorInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (Camera.main == null || isFishing) return;

        // Handle casting the fishing line on left mouse button press
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CastLine();
        }

        // Update the cursor position
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
                StartCoroutine(FishingRoutine(area));
            }
        }
        else
        {
            Debug.Log("The cast did not hit any water.");
        }
    }

    IEnumerator FishingRoutine(FishingArea area)
    {
        isFishing = true;

        Debug.Log("Fishing...");
        yield return new WaitForSeconds(WAIT_TIME);

        Fish fish = area.GetRandomFish();
        if (fish != null)
        {
            Debug.Log($"Caught a {fish.fishName}!");
        }
        else
        {
            Debug.Log("No fish were caught.");
        }

        isFishing = false;
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
