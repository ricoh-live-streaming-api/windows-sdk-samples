using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// カメラの移動・回転制御
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10.0f)]
    private float positionStep = 2.0f;
    [SerializeField, Range(0.1f, 10.0f)]
    private float rotationSpeed = 0.1f;

    private Transform cameraTransform;
    private Vector3 presentCamRotation;

    public void Start()
    {
        cameraTransform = gameObject.transform;
    }

    public void Update()
    {
        CameraPositionKeyControl();
    }

    /// <summary>
    /// カメラ位置リセット
    /// </summary>
    /// <param name="context"></param>
    public void OnCameraReset(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Performed)
        {
            cameraTransform.SetPositionAndRotation(new Vector3(0, 0, 0), new Quaternion());
        }
    }

    /// <summary>
    /// マウスによるカメラ回転
    /// </summary>
    /// <param name="context"></param>
    public void OnCameraTurn(InputAction.CallbackContext context)
    {
        if (context.action.phase == InputActionPhase.Performed)
        {
            if (Mouse.current.rightButton.isPressed)
            {
                var inputValue = context.ReadValue<Vector2>();
                presentCamRotation.y += inputValue.x * rotationSpeed;
                presentCamRotation.x -= inputValue.y * rotationSpeed;

                // 上下方向は 90 度まで
                presentCamRotation.x = Mathf.Clamp(presentCamRotation.x, -90, 90);

                cameraTransform.localEulerAngles = presentCamRotation;
            }
        }
    }

    /// <summary>
    /// キーボードによるカメラ移動
    /// </summary>
    private void CameraPositionKeyControl()
    {
        Vector3 campos = cameraTransform.position;

        if (Keyboard.current.dKey.isPressed)
        {
            // 右
            campos += positionStep * Time.deltaTime * cameraTransform.right;
        }
        else if (Keyboard.current.aKey.isPressed)
        {
            // 左
            campos -= positionStep * Time.deltaTime * cameraTransform.right;
        }
        else if (Keyboard.current.eKey.isPressed)
        {
            // 上
            campos += positionStep * Time.deltaTime * cameraTransform.up;
        }
        else if (Keyboard.current.qKey.isPressed)
        {
            // 下
            campos -= positionStep * Time.deltaTime * cameraTransform.up;
        }
        else if (Keyboard.current.wKey.isPressed)
        {
            // 前
            campos += positionStep * Time.deltaTime * cameraTransform.forward;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            // 後
            campos -= positionStep * Time.deltaTime * cameraTransform.forward;
        }
        else
        {
            // nothing to do.
        }

        cameraTransform.position = campos;
    }
}