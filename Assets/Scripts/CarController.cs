using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    [Header("Input Action Assets")]
    [SerializeField] InputActionMap _playerInputActionMap;
    private InputActionAsset _inputAsset;
    private PlayerController _playerController;

    public event Action<PlayerInput> onPlayerJoined;
    public event Action<PlayerInput> onPlayerLeft;

    [Header("Settings")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Header("Wheels")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [Header("Movement fields")]
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] float _movmentForce=5f;
    [SerializeField] float _maxSpeed=5f;
    private Vector3 _forceDirection = Vector3.zero;

    [Header("Camera")]
    [SerializeField] Camera _camera;

    #region InputActions
    private InputAction _rotateInputAction;
    private InputAction _breakInputAction;
    #endregion

    #region InputAction bools
    private bool _isRotateEnabled = true;
    private bool _isGassPressed = false;
    #endregion

    private void Awake()
    {
        _inputAsset = this.GetComponent<PlayerInput>().actions;
        _playerInputActionMap = _inputAsset.FindActionMap("Player");
        _playerController = new PlayerController();
        EnableInputAction();
    }

    private void OnEnable()
    {
        //_playerInputActionMap.FindAction("Rotate").started += RotateCar;
        _playerInputActionMap.FindAction("Gass").performed += PressGass;
        _playerInputActionMap.FindAction("Gass").canceled += CancleGass;
        _playerInputActionMap.FindAction("Break").performed += StartBreak;
        //_playerInputActionMap.FindAction("Gass").canceled += StartBreak;
    }

   

    private void OnDisable()
    {
        //_playerInputActionMap.FindAction("Rotate").started -= RotateCar;
        _playerInputActionMap.FindAction("Gass").performed -= PressGass;
        _playerInputActionMap.FindAction("Gass").canceled -= CancleGass;
        _playerInputActionMap.FindAction("Break").performed -= StartBreak;
        //_playerInputActionMap.FindAction("Gass").canceled -= StartBreak;

    }

    private void FixedUpdate()
    {
        GetInput();
        if(_isGassPressed)
        HandleMotor();
        HandleSteering();
        UpdateWheels();

        //if (_isRotateEnabled)
        //{
        //    _forceDirection += _rotateInputAction.ReadValue<Vector2>().x * GetCameraForward(_camera) * _movmentForce;
        //    _forceDirection += _rotateInputAction.ReadValue<Vector2>().y * GetCameraRight(_camera) * _movmentForce;

        //    _rigidbody.AddForce(_forceDirection,ForceMode.Impulse);
        //    _forceDirection = Vector3.zero;

        //    if(_isGassPressed)
        //    {
        //        if (_rigidbody.velocity.y < 0f)
        //            _rigidbody.velocity += Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        //        Vector3 horizontalVelocity = _rigidbody.velocity;
        //        horizontalVelocity.y = 0f;

        //        if (horizontalVelocity.sqrMagnitude > _maxSpeed * _maxSpeed)
        //            _rigidbody.velocity = horizontalVelocity.normalized * _maxSpeed + Vector3.up * _rigidbody.velocity.y;
        //    }
        //}
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0f;
        return right.normalized;
    }

    private void EnableInputAction()
    {
        _rotateInputAction = _playerController.Player.Rotate;
        _breakInputAction = _playerController.Player.Gass;
        _breakInputAction = _playerController.Player.Break;

        _rotateInputAction.Enable();
        _breakInputAction.Enable();
    }

    private void GetInput()
    {
        // Steering Input
        //horizontalInput = Input.GetAxis("Horizontal");
        horizontalInput = _rotateInputAction.ReadValue<Vector2>().x;

        //// Acceleration Input
        //verticalInput = Input.GetAxis("Vertical");
        verticalInput = 0;

        // Breaking Input
        //isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = motorForce;
        frontRightWheelCollider.motorTorque = motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        Debug.Log("StartBreak");
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void RotateCar(InputAction.CallbackContext obj)
    {
    }

    private void PressGass(InputAction.CallbackContext obj)
    {
        _isGassPressed = true;
    }

    private void CancleGass(InputAction.CallbackContext obj)
    {
        _isGassPressed = false;
    }

    private void StartBreak(InputAction.CallbackContext obj)
    {
        ApplyBreaking();
    }
}