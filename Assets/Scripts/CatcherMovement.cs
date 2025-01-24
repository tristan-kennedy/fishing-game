using UnityEngine;

public class CatcherMovement : MonoBehaviour
{
    public float maxLeft = -250f;
    public float maxRight = 250f;
    public float moveSpeed = 250f;

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");

        if (moveInput != 0)
        {
            MoveCatcher(moveInput);
        }
    }

    private void MoveCatcher(float moveInput)
    {
        Vector3 movement = Vector3.right * moveInput * moveSpeed * Time.deltaTime;

        Vector3 newPosition = transform.localPosition + movement;
        newPosition.x = Mathf.Clamp(newPosition.x, maxLeft, maxRight);

        transform.localPosition = newPosition;
    }
}
