using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5f;
    public float acceleration = 8f;
    public float deceleration = 12f;
    public float rotationSpeed = 10f;

    [Header("Sprint")]
    public float sprintSpeed = 9f;
    public float sprintAcceleration = 12f;
    
    [Header("Physics")]
    public float gravity = -20f;
    public float jumpHeight = 1.5f;
    public float slopeLimit = 45f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController _controller;
    private InputSystem_Actions _input;

    private Vector3 _velocity;
    private Vector3 _currentMove;
    private bool _isGrounded;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _controller.slopeLimit = slopeLimit;
        _input = new InputSystem_Actions();
    }

    void OnEnable() => _input.Enable();
    void OnDisable() => _input.Disable();

    void Update()
    {
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        HandleMovement();
        HandleGravity();
    }

    void HandleMovement()
    {
        Vector2 moveInput = _input.Player.Move.ReadValue<Vector2>();

        // Направление относительно камеры
        Vector3 targetMove = Vector3.zero;
        if (moveInput != Vector2.zero)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;

            targetMove = (camForward.normalized * moveInput.y +
                          camRight.normalized * moveInput.x).normalized;
        }

       
        bool isSprinting = _input.Player.Sprint.IsPressed();
        float currentMaxSpeed = isSprinting ? sprintSpeed : maxSpeed;
        float currentAccel = isSprinting ? sprintAcceleration : acceleration;
        float rate = targetMove != Vector3.zero ? currentAccel : deceleration;
        _currentMove = Vector3.MoveTowards(_currentMove, targetMove * currentMaxSpeed, rate * UnityEngine.Time.deltaTime);

        // Блокировка скольжения на откосах
        if (_isGrounded && OnSteepSlope(out Vector3 slopeNormal))
        {
            _currentMove = Vector3.ProjectOnPlane(_currentMove, slopeNormal);
        }

        _controller.Move(_currentMove * UnityEngine.Time.deltaTime);

        // Поворот персонажа за направлением движения
        if (_currentMove.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_currentMove);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * UnityEngine.Time.deltaTime
            );
        }
    }

    void HandleGravity()
    {
        // Прыжок
        if (_input.Player.Jump.triggered && _isGrounded)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        _velocity.y += gravity * UnityEngine.Time.deltaTime;
        _controller.Move(_velocity * UnityEngine.Time.deltaTime);
    }

    bool OnSteepSlope(out Vector3 slopeNormal)
    {
        slopeNormal = Vector3.up;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
        {
            slopeNormal = hit.normal;
            return Vector3.Angle(Vector3.up, hit.normal) > slopeLimit;
        }
        return false;
    }
}