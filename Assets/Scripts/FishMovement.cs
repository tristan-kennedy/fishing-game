using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float maxLeft = -250f;
    public float maxRight = 250f;

    public float moveSpeed = 250f;
    public float changeFrequency = 0.01f;

    public float targetPosition;
    public bool movingRight = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPosition = Random.Range(maxLeft, maxRight);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(targetPosition, transform.localPosition.y, transform.localPosition.z), moveSpeed * Time.deltaTime);

        if(Mathf.Approximately(transform.localPosition.x, targetPosition))
        {
            targetPosition = Random.Range(maxLeft, maxRight);
        }

        if(Random.value < changeFrequency)
        {
            movingRight = !movingRight;
            targetPosition = movingRight ? maxRight : maxLeft;
        }
    }
}
