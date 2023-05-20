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
        _playerInputActionMap.FindAction("Gass").performed += PressGass;
        _playerInputActionMap.FindAction("Gass").canceled += CancleGass;
        _playerInputActionMap.FindAction("Break").performed += StartBreak;
        _playerInputActionMap.FindAction("Break").canceled += StopBreak;
    }

   

    private void OnDisable()
    {
        _playerInputActionMap.FindAction("Gass").performed -= PressGass;
        _playerInputActionMap.FindAction("Gass").canceled -= CancleGass;
        _playerInputActionMap.FindAction("Break").performed -= StartBreak;
        _playerInputActionMap.FindAction("Break").canceled -= StopBreak;

    }

    private void FixedUpdate()
    {
        GetInput();
        if(_isGassPressed)
        HandleMotor();
        HandleSteering();
        UpdateWheels();

        currentbreakForce = isBreaking ? breakForce : 0f; //If its breakiing the force will be same as breakForce, else it will be 0f
        ApplyBreaking();
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
        horizontalInput = _rotateInputAction.ReadValue<Vector2>().x;
        verticalInput = 0;
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = motorForce;
        frontRightWheelCollider.motorTorque = motorForce;
    }

    private void ApplyBreaking()
    {
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
        isBreaking = true;
    }

    private void StopBreak(InputAction.CallbackContext obj)
    {
        isBreaking = false;
        Debug.Log(isBreaking);
    }
}