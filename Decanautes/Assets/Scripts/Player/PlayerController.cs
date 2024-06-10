using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Decanautes.Interactable;
using Cinemachine;
using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    //Components
    [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    public Transform GrabPoint;
    public CinemachineVirtualCamera VirtualCamera;

    //Variable
    private Vector2 _moveDirection = Vector2.zero;
    [Sirenix.OdinInspector.ReadOnly]
    public bool CanMove = true;
    public bool CanInteract = true;
    private bool IsUiPostItBlock = false;
    private bool IsUiScreenBlock = false;

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


    //Input actions
    private InputAction _moveAction;
    private InputAction _interactAction;
    private InputAction _interactSecAction;
    private InputAction _escapeAction;

    void Start()
    {
        CanMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        _rigidbody = GetComponent<Rigidbody>();

        UIScreenBlock(false);

        SetupInputActions();
    }

    void Update()
    {
        UpdateInputs();
        LookInteraction();
    }

    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized * _moveDirection.y + Camera.main.transform.right * _moveDirection.x;
        _rigidbody.AddForce(dir * PlayerData.MoveSpeed);
        Vector3 currentVelocity = _rigidbody.velocity;
        _rigidbody.velocity = new Vector3(currentVelocity.x / (PlayerData.Drag * Time.fixedDeltaTime*100), currentVelocity.y, currentVelocity.z / (PlayerData.Drag * Time.fixedDeltaTime * 100));
    }

    #region Inputs
    private void SetupInputActions()
    {
        _moveAction = _playerInput.actions["Move"];
        _escapeAction = _playerInput.actions["Escape"];
        _interactAction = _playerInput.actions["Interact"];
        _interactSecAction = _playerInput.actions["InteractSec"];
    }

    private void UpdateInputs()
    {
        Move();
        if (_escapeAction.WasPressedThisFrame()) Escape();
        if (_interactAction.WasPressedThisFrame()) Interact();
        if (_interactSecAction.WasPressedThisFrame()) InteractSec();
    }

    public void UIScreenBlock(bool isBlocked)
    {
        IsUiScreenBlock = isBlocked;
        if (!isBlocked && IsUiPostItBlock)
        {
            return;
        }
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

    public void UIPostItBlock(bool isBlocked)
    {
        IsUiPostItBlock = isBlocked;
        CanMove = !isBlocked;
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

    private void Move()
    {
        if (!CanMove)
        {
            _moveDirection = Vector3.zero;
            return;
        }
        _moveDirection  = _moveAction.ReadValue<Vector2>();
    }

    private void Interact()
    {
        EnterPostItView();
        if (PostItEditing != null)
        {
            return;
        }
        //If there is a grabbed object
        if (grabbed && !lookingAt)
        {
            if (_interactAction.IsPressed())
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
        if (_interactAction.IsPressed())
        {
            ManageInputScreenInteraction();
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

    private void InteractSec()
    {
        if (!CanInteract)
        {
            return;
        }
        ManagePostItInteraction();
        if (InputScreenEditing)
        {
            ManageInputScreenInteraction();
        }
    }

    private void Escape()
    {
        if (UIManager.Instance)
        {
            UIManager.Instance.Escape();
        }
    }

    #endregion

    #region postIt

    private void EnterPostItView()
    {
        if (IsUiScreenBlock || InputScreenEditing)
        {
            return;
        }
        if (PostItEditing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.gameObject == PostItEditing.gameObject)
                {
                    return;
                }
            }
            if (!PostItEditing.isEditing)
                ExitPostItView();
            return;
        }
        if (lookingAt && lookingAt.TryGetComponent<PostIt>(out PostIt postIt1))
        {
            PostItEditing = postIt1;
            PostItEditingLastPos = postIt1.transform.position;
            PostItEditingLastRot = postIt1.transform.eulerAngles;
            PostItEditing.EnterPostItView();
            MovePostItTo(postIt1, PostItEditPos, false);
            UIPostItBlock(true);
        }
    }

    private void ExitPostItView()
    {
        MovePostItTo(PostItEditing, PostItEditingLastPos, PostItEditingLastRot);
        PostItEditing.ExitPostItView();
        PostItEditing = null;
        UIPostItBlock(false);
    }

    public void ManagePostItInteraction()
    {
        if (InputScreenEditing)
        {
            return;
        }
        if (PostItEditing)
        {
            if (PostItEditing.isEditing)
            {
                PostItEditing.DeselectText();
                if (!PostItEditing.IsPosted)
                    ResetPostItPos(PostItEditing);
                else
                    MovePostItTo(PostItEditing, PostItEditingLastPos, PostItEditingLastRot);
                PostItEditing = null;
                UIPostItBlock(false);
            }
            else
            {
                ExitPostItView();
            }
            return;
        }
        if (grabbed && grabbed.TryGetComponent<PostIt>(out PostIt postIt))
        {
            bool isEditing = postIt.SelectText();
            if (!isEditing)
                return;
            PostItEditing = postIt;
            PostItEditingLastPos = postIt.transform.position;
            PostItEditingLastRot = postIt.transform.eulerAngles;
            MovePostItTo(postIt, PostItEditPos, true);
            UIPostItBlock(true);
        }
    }

    private void ResetPostItPos(PostIt postIt)
    {
        postIt.transform.DOLocalMove(Vector3.zero, 0.1f);
        postIt.transform.DOLocalRotate(Vector3.zero, 0.1f);
    }

    private void MovePostItTo(PostIt postIt, Transform towards, bool isLocal)
    {
        if (isLocal)
        {
            postIt.transform.DOLocalMove(towards.localPosition, 0.1f);
            postIt.transform.DOLocalRotate(towards.localEulerAngles, 0.1f);
        }
        else
        {
            postIt.transform.DOMove(towards.position, 0.1f);
            postIt.transform.DORotate(towards.eulerAngles, 0.1f);
        }
    }
    private void MovePostItTo(PostIt postIt, Vector3 pos, Vector3 rot)
    {
        postIt.transform.DOMove(pos, 0.1f);
        postIt.transform.DORotate(rot, 0.1f);
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
        if (PostItEditing != null)
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
