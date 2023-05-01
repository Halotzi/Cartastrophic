using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestingInputSystem : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private GameObject[] wheels = new GameObject[4];
    private PlayerInput _playerInput;
    private Player1Test _playerInputAction;

    private Vector2 _rotation;
    private Vector2 _move;
    private float _angle;

    private float _currentSpeed;

    private bool _gassPressed;
    private bool _isRotating;


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInputAction = new Player1Test();
        _playerInputAction.Movement.Enable();
        //_playerInputAction.Movement.Move.performed += ctx => _move = ctx.ReadValue<Vector2>();
        _playerInputAction.Movement.Move.performed += ctx => _rotation = ctx.ReadValue<Vector2>();
        _playerInputAction.Movement.Gass.performed += MoveCar;
        _playerInputAction.Movement.Gass.canceled += StopCar;
    }

    private void FixedUpdate()
    {

        if(_gassPressed) 
        {
            if(_currentSpeed< _maxSpeed)
                _currentSpeed+= Time.deltaTime;

            Vector3 movment = new Vector3(_move.x, 0f, _move.y);
            transform.Translate(movment * _currentSpeed * Time.deltaTime, Space.World);
        }

        else if (!_gassPressed && _currentSpeed>0)
            _currentSpeed -= Time.deltaTime;

        RotateCar();
        //Move();
        Vector2 movementInputVector = _playerInputAction.Movement.Move.ReadValue<Vector2>();
        Vector2 rotateInputVector = _playerInputAction.Movement.Rotate.ReadValue<Vector2>();
        _playerRigidbody.AddForce(new Vector3(movementInputVector.x, 0, movementInputVector.y) * _currentSpeed, ForceMode.Force);
    }


    public void MoveCar(InputAction.CallbackContext action)
    {
        _gassPressed = true;
    }

    public void StopCar(InputAction.CallbackContext action) 
    { 
        _gassPressed = false; 
    }

    public void RotateCar()
    {
        if (_rotation.x == 0 && _rotation.y == 0)
            return;
        Vector3 movment = new Vector3(_move.x, 0f, _move.y);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movment), 0.15f);
    }
}
