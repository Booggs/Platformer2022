namespace GSGD2.Player
{
	using GSGD2.Utilities;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// This class is an example of how you can add movement behavior to the player without digging into <see cref="CubeController"/>.
	/// It can enable a mario like "ground smash" 
	/// </summary>
	public class AirGliding : MonoBehaviour
	{
		[SerializeField]
		private PlayerReferences _playerReferences = null;

		[SerializeField]
		private float _glidingSpeedMultiplier = 0.8f;

		[SerializeField]
		private float _glidingGravityDivider = 15f;

		[SerializeField]
		private CubeController.State _usableInState = CubeController.State.None;

		private float _defaultAirSpeed;
		private float _defaultDescendingGravityScale;

		private CubeController _cubeController = null;
		private PlayerController _playerController = null;
		private CharacterCollision _characterCollision = null;

		private bool _isGliding = false;

        private void Awake()
        {
			_playerReferences.TryGetCubeController(out _cubeController);
			_playerReferences.TryGetPlayerController(out _playerController);
			_playerReferences.TryGetCharacterCollision(out _characterCollision);
		}

        private void OnEnable()
		{
			_playerController.GliderPerformed -= PlayerControllerGliderPerformed;
			_playerController.GliderPerformed += PlayerControllerGliderPerformed;
			_playerController.ReleaseGliderPerformed -= PlayerControllerReleaseGliderPerformed;
			_playerController.ReleaseGliderPerformed += PlayerControllerReleaseGliderPerformed;
		}

		private void OnDisable()
		{
			_playerController.GliderPerformed -= PlayerControllerGliderPerformed;
			_playerController.ReleaseGliderPerformed -= PlayerControllerReleaseGliderPerformed;
		}

		private void PlayerControllerGliderPerformed(PlayerController sender, UnityEngine.InputSystem.InputAction.CallbackContext obj)
		{
			if (_usableInState.HasFlag(_cubeController.CurrentState) && _isGliding == false)
			{
				_defaultAirSpeed = _cubeController.AirMoveSpeed;
				_defaultDescendingGravityScale = _cubeController.DescendingGravityScale;

				_isGliding = true;
				_cubeController.AirMoveSpeed = _defaultAirSpeed * _glidingSpeedMultiplier;
				_cubeController.DescendingGravityScale = _defaultDescendingGravityScale / _glidingGravityDivider;
			}
		}

		private void PlayerControllerReleaseGliderPerformed(PlayerController sender, UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
			if (_isGliding == true)
            {
				StopGliding();
            }
        }
			

		private void Update()
		{
			if (_isGliding == true)
			{
				_cubeController.ForceCheckGround();
				if (!_usableInState.HasFlag(_cubeController.CurrentState))
				{
					StopGliding();
				}
				if (_characterCollision.HasAWallInFrontOfCharacter == true)
				{
					StopGliding();
				}
			}
		}

		private void StopGliding()
        {
			_cubeController.enabled = true;
			_isGliding = false;
			_cubeController.AirMoveSpeed = _defaultAirSpeed;
			_cubeController.DescendingGravityScale = _defaultDescendingGravityScale;
		}
	}
}