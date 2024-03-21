using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Components
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    //Variable
    private Vector2 _moveDirection = Vector2.zero;


    public PlayerData playerData;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = new PlayerInput();
        _playerInput.Enable();


        //Input Events
        _playerInput.InGame.Move.performed += Move;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z) * _moveDirection.y + Camera.main.transform.right * _moveDirection.x;
        _rigidbody.AddForce(dir * playerData.MoveSpeed);
        Vector3 currentVelocity = _rigidbody.velocity;
        _rigidbody.velocity = new Vector3(currentVelocity.x / (playerData.Drag * Time.fixedDeltaTime*100), currentVelocity.y, currentVelocity.z / (playerData.Drag * Time.fixedDeltaTime * 100));
    }


    private void Move(InputAction.CallbackContext callbackContext)
    {
        _moveDirection  = callbackContext.ReadValue<Vector2>();
    }

}
