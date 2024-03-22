using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Components
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;

    //Variable
    private Vector2 _moveDirection = Vector2.zero;


    public PlayerData PlayerData;
    public LayerMask InteractionLayer;

    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private Interactable lookingAt;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = new PlayerInput();
        _playerInput.Enable();


        //Input Events
        _playerInput.InGame.Move.performed += Move;
        _playerInput.InGame.Interact.performed += Interact;
    }

    private void OnDestroy()
    {
        _playerInput.InGame.Move.performed -= Move;
        _playerInput.InGame.Interact.performed -= Interact;
    }

    void Update()
    {
        LookInteraction();
    }

    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z) * _moveDirection.y + Camera.main.transform.right * _moveDirection.x;
        _rigidbody.AddForce(dir * PlayerData.MoveSpeed);
        Vector3 currentVelocity = _rigidbody.velocity;
        _rigidbody.velocity = new Vector3(currentVelocity.x / (PlayerData.Drag * Time.fixedDeltaTime*100), currentVelocity.y, currentVelocity.z / (PlayerData.Drag * Time.fixedDeltaTime * 100));
    }


    private void Move(InputAction.CallbackContext callbackContext)
    {
        _moveDirection  = callbackContext.ReadValue<Vector2>();
    }

    private void Interact(InputAction.CallbackContext callbackContext)
    {
        if (!lookingAt)
        {
            return;
        }
        if (callbackContext.ReadValueAsButton())
        {
            lookingAt.InteractionStart();
            return;
        }
        lookingAt.InteractionEnd();
    }


    private void LookInteraction()
    {
        Vector3 dir = Camera.main.transform.forward;
        Ray ray = new Ray(Camera.main.transform.position,dir);
        Debug.DrawRay(Camera.main.transform.position, dir * PlayerData.InteractionMaxDist);
        if (Physics.Raycast(ray, out RaycastHit hit, PlayerData.InteractionMaxDist, InteractionLayer))
        {
            if (!hit.transform.TryGetComponent<Interactable>(out Interactable component))
            {
                if (lookingAt)
                {
                    lookingAt.StopHover();
                    if (lookingAt.isPressed)
                    {
                        lookingAt.InteractionEnd();
                    }
                }
                lookingAt = null;
                return;
            }
            if (lookingAt!= component)
            {
                lookingAt = component;
                lookingAt.Hover();
            }
        }
        else
        {
            if (lookingAt)
            {
                lookingAt.StopHover();
                if (lookingAt.isPressed)
                {
                    lookingAt.InteractionEnd();
                }
            }
            lookingAt = null;
        }
    }

}
