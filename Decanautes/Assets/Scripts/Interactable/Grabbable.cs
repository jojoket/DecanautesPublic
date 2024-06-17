using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Decanautes.Interactable
{
    public class Grabbable : Interactable
    {
        [TitleGroup("Components")]
        public GrabbableData GrabbableData;
        private Rigidbody _rigidbody;
        private PlayerController _playerController;
        [HideInInspector]
        public Spawner Spawner;

        [TitleGroup("FMod Event")]
        public FmodEventInfo GrabFmod;
        public FmodEventInfo StopGrabFmod;
        public FmodEventInfo PostPostItFmod;

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
        public bool DoRespawnFromGrab;
        

        // Start is called before the first frame update
        void Start()
        {
            HoverColor = PlayerPreferencesManager.Instance.PlayerPreferencesData.OutlineColor;
            foreach (AnimatorTriggerer animatorTriggerer in OnInteractStartedAnimations)
            {
                animatorTriggerer.parent = this;
            }
            foreach (AnimatorTriggerer animatorTriggerer in OnInteractEndedAnimations)
            {
                animatorTriggerer.parent = this;
            }
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
            if(TryGetComponent<PostIt>(out PostIt postIt) && (!postIt.isStarting && postIt.UsesLeft == postIt.MaxUses || !postIt.isStarting && !postIt._isValid))
            {
                return;
            }
            if (postIt)
            {
                postIt.isStarting = false;
            }
            if (!_playerController){
                _playerController = GameObject.FindFirstObjectByType<PlayerController>();
            }
            base.InteractionStart();
            Transform grabPoint = postIt? _playerController.PostItGrabPoint : _playerController.GrabPoint;
            if (!IsToggle)
            {
                GrabToTransform(grabPoint);
            }
            else
            {
                if (isActivated)
                {
                    GrabToTransform(grabPoint);
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
                    _playerController.grabbedPostIt = null;
                    return;
                }
                if (post.UsesLeft != post.MaxUses)
                {
                    post.StartPosting();
                    post.UsesLeft--;
                }
            }
            else
            {
                if (GrabFmod.EventPosition)
                    RythmManager.Instance.StartFmodEvent(GrabFmod);
            }
            if (DoRespawnFromGrab)
            {
                SpawnerReset();
            }
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
            if (DoRespawnFromGrab)
            {
                SpawnerReset();
            }
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

        public void SpawnerReset()
        {
            if (!Spawner)
            {
                return;
            }
            _originParent = null;
            Spawner.StartSpawnCoroutine();
            Spawner = null;
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
                _playerController.grabbedPostIt = null;
            }
            else
            {
                if (StopGrabFmod.EventPosition)
                    RythmManager.Instance.StartFmodEvent(StopGrabFmod);
                _playerController.grabbedObj = null;
            }
            if (GrabbableData.isSimulated && _rigidbody.velocity.magnitude<= 0.5f)
            {
                _rigidbody.velocity = Vector3.zero;
            }
            if (!GrabbableData.isSimulated)
            {
                transform.parent = null;
            }
            if (_rigidbody)
            {
                _rigidbody.isKinematic = _wasKinematic;
            }
            _isGrabbed = false;
        
        }
    }
}
