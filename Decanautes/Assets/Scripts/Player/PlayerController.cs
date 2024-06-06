using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Decanautes.Interactable;


public class PlayerController : MonoBehaviour
{
    //Components
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    public Transform GrabPoint;

    //Variable
    private Vector2 _moveDirection = Vector2.zero;
    [Sirenix.OdinInspector.ReadOnly]
    public bool CanMove = true;
    public bool CanInteract = true;

    public PlayerData PlayerData;
    public LayerMask InteractionLayer;

    [SerializeField, Sirenix.OdinInspector.ReadOnly]
    private Interactable lookingAt;
    [Sirenix.OdinInspector.ReadOnly]
    public Grabbable grabbed;
    public PostIt PostItEditing;
    public InputScreen InputScreenEditing;



    void Start()
    {
        CanMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = new PlayerInput();
        _playerInput.Enable();


        //Input Events
        _playerInput.InGame.Move.performed += Move;
        _playerInput.InGame.Interact.performed += Interact;
        _playerInput.InGame.InteractSec.performed += InteractSec;
        _playerInput.InGame.Escape.performed += Escape;
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

    public void UIScreenBlock(bool isBlocked)
    {
        CanMove = !isBlocked;
        CanInteract = !isBlocked;
        if (isBlocked)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    private void Move(InputAction.CallbackContext callbackContext)
    {
        if (!CanMove)
        {
            _moveDirection = Vector3.zero;
            return;
        }
        _moveDirection  = callbackContext.ReadValue<Vector2>();
    }

    private void Interact(InputAction.CallbackContext callbackContext)
    {
        
        //If there is a grabbed object
        if (grabbed)
        {
            if (callbackContext.ReadValueAsButton())
            {
                grabbed.InteractionStart();
            }
            else
            {
                grabbed.InteractionEnd();
            }
            return;
        }

        //If we're looking at smthg and not grabbing
        if (!lookingAt)
        {
            return;
        }
        if (callbackContext.ReadValueAsButton())
        {
            if (lookingAt.GetType() == typeof(Grabbable))
            {
                grabbed = lookingAt.GetComponent<Grabbable>();
            }
            lookingAt.InteractionStart();
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
        if (!CanInteract)
        {
            return;
        }
        ManagePostItInteraction();
        ManageInputScreenInteraction();
    }

    private void Escape(InputAction.CallbackContext callbackContext)
    {
        if (UIManager.Instance)
        {
            UIManager.Instance.Escape();
        }
    }


    private void ManagePostItInteraction()
    {
        if (PostItEditing)
        {
            if (PostItEditing.isEditing)
            {
                CanMove = true;
                PostItEditing.DeselectText();
                PostItEditing = null;
            }
            return;
        }
        if (grabbed && grabbed.TryGetComponent<PostIt>(out PostIt postIt))
        {
            bool isEditing = postIt.SelectText();
            if (!isEditing)
                return;
            PostItEditing = postIt;
            CanMove = false;
        }
        if (lookingAt && lookingAt.TryGetComponent<PostIt>(out PostIt postIt1))
        {
            bool isEditing = postIt1.SelectText();
            if (!isEditing)
                return;
            PostItEditing = postIt1;
            CanMove = false;
        }
    }

    private void ManageInputScreenInteraction()
    {
        if (InputScreenEditing)
        {
            if (InputScreenEditing.IsEditing)
            {
                if (InputScreenEditing.DoStopMovementsWhenIsEditing)
                {
                    CanMove = true;
                }
                InputScreenEditing.DeselectText();
                InputScreenEditing = null;
            }
            return;
        }
        if (grabbed || PostItEditing != null)
        {
            return;
        }
        if (lookingAt && lookingAt.TryGetComponent<InputScreen>(out InputScreen inputScreen1))
        {
            bool isEditing = inputScreen1.SelectText();
            if (!isEditing)
                return;
            InputScreenEditing = inputScreen1;
            if (InputScreenEditing.DoStopMovementsWhenIsEditing)
            {
                CanMove = false;
            }
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
                if (hit.transform.TryGetComponent<PostIt>(out PostIt postIt))
                {
                    if (!postIt.CanHover())
                        return;
                }
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
