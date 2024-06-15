using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Decanautes.Interactable;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PostIt : MonoBehaviour
{
    [TitleGroup("Components")]
    public PostItData PostItData;
    public GameObject Model;
    public GameObject PreviewModel;
    public TMP_InputField InputText;
    public TMP_Text CycleText;
    public TMP_Text VoteText;
    private Rigidbody rigidbody;

    [TitleGroup("Parameters")]
    public LayerMask NotPostItLayer;
    public UnityEvent OnEnterView;
    public UnityEvent OnExitView;
    public UnityEvent OnEnterEditorView;
    public UnityEvent OnExitEditorView;
    public UnityEvent OnEnterEdit;
    public UnityEvent OnExitEdit;
    public int Vote;
    public int Voted;
    public int VoteTresholdDelete;

    [TitleGroup("Debug")]
    [ReadOnly]
    public int CycleNum;
    [SerializeField, ReadOnly]
    private bool _isPosting;
    [ReadOnly]
    public bool IsPosted;
    [ReadOnly]
    public int UsesLeft;
    public int MaxUses;
    [ReadOnly]
    public int ModifLeft;
    public int MaxModif;
    [ReadOnly]
    public bool isEditing;
    [ReadOnly]
    public bool isViewing;
    [ReadOnly]
    public bool _isValid;
    [SerializeField, ReadOnly]
    private Vector3 _validPosition;
    [SerializeField, ReadOnly]
    private Quaternion _validRotation;
    [ReadOnly]
    public bool isStarting = true;


    private void Awake()
    {
        UsesLeft = MaxUses;
        ModifLeft = MaxModif;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Voted = Vote;
        VoteText.text = Voted.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPosting)
        {
            GetPostItSurface();
        }
    }

    private void GetPostItSurface()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, PostItData.DistanceMax, NotPostItLayer))
        {
            PreviewPostIt(hitInfo);
            _validPosition = PreviewModel.transform.position;
            _validRotation = PreviewModel.transform.rotation;
            Renderer previewRenderer = PreviewModel.GetComponent<Renderer>();
            if (PostItData.PostItSurface ==  (PostItData.PostItSurface | 1 << hitInfo.transform.gameObject.layer))
            {
                _isValid = true;
                previewRenderer.material = PostItData.ValidMaterial;
            }
            else
            {
                _isValid = false;
                previewRenderer.material = PostItData.InvalidMaterial;
            }
        }
        else
        {
            PreviewModel.SetActive(false);
        }
    }

    private void PreviewPostIt(RaycastHit hitInfo)
    {
        PreviewModel.SetActive(true);
        PreviewModel.transform.position = hitInfo.point;
        PreviewModel.transform.rotation = Quaternion.LookRotation(hitInfo.normal);
        if (hitInfo.normal == Vector3.up)
        {
            Quaternion toCamera = Quaternion.LookRotation(Camera.main.transform.position - PreviewModel.transform.position);
            PreviewModel.transform.localEulerAngles = new Vector3(PreviewModel.transform.localEulerAngles.x, PreviewModel.transform.localEulerAngles.y, toCamera.eulerAngles.z);
        }
    }

    public void SetCycleText()
    {
        CycleText.text = "Cycle : " + MapManager.Instance.MapData.CurrentCycle;
        CycleNum = MapManager.Instance.MapData.CurrentCycle;
    }

    public void StartPosting()
    {
        PreviewModel.SetActive(true);
        _isPosting = true;
        IsPosted = false;
    }
    public void StopPosting()
    {
        PreviewModel.SetActive(false);
        _isPosting = false;
        if (_isValid)
        {
            IsPosted = true;
            Post();
        }
    }

    public void Post()
    {
        transform.position = _validPosition;
        transform.rotation = _validRotation;
        GetComponent<Grabbable>().SpawnerReset();
        if (LastMinute.Instance.IsLastDecaNote)
        {
            LastMinute.Instance.EndGame();
        }
        /*Quaternion toCamera = Quaternion.LookRotation(Camera.main.transform.position - transform.position);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, toCamera.eulerAngles.z);*/
    }

    public bool CanHover()
    {
        if (ModifLeft > 0)
        {
            return true;
        }
        return false;
    }

    public void LockPostIt()
    {
        UsesLeft = 0;
        ModifLeft = 0;
        InputText.enabled = false;
    }

    public void IncrementVote(int toAdd)
    {
        Voted = Vote + toAdd;
        VoteText.text = Voted.ToString();
        if (Voted <= VoteTresholdDelete)
        {
            DeletePostIt();
        }
    }

    //TEXT

    [Button("Select Text")]
    public bool SelectText()
    {
        if (ModifLeft <= 0)
        {
            InputText.enabled = false;
            return false;
        }
        ModifLeft--;
        InputText.Select();
        EnterPostItEdit();
        isEditing = true;
        return true;
    }

    public void DeselectText()
    {
        InputText.ReleaseSelection();
        isEditing = false;
        ExitPostItEdit();
        if (ModifLeft <= 0)
        {
            InputText.enabled = false;
        }
        if (UsesLeft == MaxUses)
        {
            StartPosting();
            UsesLeft --;
        }
    }

    public void EnterPostItView()
    {
        isViewing = true;
        if (CycleNum == MapManager.Instance.MapData.CurrentCycle)
        {
            OnEnterEditorView?.Invoke();
            return;
        }
        OnEnterView?.Invoke();
    }
    public void ExitPostItView()
    {
        EventSystem.current.SetSelectedGameObject(null);
        isViewing = false;
        if (CycleNum == MapManager.Instance.MapData.CurrentCycle)
        {
            OnExitEditorView?.Invoke();
            return;
        }
        OnExitView?.Invoke();
    }

    public void EnterPostItEdit()
    {
        OnEnterEdit?.Invoke();
    }
    public void ExitPostItEdit()
    {
        EventSystem.current.SetSelectedGameObject(null);
        OnExitEdit?.Invoke();
    }

    public void DeletePostIt()
    {
        Destroy(gameObject);
        GameObject.FindFirstObjectByType<PlayerController>().UIPostItBlock(false);
    }
    public void ConfirmPostIt()
    {
        GameObject.FindFirstObjectByType<PlayerController>().ManagePostItInteraction();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * PostItData.DistanceMax);

    }
}
