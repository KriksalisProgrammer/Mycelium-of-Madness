using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Sensitivity")]
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    public float minPitch = -80f;
    public float maxPitch =  80f;
    
    [Header("Horror sway")]
    public bool  enableSway     = true;
    public float swayAmount     = 0.03f;
    public float swaySpeed      = 0.8f;

    [Header("References")]
    public Transform playerBody;
    
    private InputSystem_Actions _input;
    private float _yaw;
    private float _pitch;
    
    private void Awake()
    {
        _input = new InputSystem_Actions();
        _yaw   = playerBody != null ? playerBody.eulerAngles.y : 0f;
        _pitch = transform.localEulerAngles.x;
    }

    private void OnEnable()
    {
        _input.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void OnDisable()
    {
        _input.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    private void Update()
    {
        Vector2 look = _input.Player.Look.ReadValue<Vector2>();

        _yaw   += look.x * sensitivityX;
        _pitch -= look.y * sensitivityY;
        _pitch  = Mathf.Clamp(_pitch, minPitch, maxPitch);
        
        if (playerBody != null)
            playerBody.rotation = Quaternion.Euler(0f, _yaw, 0f);
        
        transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

        if (enableSway)
            ApplySway();
    }

    private void ApplySway()
    {
        float swayX = Mathf.Sin(Time.time * swaySpeed)        * swayAmount;
        float swayY = Mathf.Sin(Time.time * swaySpeed * 0.5f) * swayAmount * 0.5f;
        transform.localPosition = new Vector3(swayX, swayY, 0f);
    }
}