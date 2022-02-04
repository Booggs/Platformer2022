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
    public class PhysicsDash : MonoBehaviour
    {
        [SerializeField]
        private CubeController _cubeController = null;

        [SerializeField]
        private PlayerController _playerController = null;

        [SerializeField]
        private float _force = 30f;

        [SerializeField]
        private float _groundDashMultiplier = 1.2f;

        [SerializeField]
        private Timer _dashTimeLimit = null;

        [SerializeField]
        private CubeController.State _usableInState = CubeController.State.None;

        [SerializeField]
        private DamageHandler _damageHandler = null;

        private bool _isDashing = false;
        private PlayerReferences playerRefs = null;

        public bool Dashing => _isDashing;

        private void Awake()
        {
            playerRefs = GetComponent<PlayerReferences>();
        }

        private void OnEnable()
        {
            _playerController.DashPerformed -= PlayerControllerOnDashPerformed;
            _playerController.DashPerformed += PlayerControllerOnDashPerformed;
        }

        private void OnDisable()
        {
            _playerController.DashPerformed -= PlayerControllerOnDashPerformed;
        }

        private void PlayerControllerOnDashPerformed(PlayerController sender, UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (_usableInState.HasFlag(_cubeController.CurrentState) && _isDashing == false && (_cubeController.Rigidbody.velocity.z >= 3f || _cubeController.Rigidbody.velocity.z <= -3f))
            {
                float dashDirection = Mathf.Sign(_playerController.HorizontalMove);
                _isDashing = true;
                _cubeController.ResetRigidbodiesVelocity();

                if (playerRefs.TryGetCubeController(out CubeController cubeController) == true)
                {
                    if (cubeController.CurrentState == CubeController.State.Grounded)
                    {
                        foreach (var rigidbody in cubeController.Rigidbodies)
                        {
                            rigidbody.AddForce(new Vector3(0f, 5f, _force * dashDirection * _groundDashMultiplier), ForceMode.Impulse);
                        }
                        cubeController.ChangeState(CubeController.State.Jumping);
                    }
                    else
                    {
                        foreach (var rigidbody in cubeController.Rigidbodies)
                        {
                            rigidbody.AddForce(new Vector3(0f, 0, _force * dashDirection), ForceMode.Impulse);
                        }
                    }
                }
                _damageHandler.UpdateState(DamageHandler.DamageStates.Dashing);
                _cubeController.enabled = false;
                _dashTimeLimit.Start();
            }
        }

        private void Update()
        {
            if (_dashTimeLimit.IsRunning == true)
            {
                _dashTimeLimit.Update();
            }
            else
            {
                if (_isDashing == true)
                {
                    _cubeController.ChangeState(CubeController.State.Jumping);
                    _cubeController.ForceCheckGround();
                    if ((_cubeController.CurrentState != CubeController.State.Jumping == true) || _dashTimeLimit.IsRunning == false)
                    {
                        _cubeController.enabled = true;
                        _isDashing = false;
                        _damageHandler.UpdateState(DamageHandler.DamageStates.None);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (_isDashing == true)
            {
                foreach (var rigidbody in _cubeController.Rigidbodies)
                {
                    rigidbody.velocity = rigidbody.velocity * 0.95f;
                }
            }
        }

        private void OnValidate()
        {
            _force = Mathf.Clamp(_force, 10f, float.MaxValue);
        }
    }
}