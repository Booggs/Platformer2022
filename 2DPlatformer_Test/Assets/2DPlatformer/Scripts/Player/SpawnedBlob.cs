namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Utilities;

    public class SpawnedBlob : MonoBehaviour
    {
        //[SerializeField]
        public Timer _blobLifespanTimer;

        [SerializeField]
        private Timer _blobExplosionTimer;

        [SerializeField]
        private float _blobJumpForce = 5f;

        [SerializeField]
        private GameObject _healthBlob;

        [SerializeField]
        private int _healthBlobsToSpawn = 3;

        private BlobSeparation _parentScript = null;

        private Rigidbody[] _rigidbodies = null;

        public int HealthBlobsToSpawn
        {
            get
            {
                return _healthBlobsToSpawn;
            }
            set
            {
                _healthBlobsToSpawn = value;
            }
        }

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
            _blobLifespanTimer.Start();
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
        }

        private void OnEnable()
        {
            _blobLifespanTimer.StateChanged -= BlobLifespanChanged;
            _blobLifespanTimer.StateChanged += BlobLifespanChanged;

            _blobExplosionTimer.StateChanged -= BlobExplosionTimerChanged;
            _blobExplosionTimer.StateChanged += BlobExplosionTimerChanged;
        }

        private void OnDisable()
        {
            _blobLifespanTimer.StateChanged -= BlobLifespanChanged;
            _blobExplosionTimer.StateChanged -= BlobExplosionTimerChanged;
        }

        private void Update()
        {
            _blobLifespanTimer.Update();
            _blobExplosionTimer.Update();
        }

        private void BlobLifespanChanged(Timer timer, Timer.State status)
        {
            if(status == Timer.State.Finished)
            {
                _blobExplosionTimer.Start();
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
            for (int i = 0; i < _healthBlobsToSpawn; i++)
            {
                Instantiate(_healthBlob, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}