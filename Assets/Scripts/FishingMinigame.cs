using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FishingMinigame : MonoBehaviour
{
    public RectTransform fishTransform;
    public RectTransform catcherTransform;

    public bool isFishOverlapping;
    public UnityEvent onFishCaught;
    public UnityEvent onFishLost;

    public Slider successSlider;
    float successIncrement = 15;
    float failDecrement = 12;
    float successThreshold = 100;
    float failThreshold = -100;
    float successCounter = 0;

    // Update is called once per frame
    void Update()
    {
        if (CheckOverlapping(fishTransform, catcherTransform))
        {
            isFishOverlapping = true;
        }
        else
        {
            isFishOverlapping = false;
        }

        OverlappingCalculation();
    }

    private void OverlappingCalculation()
    {
        if (isFishOverlapping)
        {
            successCounter += successIncrement * Time.deltaTime;
        }
        else
        {
            successCounter -= failDecrement * Time.deltaTime;
        }

        successCounter = Mathf.Clamp(successCounter, failThreshold, successThreshold);
        successSlider.value = successCounter;

        if(successCounter >= successThreshold)
        {
            onFishCaught.Invoke();

            successCounter = 0;
            successSlider.value = 0;
        }
        else if (successCounter <= failThreshold)
        {
            onFishLost.Invoke();

            successCounter = 0;
            successSlider.value = 0;
        }
    }

    private bool CheckOverlapping(RectTransform fishTransform, RectTransform catcherTransform)
    {
        Rect r1 = new Rect(fishTransform.position.x, fishTransform.position.y, fishTransform.rect.width, fishTransform.rect.height);
        Rect r2 = new Rect(catcherTransform.position.x, catcherTransform.position.y, catcherTransform.rect.width, catcherTransform.rect.height);
        return r1.Overlaps(r2);
    }
}
