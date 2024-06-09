using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Decanautes.Interactable;
using Cinemachine;
using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    //Components
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    public Transform GrabPoint;
    public CinemachineVirtualCamera VirtualCamera;

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
    public Vector3 PostItEditingLastPos;
    public Vector3 PostItEditingLastRot;
    public Transform PostItEditPos;
    public InputScreen InputScreenEditing;



    void Start()
    {
        CanMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = new PlayerInput();
        _playerInput.Enable();

        UIScreenBlock(false);

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
            VirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = 0;
            VirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = 0;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            VirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = PlayerPreferencesManager.Instance.PlayerPreferencesData.CameraSensibility;
            VirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = PlayerPreferencesManager.Instance.PlayerPreferencesData.CameraSensibility;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    #region Input
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
        if (PostItEditing != null)
        {
            return;
        }
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

        if (!lookingAt)
        {
            return;
        }
        //If we're looking at smthg and not grabbing
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

    #endregion

    #region postIt
    private void ManagePostItInteraction()
    {
        if (PostItEditing)
        {
            if (PostItEditing.isEditing)
            {
                CanMove = true;
                PostItEditing.DeselectText();
                if (PostItEditing.UsesLeft == PostItEditing.MaxUses-1)
                    ResetPostItPos(PostItEditing);
                else
                    MovePostItTo(PostItEditing, PostItEditingLastPos, PostItEditingLastRot);
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
            PostItEditingLastPos = postIt.transform.position;
            PostItEditingLastRot = postIt.transform.eulerAngles;
            MovePostItTo(postIt, PostItEditPos);
        }
        if (lookingAt && lookingAt.TryGetComponent<PostIt>(out PostIt postIt1))
        {
            bool isEditing = postIt1.SelectText();
            if (!isEditing)
                return;
            PostItEditing = postIt1;
            CanMove = false;
            PostItEditingLastPos = postIt1.transform.position;
            PostItEditingLastRot = postIt1.transform.eulerAngles;
            MovePostItTo(postIt1, PostItEditPos);
        }
    }

    private void ResetPostItPos(PostIt postIt)
    {
        postIt.transform.DOLocalMove(Vector3.zero, 0.35f);
        postIt.transform.DOLocalRotate(Vector3.zero, 0.35f);
    }

    private void MovePostItTo(PostIt postIt, Transform towards)
    {
        postIt.transform.DOLocalMove(towards.localPosition, 0.35f);
        postIt.transform.DOLocalRotate(towards.localEulerAngles, 0.35f);
    }
    private void MovePostItTo(PostIt postIt, Vector3 pos, Vector3 rot)
    {
        postIt.transform.DOLocalMove(postIt.transform.InverseTransformPoint(pos), 0.35f);
        postIt.transform.DOLocalRotate(rot, 0.35f);
    }

    #endregion

    #region Input Screen
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
    #endregion


    #region Look
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
    #endregion
}
