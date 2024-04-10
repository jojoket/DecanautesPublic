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
    public Transform GrabPoint;

    //Variable
    private Vector2 _moveDirection = Vector2.zero;


    public PlayerData PlayerData;
    public LayerMask InteractionLayer;

    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private Interactable lookingAt;
    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private List<Grabbable> grabbed;



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = new PlayerInput();
        _playerInput.Enable();


        //Input Events
        _playerInput.InGame.Move.performed += Move;
        _playerInput.InGame.Interact.performed += Interact;
        _playerInput.InGame.InteractSec.performed += InteractSec;
    }

    private void OnDestroy()
    {
        _playerInput.InGame.Move.performed -= Move;
        _playerInput.InGame.Interact.performed -= Interact;
        _playerInput.InGame.InteractSec.performed -= InteractSec;
    }

    void Update()
    {
        LookInteraction();
    }

    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized * _moveDirection.y + Camera.main.transform.right * _moveDirection.x;
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
            if (lookingAt.GetType() == typeof(Grabbable))
            {
                grabbed.Add(lookingAt.GetComponent<Grabbable>());
            }
            return;
        }
        if (lookingAt.isPressed)
        {
            lookingAt.InteractionEnd();
            lookingAt = null;
        }
    }

    private void InteractSec(InputAction.CallbackContext callbackContext)
    {
        if (lookingAt && lookingAt.TryGetComponent<PostIt>(out PostIt postIt))
        {
            postIt.SelectText();
        }
    }


    private void LookInteraction()
    {
        Vector3 dir = Camera.main.transform.forward;
        Ray ray = new Ray(Camera.main.transform.position,dir);
        Debug.DrawRay(Camera.main.transform.position, dir * PlayerData.InteractionMaxDist);
        if (Physics.Raycast(ray, out RaycastHit hit, PlayerData.InteractionMaxDist))
        {
            //if looking at something without the Interactable component
            if (!hit.transform.TryGetComponent<Interactable>(out Interactable component))
            {
                if (lookingAt)
                {
                    StopLookAtInteractable();
                }
                lookingAt = null;
                return;
            }
            //if looking at another object
            if (lookingAt!= component) 
            {
                if (lookingAt)
                {
                    StopLookAtInteractable();
                }
                lookingAt = component;
                lookingAt.Hover();
            }
        }
        else
        {
            //if looking at nothing
            if (lookingAt)
            {
                StopLookAtInteractable();
            }
            lookingAt = null;
        }
    }

    private void StopLookAtInteractable()
    {
        lookingAt.StopHover();
        if (lookingAt.isPressed && lookingAt.NeedLookToKeepInteraction)
        {
            lookingAt.InteractionEnd();
        }
    }

}
