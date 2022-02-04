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
        private Timer _slingshotDurationTimer;

        [SerializeField]
        private float _minLaunchForce = 20f;

        [SerializeField]
        private float _maxLaunchForce = 100f;

        [SerializeField]
        private float _angleLimit = 45f;

        [SerializeField]
        private bool _preventUseIfNoDirection = false;

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
        private GameObject _rotator = null;

        [SerializeField]
        private DamageHandler _damageHandler = null;

        private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
        private CubeController _cubeController = null;
        private PlayerController _playerController = null;
        private CameraAimController _slingshotCameraAimController = null;
        private CameraAimController _shootingCameraAimController = null;
        private ProjectileLauncherController _projectileLauncherController = null;
        private BoneSphere _boneSphere = null;

        private float _currentLaunchForce = 20f;
        private float _maxRaycasterDistance = 0f;
        private float _maxSlingshottingDuration = 2f;
        private bool _slingshotCharging = false;
        private bool _slingshotting = false;

        public float AngleLimit => _angleLimit;

        public Timer SlingshotTimer => _slingshotDurationTimer;

        public bool Slingshotting => _slingshotting;

        public bool ChargingSlingshot => _slingshotCharging;

        private void Awake()
        {
            _playerRefs.TryGetCubeController(out _cubeController);
            _playerRefs.TryGetPlayerController(out _playerController);
            _playerRefs.TryGetSlingshotCameraAimController(out _slingshotCameraAimController);
            _playerRefs.TryGetShootingCameraAimController(out _shootingCameraAimController);
            _playerRefs.TryGetProjectileLauncherController(out _projectileLauncherController);
            _playerRefs.TryGetBoneSphere(out _boneSphere);
            _maxRaycasterDistance = _raycaster.MaxDistance;
            _maxSlingshottingDuration = _slingshotDurationTimer.Duration;
            _currentLaunchForce = _minLaunchForce;
        }

        private void OnEnable()
        {
            _playerController.ChargeSlingshotPerformed -= ChargeSlingshot;
            _playerController.ReleaseSlingshotPerformed -= ReleaseSlingshot;
            _slingshotDurationTimer.StateChanged -= SlingshotTimerUpdated;

            _playerController.ChargeSlingshotPerformed += ChargeSlingshot;
            _playerController.ReleaseSlingshotPerformed += ReleaseSlingshot;
            _slingshotDurationTimer.StateChanged += SlingshotTimerUpdated;

            _rigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
        }

        private void OnDisable()
        {
            _playerController.ChargeSlingshotPerformed -= ChargeSlingshot;
            _playerController.ReleaseSlingshotPerformed -= ReleaseSlingshot;
            _slingshotDurationTimer.StateChanged -= SlingshotTimerUpdated;
        }

        private void Update()
        {
            if (_slingshotCharging)
            {
                Vector3 direction = _playerController.LookDirection;

                if (direction != Vector3.zero)
                {
                    if (_preventUseIfNoDirection == true && _lineRenderer.enabled == false)
                    {
                        _lineRenderer.enabled = true;
                    }
                    if (Quaternion.LookRotation(direction, _rotator.transform.up).eulerAngles.x > 360 - _angleLimit || Quaternion.LookRotation(direction, _rotator.transform.up).eulerAngles.x == 0)
                        _rotator.transform.rotation = Quaternion.LookRotation(direction, _rotator.transform.up);
                }
                else
                {
                    direction = _playerController.transform.forward;
                    if (_preventUseIfNoDirection == true && _lineRenderer.enabled == false)
                    {
                        _lineRenderer.enabled = true;
                    }
                    _rotator.transform.rotation = Quaternion.LookRotation(direction, _rotator.transform.up);
                    /*_rotator.transform.rotation = Quaternion.LookRotation(_playerController.transform.forward);
                    if (_preventUseIfNoDirection == true && _lineRenderer.enabled == true)
                    {
                        _lineRenderer.enabled = false;
                    }*/
                }
                UpdateLineRenderer();

                _chargingDurationTimer.Update();
                _raycaster.SetMaxDistance(_chargingDurationTimer.Progress * _maxRaycasterDistance);
                _currentLaunchForce = _maxLaunchForce * _chargingDurationTimer.Progress;
                if (_currentLaunchForce < _minLaunchForce)
                    _currentLaunchForce = _minLaunchForce;
            }
            if (_slingshotDurationTimer.IsRunning)
            {
                _slingshotDurationTimer.Update();
            }
            else if(_slingshotting && (_playerController.HorizontalMove != 0 || _playerController.IsJumpButtonPressed))
            {
                ResetSlingshot();
            }
        }

        private void ChargeSlingshot(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_cubeController.CurrentState == CubeController.State.Grounded && _cubeController.CurrentStamina >= 0.5f)
            {
                //_cubeController.enabled = false;
                _slingshotCameraAimController.enabled = true;
                _shootingCameraAimController.enabled = false;
                _projectileLauncherController.enabled = false;
                _slingshotCharging = true;
                _cubeController.StopStaminaRegen();
                _chargingDurationTimer.ResetTimeElapsed();
                _chargingDurationTimer.Start();
            }
        }

        private void ReleaseSlingshot(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_slingshotCharging == true && _cubeController.UseStamina(0.5f) == true)
            {
                //_boneSphere.SpringJoints[4].spring *= 5f;
                _slingshotCharging = false;
                _slingshotting = true;
                _slingshotCameraAimController.enabled = false;
                _slingshotCameraAimController.ResetCameraAimPosition();
                _lineRenderer.enabled = false;
                _slingshotDurationTimer.Start();
                if (_chargingDurationTimer.IsRunning == true)
                    _slingshotDurationTimer.TimeElapsed = 1 - _chargingDurationTimer.TimeElapsed;
                _chargingDurationTimer.ForceFinishState();
                _cubeController.StartStaminaRegen();
                foreach (Rigidbody rigidbody in _rigidbodies)
                {
                    rigidbody.AddForce(_playerController.LookDirection * _currentLaunchForce, ForceMode.Impulse);
                }
                _damageHandler.UpdateState(DamageHandler.DamageStates.Slingshotting);
                print((_playerController.LookDirection * _currentLaunchForce).ToString("F4"));
                _playerController.enabled = false;
            }
        }

        private void SlingshotTimerUpdated(Timer timer, Timer.State state)
        {
            switch (state)
            {
                case Timer.State.Stopped:
                    break;
                case Timer.State.Running:
                    break;
                case Timer.State.Finished:
                    {
                        if (_slingshotting)
                        {
                            _playerController.enabled = true;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void ResetSlingshot()
        {
            if (_slingshotDurationTimer.IsRunning)
                _slingshotDurationTimer.ForceFinishState();

            _shootingCameraAimController.enabled = true;
            _projectileLauncherController.enabled = true;
            _slingshotting = false;
            _boneSphere.SnapTimer.Start(0.03f);
            _damageHandler.UpdateState(DamageHandler.DamageStates.None);
            //_cubeController.enabled = true;

            //_boneSphere.SpringJoints[4].spring /= 5f;
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