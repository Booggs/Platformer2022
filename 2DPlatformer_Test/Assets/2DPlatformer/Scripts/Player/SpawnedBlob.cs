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

        private BlobSeparation _parentScript = null;
        private MovingPlatformInteractorActivator = null;

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
        }

        private void OnEnable()
        {
            _blobLifespan.StateChanged -= BlobLifespanChanged;
            _blobLifespan.StateChanged += BlobLifespanChanged;
        }

        private void OnDisable()
        {
            _blobLifespan.StateChanged -= BlobLifespanChanged;
        }

        private void Update()
        {
            _blobLifespan.Update();
        }

        private void BlobLifespanChanged(Timer timer, Timer.State status)
        {
            if(status == Timer.State.Finished)
            {
                KillBlob();
            }
        }

        public void KillBlob()
        {
            _parentScript.SpawnedBlob = null;
            Destroy(gameObject);
        }
    }
}