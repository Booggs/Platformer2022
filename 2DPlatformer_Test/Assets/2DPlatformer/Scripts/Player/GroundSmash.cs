namespace GSGD2.Player
{
    using GSGD2.Utilities;
    using GSGD2.Gameplay;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// This class is an example of how you can add movement behavior to the player without digging into <see cref="CubeController"/>.
    /// It can enable a mario like "ground smash" 
    /// </summary>
    public class GroundSmash : MonoBehaviour
    {
        [SerializeField]
        private CubeController _cubeController = null;

        [SerializeField]
        private PlayerController _playerController = null;

        [SerializeField]
        private float _force = 50f;

        [SerializeField]
        private float _staminaCost = 30f;

        [SerializeField]
        private Timer _enableControlsAfterTimer = null;

        [SerializeField]
        private CubeController.State _usableInState = CubeController.State.None;

        [SerializeField]
        private DamageHandler _damageHandler = null;

        private bool _isOnGroundSmash = false;
        private PlayerReferences playerRefs = null;

        public bool GroundSmashing => _isOnGroundSmash;

        private void Awake()
        {
            playerRefs = GetComponent<PlayerReferences>();
        }

        private void OnEnable()
        {
            _playerController.GroundSmashPerformed -= PlayerControllerOnGroundSmashPerformed;
            _playerController.GroundSmashPerformed += PlayerControllerOnGroundSmashPerformed;
        }

        private void OnDisable()
        {
            _playerController.GroundSmashPerformed -= PlayerControllerOnGroundSmashPerformed;
        }

        private void PlayerControllerOnGroundSmashPerformed(PlayerController sender, UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (_usableInState.HasFlag(_cubeController.CurrentState) && _isOnGroundSmash == false && _cubeController.UseStamina(_staminaCost / 100f))
            {
                _damageHandler.UpdateState(DamageHandler.DamageStates.GroundSmashing);
                _isOnGroundSmash = true;
                _cubeController.ResetRigidbodiesVelocity();
                // TODO AL : maybe reset vel to 0 before applying the bump
                foreach (var rigidbody in _cubeController.Rigidbodies)
                {
                    rigidbody.AddForce(new Vector3(0f, _force * -1, 0f), ForceMode.Impulse);
                }
                _cubeController.enabled = false;
                _enableControlsAfterTimer.Start();
            }
        }

        private void Update()
        {
            if (_isOnGroundSmash == true)
            {
                _cubeController.ForceCheckGround();
                if (_cubeController.CurrentState != CubeController.State.Jumping && _enableControlsAfterTimer.Update() == true)
                {
                    _cubeController.enabled = true;
                    _cubeController.StartStaminaRegen();
                    _isOnGroundSmash = false;
                    _damageHandler.UpdateState(DamageHandler.DamageStates.None);
                }
            }
        }

        private void OnValidate()
        {
            _force = Mathf.Clamp(_force, 10f, float.MaxValue);
        }
    }
}