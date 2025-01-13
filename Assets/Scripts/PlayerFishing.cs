using System.Collections;
using UnityEngine;

public class PlayerFishing : MonoBehaviour
{
    public LayerMask waterLayer;
    private bool isFishing = false;
    private const float WAIT_TIME = 3f;

    // Debug variables
    public bool showDebugLines = true;
    public float debugLineLength = 100f;

    void OnDrawGizmos()
    {
        if (showDebugLines && Camera.main != null)
        {
            // Draw the ray in the scene view
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * debugLineLength);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFishing)
        {
            CastLine();
        }
    }

    void CastLine()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Debug info
        Debug.Log($"Ray Origin: {ray.origin}, Direction: {ray.direction}");
        Debug.Log($"Water Layer Mask Value: {waterLayer.value}");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, waterLayer))
        {
            Debug.Log($"Hit something at distance: {hit.distance} on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            FishingArea area = hit.collider.GetComponent<FishingArea>();
            if (area != null)
            {
                StartCoroutine(FishingRoutine(area));
            }
            else
            {
                Debug.Log("No fishing area component found on water!");
            }
        }
        else
        {
            // Additional debug info
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
            Debug.Log("Didn't hit water! Check the following:");
            Debug.Log("1. Is the water object on the correct layer?");
            Debug.Log($"2. Current water layer mask: {waterLayer.value}");
            Debug.Log("3. Does the water have a collider?");
            Debug.Log("4. Is the collider enabled?");
        }
    }

    IEnumerator FishingRoutine(FishingArea area)
    {
        isFishing = true;
        Debug.Log("Started fishing...");

        yield return new WaitForSeconds(WAIT_TIME);

        Fish fish = area.GetRandomFish();
        if (fish != null)
        {
            Debug.Log($"Caught a {fish.fishName}!");
        }
        else
        {
            Debug.Log("No fish in this area!");
        }

        isFishing = false;
    }
}