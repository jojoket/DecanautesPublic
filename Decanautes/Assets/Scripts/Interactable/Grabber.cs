using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Grabber : MonoBehaviour
{
    [TitleGroup("Components")]
    public Transform grabPoint;

    [TitleGroup("Debug")]
    [SerializeField, ReadOnly]
    private Grabbable _lookingAt;
    [SerializeField, ReadOnly]
    private Grabbable _grabbed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForGrabbable();
    }

    private void CheckForGrabbable()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        _lookingAt = null;
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (!hitInfo.transform.TryGetComponent<Grabbable>(out Grabbable grabbable))
            {
                return;
            }
            _lookingAt = grabbable;
        }
    }

    public void Grab()
    {
        if (_grabbed)
        {
            _grabbed.Release();
            _grabbed = null;
            return;
        }

        if (_lookingAt != null)
        {
            _lookingAt.GrabToTransform(grabPoint);
            _grabbed = _lookingAt;
        }
    }

}
