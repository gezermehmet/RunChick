using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Transform _orientationTransform;

    [Header("MovementSettings")] [SerializeField]
    private KeyCode _movementKey;

    [SerializeField] private float _moveSpeed = 15f;

    [Header("JumpSettings")] [SerializeField]
    private KeyCode _jumpKey;

    [SerializeField] private float _jumpforce;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private float _jumpCoolDown;
    [SerializeField] private float _airMultiplier;

    [Header("SlideSettings")] [SerializeField]
    private KeyCode _slideKey;

    [SerializeField] private float _slideMultiplier;
    [SerializeField] private float _slideDrag;

    [Header("GroundCheckSettings")] [SerializeField]
    private LayerMask _groundLayer;

    [SerializeField] private float _playerHeight;
    [SerializeField] private float _groundDrag;

    private StateController _stateController;
    private Rigidbody _playerRigidBody;
    private float _horizontalInput, _verticalInput;
    private Vector3 _movementDirection;
    private bool _isSliding;


    private void Awake()
    {
        _playerRigidBody = GetComponent<Rigidbody>();
        _playerRigidBody.freezeRotation = true;
        _stateController = GetComponent<StateController>();
    }

    private void Update()
    {
        SetInputs();
        SetStatus();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }

    private void FixedUpdate()
    {
        SetPlayerMovement();
    }

    private void SetStatus()
    {
        var movementDirection = GetMovementDirection();
        var isGrounded = IsGrounded();
        var isSliding = IsSliding();
        var currentState = _stateController.GetCurrentPlayerState();


        var newState = currentState switch
        {
            _ when movementDirection == Vector3.zero && isGrounded && !_isSliding => PlayerState.Idle,
            _ when movementDirection != Vector3.zero && isGrounded && !_isSliding => PlayerState.Move,
            _ when movementDirection != Vector3.zero && isGrounded && isSliding => PlayerState.Slide,
            _ when movementDirection == Vector3.zero && isGrounded && isSliding => PlayerState.SlideIdle,
            _ when !_canJump && !isGrounded => PlayerState.Jump,
            _ => currentState   
        };

        if (newState != currentState)
        {
            _stateController.ChangeState(newState);
        }
        
        Debug.Log(newState);
    }

    private void SetInputs()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(_slideKey))
        {
            _isSliding = true;
        }
        else if (Input.GetKeyDown(_movementKey))
        {
            _isSliding = false;
        }
        else if (Input.GetKey(_jumpKey) && _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJump();
            Invoke(nameof(ResetJumping), _jumpCoolDown);
        }
    }

    private void SetPlayerMovement()
    {
        _movementDirection = _orientationTransform.forward * _verticalInput
                             + _orientationTransform.right * _horizontalInput;

        float forceMultiplier = _stateController.GetCurrentPlayerState() switch
        {
            PlayerState.Move => 1f,
            PlayerState.Slide => _slideMultiplier,
            PlayerState.Jump => _airMultiplier,
            _ => 1f
        };

        if (_isSliding)
        {
            _playerRigidBody.AddForce(_movementDirection.normalized * _moveSpeed * _slideMultiplier, ForceMode.Force);
        }
        else
        {
            _playerRigidBody.AddForce(_movementDirection.normalized * _moveSpeed, ForceMode.Force);
        }
    }

    private void SetPlayerDrag()
    {
        if (_isSliding)
        {
            _playerRigidBody.linearDamping = _slideDrag;
        }
        else
        {
            _playerRigidBody.linearDamping = _groundDrag;
        }
    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity =
            new Vector3(_playerRigidBody.linearVelocity.x, 0f, _playerRigidBody.linearVelocity.z);

        if (flatVelocity.magnitude > _moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _moveSpeed;
            _playerRigidBody.linearVelocity =
                new Vector3(limitedVelocity.x, _playerRigidBody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void SetPlayerJump()
    {
        _playerRigidBody.linearVelocity =
            new Vector3(_playerRigidBody.linearVelocity.x, 0f, _playerRigidBody.linearVelocity.z);
        _playerRigidBody.AddForce(transform.up * _jumpforce, ForceMode.Impulse);
    }

    private void ResetJumping()
    {
        _canJump = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);
    }

    private Vector3 GetMovementDirection()
    {
        return _movementDirection.normalized;
    }

    private bool IsSliding()
    {
        return _isSliding;
    }
}