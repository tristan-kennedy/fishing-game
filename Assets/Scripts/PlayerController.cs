using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Vector2 move;
    public bool canMove = true;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            movePlayer();
        }
    }

    public void movePlayer()
    {
        if (move != Vector2.zero)
        {
            // Get the camera's forward and right directions
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;

            // Flatten the directions to the XZ plane
            cameraForward.y = 0;
            cameraRight.y = 0;

            // Normalize them to avoid scaling issues
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate the movement direction relative to the camera
            Vector3 movement = (cameraForward * move.y + cameraRight * move.x).normalized;

            // Rotate the player to face the movement direction
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);

            // Move the player
            transform.Translate(movement * speed * Time.deltaTime, Space.World);
        }
    }

}
