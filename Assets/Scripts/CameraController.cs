using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    #region Variables

    private Vector2 _delta;

    private bool _isRotating;

    private float _xRotation;

    [SerializeField] private float rotationSpeed = 0.5f;

    private Tween _snapTween;

    #endregion

    private void Awake()
    {
        _xRotation = transform.rotation.eulerAngles.x;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _delta = context.ReadValue<Vector2>();
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            _isRotating = true;

            // If snapping is in progress, stop it immediately
            _snapTween?.Kill();
        }

        if (context.canceled)
        {
            _isRotating = false;

            SnapRotation();
        }
    }

    private void LateUpdate()
    {
        if (_isRotating)
        {
            transform.Rotate(new Vector3(_xRotation, -_delta.x * rotationSpeed, 0.0f));
            transform.rotation = Quaternion.Euler(_xRotation, transform.rotation.eulerAngles.y, 0.0f);
        }
    }

    private void SnapRotation()
    {
        _snapTween = transform.DORotate(SnappedVector(), 0.5f)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _snapTween = null; // Clear the reference when snapping completes
            });
    }

    private Vector3 SnappedVector()
    {
        var endValue = 0.0f;
        var currentY = Mathf.Round(transform.rotation.eulerAngles.y);

        endValue = currentY switch
        {
            >= 0 and < 90 => 45.0f,
            >= 90 and < 180 => 135.0f,
            >= 180 and < 270 => 225.0f,
            _ => 315.0f
        };

        return new Vector3(_xRotation, endValue, 0.0f);
    }
}
