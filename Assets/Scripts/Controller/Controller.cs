using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _accelerationTime;
    [SerializeField] private float _decelerationOverTime;
    private float _currentSpeed;
    
    [Header("Input Action Assets")]
    [SerializeField] InputActionMap _playerInputActionMap;
    private InputActionAsset _inputAsset;
    private PlayerController _playerController;
    
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
    
    private void Awake()
    {
        _inputAsset = this.GetComponent<PlayerInput>().actions;
        _playerInputActionMap = _inputAsset.FindActionMap("Player");
        _playerController = new PlayerController();
        EnableInputAction();
    }

    void Update()
    {
        var position = transform.position;

        if (_isGassPressed)
            Acceleration();
        if (_isBreakingPressed)
            Deceleration();
        if (!_isGassPressed && !_isBreakingPressed)
            DecelerationOverTime();

        Vector3 moveDir = Quaternion.AngleAxis(20, transform.up) * transform.forward;

        position = Vector3.Lerp(position, position + moveDir, _currentSpeed * Time.deltaTime);
        transform.position = position;

        Debug.Log(_currentSpeed);
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
    
    private void EnableInputAction()
    {
        _rotateInputAction = _playerController.Player.Rotate;
        _breakInputAction = _playerController.Player.Gass;
        _breakInputAction = _playerController.Player.Break;

        _rotateInputAction.Enable();
        _breakInputAction.Enable();
    }
    
    private void Acceleration() =>
        _currentSpeed = Mathf.Lerp(_currentSpeed, _maxSpeed,_accelerationTime * Time.deltaTime);

    private void Deceleration()=>
        _currentSpeed = Mathf.Lerp(_currentSpeed, 0,_accelerationTime * Time.deltaTime);
    
    private void DecelerationOverTime()=>
        _currentSpeed = Mathf.Lerp(_currentSpeed, 0,_decelerationOverTime * Time.deltaTime);


    #region InputMethod
    
    private void PressGass(InputAction.CallbackContext obj)=>
        _isGassPressed = true;
    
    private void CancleGass(InputAction.CallbackContext obj)=>
        _isGassPressed = false;
    
    private void StartBreak(InputAction.CallbackContext obj)=>
        _isBreakingPressed = true;

    private void StopBreak(InputAction.CallbackContext obj)=>
        _isBreakingPressed = false;

    private void StartRevers(InputAction.CallbackContext obj)=>
        _isReversedPressed = true;

    private void StopRevers(InputAction.CallbackContext obj)=>
        _isReversedPressed = false;

    #endregion
}