using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Grabbable : Interactable
{
    [TitleGroup("Components")]
    public GrabbableData GrabbableData;
    private Rigidbody _rigidbody;
    private PlayerController _playerController;

    [TitleGroup("Debug")]
    [SerializeField,ReadOnly]
    private bool _isGrabbed;
    [SerializeField,ReadOnly]
    private bool _isTransformBased = false;
    [SerializeField,ReadOnly]
    private Transform _grabTransform;
    [SerializeField,ReadOnly]
    private Vector3 _grabPosition;
    private Transform _originParent;
    private bool _wasKinematic;

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            _rigidbody = rigidbody;
        }
        _originParent = transform.parent;
        _playerController = GameObject.FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        if (_isGrabbed && GrabbableData.isSimulated)
        {
            GrabSimulation();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public override void InteractionStart()
    {
        base.InteractionStart();
        if (!IsToggle)
        {
            GrabToTransform(_playerController.GrabPoint);
        }
        else
        {
            if (isActivated)
            {
                GrabToTransform(_playerController.GrabPoint);
            }
            else
            {
                Release();
            }
        }
    }

    public override void InteractionEnd()
    {
        base.InteractionEnd();
        if (!IsToggle)
        {
            Release();
        }
    }

    private void GrabSimulation()
    {
        Vector3 position = _isTransformBased ? _grabTransform.position : _grabPosition;
        Quaternion rotation = _isTransformBased ? _grabTransform.rotation : Quaternion.identity;

        _rigidbody.velocity = (position - transform.position) * GrabbableData.GrabForce;
        transform.rotation = rotation;
    }

    public void GrabToTransform(Transform toTransform)
    {
        if (TryGetComponent<PostIt>(out PostIt post))
        {
            if (post.UsesLeft<=0)
            {
                return;
            }
            post.UsesLeft--;
            post.StartPosting();
        }
        SpawnerReset();
        _isTransformBased = true;
        _isGrabbed = true;
        _grabTransform = toTransform;

        _wasKinematic = _rigidbody ? _rigidbody.isKinematic : false;
        if (GrabbableData.isSimulated)
        {
            _rigidbody.isKinematic = false;
        }
        else
        {
            if (_rigidbody)
            {
                _rigidbody.isKinematic = true;
            }
        }

        if (!GrabbableData.isSimulated)
        {
            transform.position = _grabTransform.position;
            transform.rotation = _grabTransform.rotation;
            transform.parent = _grabTransform;
        }
        
    }

    public void GrabToPosition(Vector3 position)
    {
        SpawnerReset();
        _isTransformBased = false;
        _isGrabbed = true;
        _grabPosition = position;

        _wasKinematic = _rigidbody ? _rigidbody.isKinematic : false;
        if (GrabbableData.isSimulated)
        {
            _rigidbody.isKinematic = false;
        }
        else
        {
            if (_rigidbody)
            {
                _rigidbody.isKinematic = true;
            }
        }

        if (!GrabbableData.isSimulated)
        {
            transform.position = _grabPosition;
            transform.rotation = Quaternion.identity;
            transform.parent = _grabTransform;
        }
    }

    private void SpawnerReset()
    {
        Spawner spawner= GetComponentInParent<Spawner>();
        if (!spawner)
        {
            return;
        }
        _originParent = null;
        spawner.StartSpawnCoroutine();
    }

    public void Release()
    {
        if (TryGetComponent<PostIt>(out PostIt post))
        {
            if (!post._isValid)
            {
                return;
            }
            post.StopPosting();
        }
        _playerController.grabbed = null;
        if (GrabbableData.isSimulated && _rigidbody.velocity.magnitude<= 0.5f)
        {
            _rigidbody.velocity = Vector3.zero;
        }
        if (!GrabbableData.isSimulated)
        {
            transform.parent = _originParent;
        }
        if (_rigidbody)
        {
            _rigidbody.isKinematic = _wasKinematic;
        }
        _isGrabbed = false;
        
    }
}
