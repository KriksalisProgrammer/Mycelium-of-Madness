using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float acceleration = 10f;
    public float deceleration = 14f;

    [Header("Physics")]
    public float gravity = -20f;
    public float jumpHeight = 1.2f;
    public float slopeLimit = 45f;
    
    [Header("First Person")]
    public Transform cameraHolder;

    private CharacterController _controller;
    private InputSystem_Actions _input;
    private Vector3 _velocity;
    private Vector3 _currentMove;
    private bool _isGrounded;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _controller.slopeLimit = slopeLimit;
        _input = new InputSystem_Actions();
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();

    private void Update()
    {
        _isGrounded = _controller.isGrounded;
        if (_isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        HandleMovement();
        HandleGravity();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = _input.Player.Move.ReadValue<Vector2>();

        Vector3 targetMove = Vector3.zero;
        if (moveInput != Vector2.zero && cameraHolder != null)
        {
            Vector3 forward = cameraHolder.forward;
            Vector3 right   = cameraHolder.right;
            forward.y = 0f;
            right.y   = 0f;

            targetMove = (forward.normalized * moveInput.y +
                          right.normalized  * moveInput.x).normalized;
        }

        bool isSprinting   = _input.Player.Sprint.IsPressed();
        float targetSpeed  = isSprinting ? sprintSpeed : walkSpeed;
        float rate         = targetMove != Vector3.zero ? acceleration : deceleration;

        _currentMove = Vector3.MoveTowards(
            _currentMove,
            targetMove * targetSpeed,
            rate * Time.deltaTime
        );

        if (_isGrounded && OnSteepSlope(out Vector3 slopeNormal))
            _currentMove = Vector3.ProjectOnPlane(_currentMove, slopeNormal);

        _controller.Move(_currentMove * Time.deltaTime);
    }

    private void HandleGravity()
    {
        if (_input.Player.Jump.triggered && _isGrounded)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private bool OnSteepSlope(out Vector3 slopeNormal)
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