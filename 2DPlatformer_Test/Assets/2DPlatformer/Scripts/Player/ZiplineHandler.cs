namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;
    using UnityEngine.InputSystem;

    public class ZiplineHandler : MonoBehaviour
    {
        [SerializeField]
        private PlayerReferences _playerRefs = null;

        [SerializeField]
        private float _startingZiplineSpeed = 5f;

        /*[SerializeField]
        private float _maxAccelerationTime = 3f;*/

        private Zipline _currentZipline = null;
        private bool _ziplining = false;
        private bool _zipliningLeft = false;
        private Vector3 _zipliningDestination;
        private PlayerController _playerController = null;
        private CubeController _cubeController = null;
        private Rigidbody _rigidbody = null;
        private float _currentZiplineSpeed = 5f;
        private float _maxZiplineSpeed = 0f;
        private float _accelerationFactor = 0f;

        public bool ZipliningLeft => _zipliningLeft;

        private void Awake()
        {
            _playerRefs.TryGetPlayerController(out _playerController);
            _playerRefs.TryGetCubeController(out _cubeController);
            _playerRefs.TryGetRigidbody(out _rigidbody);
        }

        private void OnEnable()
        {
            _playerController.JumpPerformed -= PlayerController_JumpPerformed;
            _playerController.JumpPerformed += PlayerController_JumpPerformed;
        }

        private void OnDisable()
        {
            _playerController.JumpPerformed -= PlayerController_JumpPerformed;
        }

        public void SetZipline(Zipline zipline)
        {
            _currentZiplineSpeed = _startingZiplineSpeed;
            _currentZipline = zipline;
            _ziplining = true;
            if (_cubeController.Rigidbody.velocity.z >= 0)
            {
                _zipliningLeft = false;
                _zipliningDestination = _currentZipline.ZiplineTarget.position;
            }
            else
            {
                _zipliningLeft = true;
                _zipliningDestination = _currentZipline.ZiplineStart.position;
            }
            _cubeController.ResetRigidbodiesVelocity();
            SetNewZiplineStats();
            _cubeController.ChangeState(CubeController.State.None);
            //_cubeController.enabled = false;
            foreach (Rigidbody rigidbody in _cubeController.Rigidbodies)
            {
                rigidbody.useGravity = false;
            }
        }

        public void LeaveZipline(Zipline zipline)
        {
            if (zipline == _currentZipline)
            {
                _currentZipline = null;
                _ziplining = false;
                //_cubeController.enabled = true;
                _cubeController.ResetJumpCount(_cubeController.MaxJumpCount);
                _cubeController.ChangeState(CubeController.State.StartJump);
                foreach(Rigidbody rigidbody in _cubeController.Rigidbodies)
                {
                    rigidbody.useGravity = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (_ziplining == true && _currentZipline != null)
            {
                if (Vector3.Distance(transform.position, _zipliningDestination) > 0.5f)
                {
                    if (transform.position.y > _zipliningDestination.y)
                    {

                        if (_currentZiplineSpeed > _maxZiplineSpeed)
                        {
                            _currentZiplineSpeed = _maxZiplineSpeed;
                        }
                        else _currentZiplineSpeed = _currentZiplineSpeed + (_accelerationFactor * Time.deltaTime);
                    }

                    transform.position += (_zipliningDestination - transform.position).normalized * _currentZiplineSpeed * Time.deltaTime;
                }
                else
                {
                    LeaveZipline(_currentZipline);
                }
            }
        }

        private void SetNewZiplineStats()
        {
            float ziplineAngle;
            if (_currentZipline.ZiplineAngle < 5f)
            {
                ziplineAngle = 10f;
            }
            else if (_currentZipline.ZiplineAngle > 75f)
            {
                ziplineAngle = 150f;
            }
            else ziplineAngle = _currentZipline.ZiplineAngle * 2;
            _accelerationFactor = (ziplineAngle - 10f) / (150f - 10f) * 15f * 2;
            _maxZiplineSpeed = (ziplineAngle - 10f) / (150f - 10f) * 25f;
            if (_maxZiplineSpeed < 10f)
            {
                _maxZiplineSpeed = 10f;
            }
        }

        private void PlayerController_JumpPerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_ziplining == true)
            {
                LeaveZipline(_currentZipline);
            }
        }
    }
}