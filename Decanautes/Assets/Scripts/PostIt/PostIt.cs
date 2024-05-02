using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class PostIt : MonoBehaviour
{
    [TitleGroup("Components")]
    public PostItData PostItData;
    public GameObject Model;
    public GameObject PreviewModel;
    public TMP_InputField Text;
    private Rigidbody rigidbody;

    [TitleGroup("Parameters")]
    public LayerMask NotPostItLayer;

    [TitleGroup("Debug")]
    [SerializeField, ReadOnly]
    private bool _isPosting;
    [SerializeField, ReadOnly]
    private bool _isPosted;
    public int UsesLeft;
    [SerializeField]
    private int _modifLeft;
    [ReadOnly]
    public bool _isValid;
    [SerializeField, ReadOnly]
    private Vector3 _validPosition;
    [SerializeField, ReadOnly]
    private Quaternion _validRotation;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
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

    public void StartPosting()
    {
        PreviewModel.SetActive(true);
        _isPosting = true;
        _isPosted = false;
    }
    public void StopPosting()
    {
        PreviewModel.SetActive(false);
        _isPosting = false;
        if (_isValid)
        {
            _isPosted = true;
            Post();
        }
    }

    public void Post()
    {
        transform.position = _validPosition;
        transform.rotation = _validRotation;
        /*Quaternion toCamera = Quaternion.LookRotation(Camera.main.transform.position - transform.position);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, toCamera.eulerAngles.z);*/
    }

    public bool CanHover()
    {
        if (_modifLeft > 0)
        {
            return true;
        }
        return false;
    }

    public void LockPostIt()
    {
        Debug.Log("locked", this);
        UsesLeft = 0;
        _modifLeft = 0;
    }

    //TEXT

    [Button("Select Text")]
    public void SelectText()
    {
        if (_modifLeft <= 0)
        {
            return;
        }
        _modifLeft--;
        Text.Select();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * PostItData.DistanceMax);

    }
}
