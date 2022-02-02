namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;
    using GSGD2.Utilities;

    public class SlingshotHandler : MonoBehaviour
    {
        [Header("Balancing")]
        [SerializeField]
        private Timer _chargingDurationTimer;

        [SerializeField]
        private float _minLaunchForce = 20f;

        [SerializeField]
        private float _maxLaunchForce = 100f;

        [SerializeField]
        private bool _preventUseIfNoDirection;

        [Header("References")]
        [SerializeField]
        private PlayerReferences _playerRefs = null;

        [SerializeField]
        private LineRenderer _lineRenderer = null;

        [SerializeField]
        private Raycaster _raycaster = null;

        [SerializeField]
        private ProjectileLauncher _projectileLauncher = null;

        [SerializeField]
        private GameObject _rotator;

        private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
        private CubeController _cubeController = null;
        private PlayerController _playerController = null;
        private CameraAimController _slingshotCameraAimController = null;
        private CameraAimController _shootingCameraAimController = null;
        private ProjectileLauncherController _projectileLauncherController = null;

        private bool _slingshotCharging = false;

        private void Awake()
        {
            _playerRefs.TryGetCubeController(out _cubeController);
            _playerRefs.TryGetPlayerController(out _playerController);
            _playerRefs.TryGetSlingshotCameraAimController(out _slingshotCameraAimController);
            _playerRefs.TryGetShootingCameraAimController(out _shootingCameraAimController);
            _playerRefs.TryGetProjectileLauncherController(out _projectileLauncherController);
        }

        private void OnEnable()
        {
            _playerController.ChargeSlingshotPerformed -= ChargeSlingshot;
            _playerController.ReleaseSlingshotPerformed -= ReleaseSlingshot;

            _playerController.ChargeSlingshotPerformed += ChargeSlingshot;
            _playerController.ReleaseSlingshotPerformed += ReleaseSlingshot;

            _rigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
        }

        private void OnDisable()
        {
            _playerController.ChargeSlingshotPerformed -= ChargeSlingshot;
            _playerController.ReleaseSlingshotPerformed -= ReleaseSlingshot;
        }

        private void Update()
        {
            Vector3 direction = _playerController.LookDirection;

            if (direction != Vector3.zero)
            {
                if (_preventUseIfNoDirection == true && _lineRenderer.enabled == false)
                {
                    _lineRenderer.enabled = true;
                }
                _rotator.transform.rotation = Quaternion.LookRotation(direction, _rotator.transform.up);
            }
            else
            {
                _rotator.transform.rotation = Quaternion.LookRotation(_playerController.transform.forward);
                if (_preventUseIfNoDirection == true && _lineRenderer.enabled == true)
                {
                    _lineRenderer.enabled = false;
                }
            }
            UpdateLineRenderer();
        }

        private void ChargeSlingshot(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_cubeController.CurrentState == CubeController.State.Grounded)
            {
                _slingshotCameraAimController.enabled = true;
                _shootingCameraAimController.enabled = false;
                _projectileLauncherController.enabled = false;
            }
        }

        private void ReleaseSlingshot(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_slingshotCharging == true)
            {
                _slingshotCameraAimController.enabled = false;
                _shootingCameraAimController.enabled = true;
                _projectileLauncherController.enabled = true;
            }
        }

        private void UpdateLineRenderer()
        {
            bool result = _raycaster.Raycast(out RaycastHit hit);
            var projectileInstanceOffset = _projectileLauncher.ProjectileInstanceOffset;
            Vector3 startPosition = projectileInstanceOffset.position;
            if (result == true)
            {
                _lineRenderer.SetPositions(new Vector3[2] { startPosition, hit.point });
            }
            else
            {
                _lineRenderer.SetPositions(new Vector3[2] { startPosition, startPosition + projectileInstanceOffset.forward * _raycaster.MaxDistance });
            }
        }
    }
}