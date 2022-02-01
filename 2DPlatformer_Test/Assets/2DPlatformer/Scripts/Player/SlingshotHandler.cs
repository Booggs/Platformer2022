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

        [Header("References")]
        [SerializeField]
        private PlayerReferences _playerRefs = null;

        private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
        private CubeController _cubeController = null;
        private PlayerController _playerController = null;
        private CameraAimController _aimController = null;

        private void Awake()
        {
            _playerRefs.TryGetCubeController(out _cubeController);
            _playerRefs.TryGetPlayerController(out _playerController);
            _playerRefs.TryGetCameraAimController(out _aimController);
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

        private void ChargeSlingshot(PlayerController sender, InputAction.CallbackContext obj)
        {

        }

        private void ReleaseSlingshot(PlayerController sender, InputAction.CallbackContext obj)
        {

        }
    }
}