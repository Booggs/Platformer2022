namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Utilities;

    public class SpawnedBlob : MonoBehaviour
    {
        [SerializeField]
        private Timer _blobLifespan;

        [SerializeField]
        private Timer _blobExplosionTimer;

        [SerializeField]
        private float _blobJumpForce = 10f;

        private BlobSeparation _parentScript = null;

        private Rigidbody[] _rigidbodies = null;

        public BlobSeparation ParentScript
        {
            get
            {
                return _parentScript;
            }
            set
            {
                _parentScript = value;
            }
        }

        private void Awake()
        {
            _blobLifespan.Start();
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
        }

        private void OnEnable()
        {
            _blobLifespan.StateChanged -= BlobLifespanChanged;
            _blobLifespan.StateChanged += BlobLifespanChanged;

            _blobExplosionTimer.StateChanged -= BlobExplosionTimerChanged;
            _blobExplosionTimer.StateChanged += BlobExplosionTimerChanged;
        }

        private void OnDisable()
        {
            _blobLifespan.StateChanged -= BlobLifespanChanged;
            _blobExplosionTimer.StateChanged -= BlobExplosionTimerChanged;
        }

        private void Update()
        {
            _blobLifespan.Update();
            _blobExplosionTimer.Update();
        }

        private void BlobLifespanChanged(Timer timer, Timer.State status)
        {
            if(status == Timer.State.Finished)
            {
                //_blobExplosionTimer.Start();
                KillBlob();
            }
        }

        private void BlobExplosionTimerChanged(Timer timer, Timer.State status)
        {
            switch(status)
            {
                case Timer.State.Running:
                    {
                        foreach (Rigidbody rigidbody in _rigidbodies)
                        {
                            rigidbody.velocity = new Vector3(0f, _blobJumpForce, 0f);
                        }
                    }
                    break;
                case Timer.State.Stopped:
                    break;
                case Timer.State.Finished:
                    {
                        KillBlob();
                    }
                    break;
            }
        }

        public void KillBlob()
        {
            _parentScript.SpawnedBlob = null;
            Destroy(gameObject);
        }
    }
}