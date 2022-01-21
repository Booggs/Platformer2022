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

        private Zipline _currentZipline = null;
        private bool _ziplining = false;
        private bool _zipliningLeft = false;
        private Vector3 _zipliningDestination;
        private PlayerController _playerController = null;
        private CubeController _cubeController = null;
        private Rigidbody _rigidbody = null;
        private float _currentZiplineSpeed = 5f;

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
            _currentZipline = zipline;
            _ziplining = true;
            this._rigidbody.velocity = new Vector3(0, 0, 0);
            if ((transform.rotation.eulerAngles.y <= 90 && transform.rotation.eulerAngles.y >= -90) == true)
            {
                _zipliningLeft = false;
                _zipliningDestination = _currentZipline.ZiplineTarget.position;
            }
            else
            {
                _zipliningLeft = true;
                _zipliningDestination = _currentZipline.ZiplineStart.position;
            }
            _cubeController.ChangeState(CubeController.State.None);
            //_cubeController.enabled = false;
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
            }
        }

        private void Update()
        {
            if (_ziplining == true && _currentZipline != null)
            {
                if (Vector3.Distance(transform.position, _zipliningDestination) > 0.5f)
                {
                    transform.position += (_zipliningDestination - transform.position).normalized * _startingZiplineSpeed * Time.deltaTime;
                    print(Vector3.Distance(transform.position, _zipliningDestination).ToString());
                }
                else 
                {
                    LeaveZipline(_currentZipline);
                }
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