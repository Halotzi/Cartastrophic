using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce, currentMotorForce;

    [Header("Input Action Assets")]
    [SerializeField] InputActionMap _playerInputActionMap;
    private InputActionAsset _inputAsset;
    private PlayerController _playerController;

    public event Action<PlayerInput> onPlayerJoined;
    public event Action<PlayerInput> onPlayerLeft;

    [Header("Settings")]
    [SerializeField] private float _maxForwardMotorForce;
    [SerializeField] private float _maxReverseMotorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float knockBackForce;

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

    //[Header("Camera")]
   // [SerializeField] Camera _camera;

    #region InputActions
    private InputAction _rotateInputAction;
    private InputAction _breakInputAction;
    #endregion

    #region InputAction bools
    private bool _isBreakingPressed = false;
    private bool _isGassPressed = false;
    private bool _isBoostPressed = false;
    private bool _isReversedPressed = false;
    #endregion

    private void AddKnockBack(Vector3 dir, float force)
    {
        _rigidbody.AddForce(Vector3.up * force / 4 ,ForceMode.Impulse);
        _rigidbody.AddForce(dir * force ,ForceMode.Impulse);
    }

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
        _playerInputActionMap.FindAction("Revers").performed += StartRevers;
        _playerInputActionMap.FindAction("Revers").canceled += StopRevers;
    }
    
    private void OnDisable()
    {
        _playerInputActionMap.FindAction("Gass").performed -= PressGass;
        _playerInputActionMap.FindAction("Gass").canceled -= CancleGass;
        _playerInputActionMap.FindAction("Break").performed -= StartBreak;
        _playerInputActionMap.FindAction("Break").canceled -= StopBreak;
        _playerInputActionMap.FindAction("Revers").performed -= StartRevers;
        _playerInputActionMap.FindAction("Revers").canceled -= StopRevers;
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
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
        UpdateCurrentMotorForce();
        frontLeftWheelCollider.motorTorque = currentMotorForce;
        frontRightWheelCollider.motorTorque = currentMotorForce;
    }

    private void UpdateCurrentMotorForce()
    {
        if (_isGassPressed)
        {
            _isBreakingPressed =false;
            _isReversedPressed=false;

            if (currentMotorForce >= _maxForwardMotorForce)
                currentMotorForce = _maxForwardMotorForce;

            else if (currentMotorForce < 0)
            {
                currentMotorForce += Time.deltaTime * 50;
                ApplyBreaking(breakForce*2);
            }

            else
                currentMotorForce += Time.deltaTime * 50;
        }

        else if (_isReversedPressed) 
        {
            _isGassPressed = false;
            _isBreakingPressed =false;
            if (currentMotorForce <= _maxReverseMotorForce)
                currentMotorForce = _maxReverseMotorForce;

            else if (currentMotorForce > 0)
            {
                currentMotorForce -= Time.deltaTime * 50;
                ApplyBreaking(breakForce*2);
            }

            else
                currentMotorForce -= Time.deltaTime * 50;
        }

        else
        {
            _isBreakingPressed = true;
            currentMotorForce = 0;
        }

        Debug.Log(currentMotorForce);
        //currentMotorForce = _isReversedPressed ? -currentMotorForce : currentMotorForce;
    }

    private void ApplyBreaking()
    {
        currentbreakForce = _isBreakingPressed ? breakForce : 0f; //If its breakiing the force will be same as breakForce, else it will be 0f
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void ApplyBreaking(float breakForce)
    {
        currentbreakForce = breakForce; 
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

    private void PressGass(InputAction.CallbackContext obj)
    {
        _isGassPressed = true;
        Debug.Log("Gass pressed");
    }

    private void CancleGass(InputAction.CallbackContext obj)
    {
        _isGassPressed = false;
    }

    private void StartBreak(InputAction.CallbackContext obj)
    {
        _isBreakingPressed = true;
    }

    private void StopBreak(InputAction.CallbackContext obj)
    {
        _isBreakingPressed = false;
       // Debug.Log(_isBreakingPressed);
    }

    private void StartRevers(InputAction.CallbackContext obj)
    {
        _isReversedPressed = true;
    }

    private void StopRevers(InputAction.CallbackContext obj)
    {
        _isReversedPressed = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<CarController>(out var car))
        {
            if (_rigidbody.velocity.magnitude > car._rigidbody.velocity.magnitude)
            {
                var dir = (car.transform.position - transform.position).normalized;
                //dir = new Vector3(dir.x, 0, dir.z);
                
                car.AddKnockBack(dir, knockBackForce);
            }
            else
            {
                var dir = (transform.position - car.transform.position).normalized;
                //dir = new Vector3(dir.x, 0, dir.z);

                AddKnockBack(dir, knockBackForce);
            }
        }
    }
}