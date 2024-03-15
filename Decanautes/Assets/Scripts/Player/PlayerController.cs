using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class PlayerController : MonoBehaviour
{
    //Components
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    //Variable
    private Vector2 _moveDirection = Vector2.zero;


    [TitleGroup("Parameters", Alignment = TitleAlignments.Centered)]
    [Title("Movement")]
    public float MoveSpeed;
    public float Drag;


    void Start()
    {
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

        _rigidbody.AddForce(_moveDirection * MoveSpeed);
        Vector3 currentVelocity = _rigidbody.velocity;
        _rigidbody.velocity = new Vector3(currentVelocity.x / Drag * Time.fixedDeltaTime, currentVelocity.y, currentVelocity.z / Drag * Time.fixedDeltaTime);
    }


    private void Move(InputAction.CallbackContext callbackContext)
    {
        _moveDirection  = callbackContext.ReadValue<Vector2>();
        


    }

}
