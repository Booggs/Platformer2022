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
	public class ExampleUpDash : MonoBehaviour
	{
		[SerializeField]
		private PlayerReferences _playerReferences = null;

		private Rigidbody _rigidbody = null;
		private CubeController _cubeController = null;
		private PlayerController _playerController = null;
		private CharacterCollision _characterCollision = null;

		[SerializeField]
		private float _moveSpeed = 2f;

		[SerializeField]
		private float _force = 50f;

		//[SerializeField]
		//private float _airFriction = 5f;

		[SerializeField]
		private float _ascendingGravityScale = 5f;

		[SerializeField]
		private float _descendingGravityScale = 5f;


		[SerializeField]
		private Timer _enableControlsAfterTimer = null;

		[SerializeField]
		private CubeController.State _usableInState = CubeController.State.None;

		private bool _isOnGroundSmash = false;

		private void Awake()
		{
			_playerReferences.TryGetRigidbody(out _rigidbody);
			_playerReferences.TryGetCubeController(out _cubeController);
			_playerReferences.TryGetPlayerController(out _playerController);
			_playerReferences.TryGetCharacterCollision(out _characterCollision);

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
			if (_usableInState.HasFlag(_cubeController.CurrentState) && _isOnGroundSmash == false)
			{
				_isOnGroundSmash = true;
				// TODO AL : maybe reset vel to 0 before applying the bump

				Vector3 force = new Vector3
				(
					0f,
					Mathf.Sqrt(2 * (_force * _ascendingGravityScale) * Mathf.Abs(Physics.gravity.y)),
					0f
				);

				_rigidbody.AddForce(force, ForceMode.Impulse);
				_cubeController.ChangeState(CubeController.State.None);
				_cubeController.enabled = false;
				_enableControlsAfterTimer.Start();
			}
		}

		private bool _enabledFirstTime = false;

		private void Update()
		{
			if (_isOnGroundSmash == true)
			{
				if (_enableControlsAfterTimer.Update() == true)
				{
					if (_enabledFirstTime == false)
					{
						_cubeController.ChangeState(CubeController.State.Falling);
						_enabledFirstTime = true;
					}

					_cubeController.ForceCheckGround();
					if (_cubeController.CurrentState != CubeController.State.Falling)
					{
						StopUpDashMovement();
					}
				}

				float movementDirection = _playerController.HorizontalMove;
				if (movementDirection != 0)
				{
					movementDirection = movementDirection > 0 ? 1 : -1;
				}
				_characterCollision.HandleWallCollisionAndApplyBonusYReplacement((int)movementDirection);

				if (_characterCollision.HasAWallInFrontOfCharacter == true)
				{
					StopUpDashMovement();
				}

				Vector3 velocity = _rigidbody.velocity;
				velocity.z = movementDirection * _moveSpeed;

				float gravityScale = (velocity.y > 0 ? _ascendingGravityScale : _descendingGravityScale);
				velocity.y += Physics.gravity.y * gravityScale * Time.deltaTime;

				_rigidbody.velocity = velocity;
			}
		}

		private void StopUpDashMovement()
		{
			if (_cubeController.CurrentState == CubeController.State.None)
			{
				_cubeController.ChangeState(CubeController.State.Falling);
				_cubeController.ForceCheckGround();
			}


			_cubeController.enabled = true;
			_isOnGroundSmash = false;
			_enabledFirstTime = false;
		}

		private void OnValidate()
		{
			//_force = Mathf.Clamp(_force, 10f, float.MaxValue);
		}
	}

}