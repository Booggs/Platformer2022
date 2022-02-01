namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;
    using GSGD2.Utilities;
    using UnityEngine.InputSystem;

    public class BlobSeparation : MonoBehaviour
    {
        [SerializeField]
        private PlayerReferences _playerRefs = null;

        [SerializeField]
        private GameObject _blobPrefab = null;

        [SerializeField]
        private Timer _blobCooldown;

        [SerializeField]
        private Damage _spawningDamage;

        private PlayerController _playerController = null;
        private PlayerDamageable _playerDamageable = null;
        private CubeController _cubeController = null;
        private SpawnedBlob _spawnedBlob = null;



        public SpawnedBlob SpawnedBlob
        {
            get
            {
                return _spawnedBlob;
            }
            set
            {
                _spawnedBlob = value;
            }
        }

        private void Awake()
        {
            _playerRefs.TryGetPlayerController(out _playerController);
            _playerRefs.TryGetPlayerDamageable(out _playerDamageable);
            _playerRefs.TryGetCubeController(out _cubeController);
        }

        private void OnEnable()
        {
            _playerController.BlobSeparationPerformed -= SeparationPerformed;
            _playerController.BlobSeparationPerformed += SeparationPerformed;
        }

        private void OnDisable()
        {
            _playerController.BlobSeparationPerformed -= SeparationPerformed;
        }

        private void Update()
        {
            _blobCooldown.Update();
        }

        private void SeparationPerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_spawnedBlob == null && _blobCooldown.IsRunning == false && _playerDamageable.CurrentHealth > _spawningDamage.DamageValue)
            {
                _blobCooldown.Start();
                _cubeController.LaunchBlob(new Vector3(0f, 7.5f, 0f));
                _playerDamageable.TakeDamage(_spawningDamage);

                GameObject spawnedBlob = Instantiate(_blobPrefab, transform.position, Quaternion.identity);
                _spawnedBlob = spawnedBlob.GetComponent<SpawnedBlob>();
                _spawnedBlob.ParentScript = this;
                _spawnedBlob.HealthBlobsToSpawn = _spawningDamage.DamageValue;

                return;
            }
            if (_spawnedBlob != null)
            {
                _spawnedBlob._blobLifespanTimer.TimeElapsed = _spawnedBlob._blobLifespanTimer.Duration;
                //_spawnedBlob._blobLifespanTimer.ForceFinishState();
            }
        }

    }
}