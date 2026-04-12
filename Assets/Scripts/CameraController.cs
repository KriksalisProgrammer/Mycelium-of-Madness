using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -4f);

    [Header("Rotation")]
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    public float minYAngle = -30f;
    public float maxYAngle = 60f;

    [Header("Smoothing")]
    public float positionSmoothing = 8f;
    public float rotationSmoothing = 10f;

    [Header("Horror")]
    public float horrorSwayAmount = 0.3f;
    public float horrorSwaySpeed = 0.8f;
    public bool enableSway = true;

    private InputSystem_Actions _input;
    private float _yaw;
    private float _pitch;
    private Vector3 _currentVelocity;

    void Awake()
    {
        _input = new InputSystem_Actions();
        // Начальные углы с текущего поворота камеры
        _yaw = transform.eulerAngles.y;
        _pitch = transform.eulerAngles.x;
    }

    void OnEnable()
    {
        _input.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        _input.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleRotation();
        HandlePosition();
        HandleSway();
    }

    void HandleRotation()
    {
        Vector2 lookInput = _input.Player.Look.ReadValue<Vector2>();

        _yaw += lookInput.x * sensitivityX;
        _pitch -= lookInput.y * sensitivityY;
        _pitch = Mathf.Clamp(_pitch, minYAngle, maxYAngle);
    }

    void HandlePosition()
    {
        // Позиция камеры вокруг игрока
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 targetPosition = target.position + rotation * offset;

        // Плавное следование
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _currentVelocity,
            1f / positionSmoothing
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotation,
            rotationSmoothing * UnityEngine.Time.deltaTime
        );
    }

    void HandleSway()
    {
        if (!enableSway) return;

        // Лёгкое покачивание камеры — добавляет тревожность
        float swayX = Mathf.Sin(UnityEngine.Time.time * horrorSwaySpeed) * horrorSwayAmount;
        float swayY = Mathf.Sin(UnityEngine.Time.time * horrorSwaySpeed * 0.5f) * horrorSwayAmount * 0.5f;

        transform.position += new Vector3(swayX, swayY, 0f) * UnityEngine.Time.deltaTime;
    }
}