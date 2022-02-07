namespace GSGD2.Player
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.InputSystem;

	/// <summary>
	/// Component that bind InputSystem InputActionMap to events and properties to be used in game. Disabling the component disable the input listening.
	/// </summary>
	public class PlayerController : MonoBehaviour
	{
		private const string HORIZONTAL_MOVE_ACTION_NAME = "HorizontalMove";
		private const string HORIZONTAL_LOOK_ACTION_NAME = "HorizontalLook";
		private const string VERTICAL_LOOK_ACTION_NAME = "VerticalLook";
		private const string JUMP_ACTION_NAME = "Jump";
		private const string DASH_ACTION_NAME = "Dash";
		private const string WALL_GRAB_ACTION_NAME = "WallGrab";
		private const string RELEASE_WALL_GRAB_ACTION_NAME = "ReleaseWallGrab";
		private const string WALL_JUMP_ACTION_NAME = "WallJump";
		private const string TAKE_ITEM_ACTION_NAME = "TakeItem";
		private const string RELEASE_ITEM_ACTION_NAME = "ReleaseItem";
		private const string USE_ITEM_ACTION_NAME = "UseItem";
		private const string GROUND_SMASH_ACTION_NAME = "GroundSmash";
		private const string USE_INTERACTABLE_ACTION_NAME = "UseInteractable";
		private const string LEAVE_INTERACTABLE_ACTION_NAME = "LeaveInteractable"; 
		private const string NAVIGATE_LEFT_INTERACTABLE_ACTION_NAME = "NavigateLeftInteractable";
		private const string NAVIGATE_RIGHT_INTERACTABLE_ACTION_NAME = "NavigateRightInteractable";
		private const string GLIDER_ACTION_NAME = "Glider";
		private const string RELEASE_GLIDER_ACTION_NAME = "ReleaseGlider";
		private const string RESET_PLAYER_ACTION_NAME = "ResetPlayer";
		private const string START_TIMER_ACTION_NAME = "StartTestTimer";
		private const string BLOB_SEPARATION_ACTION_NAME = "BlobSeparation";
		private const string SHOOT_ACTION_NAME = "Shoot";
		private const string CHARGE_SLINGSHOT_ACTION_NAME = "ChargeSlingshot";
		private const string RELEASE_SLINGSHOT_ACTION_NAME = "ReleaseSlingshot";
		private const string PAUSE_MENU_ACTION_NAME = "PauseMenu";

		[SerializeField]
		private InputActionMapWrapper _inputActionMapWrapper;

		[SerializeField]
		private bool _useMouseForLookDirection = false;

		private InputAction _horizontalMoveInputAction = null;
		private InputAction _horizontalLookInputAction = null;
		private InputAction _verticalLookInputAction = null;
		private InputAction _jumpInputAction = null;
		private InputAction _dashInputAction = null;
		private InputAction _wallGrabInputAction = null;
		private InputAction _releaseWallGrabInputAction = null;
		private InputAction _wallJumpInputAction = null;
		private InputAction _takeItemInputAction = null;
		private InputAction _releaseItemInputAction = null;
		private InputAction _useItemInputAction = null;
		private InputAction _groundSmashInputAction = null;
		private InputAction _useInteractableInputAction = null;
		private InputAction _leaveInteractableInputAction = null;
		private InputAction _navigateLeftInteractableInputAction = null;
		private InputAction _navigateRightInteractableInputAction = null;
		private InputAction _gliderInputAction = null;
		private InputAction _releaseGliderInputAction = null;
		private InputAction _resetPlayerInputAction = null;
		private InputAction _startTimerInputAction = null;
		private InputAction _blobSeparationInputAction = null;
		private InputAction _shootInputAction = null;
		private InputAction _chargeSlingshotInputAction = null;
		private InputAction _releaseSlingshotInputAction = null;
		private InputAction _pauseMenuInputAction = null;

		public bool UseMouseForLookDirection => _useMouseForLookDirection;
		public float HorizontalMove => _horizontalMoveInputAction.ReadValue<float>();
		public float HorizontalLook => _horizontalLookInputAction.ReadValue<float>();
		public float VerticalLook => _verticalLookInputAction.ReadValue<float>();
		public Vector3 LookDirection => new Vector3(0f, VerticalLook, HorizontalLook);
		public bool IsJumpButtonPressed => _jumpInputAction.IsPressed();

		public delegate void InputEvent(PlayerController sender, InputAction.CallbackContext obj);
		public event InputEvent JumpPerformed = null;
		public event InputEvent DashPerformed = null;
		public event InputEvent WallGrabPerformed = null;
		public event InputEvent ReleaseWallGrabPerformed = null;
		public event InputEvent WallJumpPerformed = null;
		public event InputEvent TakeItemPerformed = null;
		public event InputEvent ReleaseItemPerformed = null;
		public event InputEvent UseItemPerformed = null;
		public event InputEvent GroundSmashPerformed = null;
		public event InputEvent UseInteractablePerformed = null;
		public event InputEvent LeaveInteractablePerformed = null;
		public event InputEvent NavigateLeftInteractablePerformed = null;
		public event InputEvent NavigateRightInteractablePerformed = null;
		public event InputEvent GliderPerformed = null;
		public event InputEvent ReleaseGliderPerformed = null;
		public event InputEvent ResetPlayerPerformed = null;
		public event InputEvent StartTimerPerformed = null;
		public event InputEvent BlobSeparationPerformed = null;
		public event InputEvent ShootPerformed = null;
		public event InputEvent ChargeSlingshotPerformed = null;
		public event InputEvent ReleaseSlingshotPerformed = null;
		public event InputEvent PauseMenuPerformed = null;

		private void OnEnable()
		{
			_inputActionMapWrapper.TryFindAction(HORIZONTAL_MOVE_ACTION_NAME, out _horizontalMoveInputAction, true);

			_inputActionMapWrapper.TryFindAction(HORIZONTAL_LOOK_ACTION_NAME, out _horizontalLookInputAction, true);
			_inputActionMapWrapper.TryFindAction(VERTICAL_LOOK_ACTION_NAME, out _verticalLookInputAction, true);

			if (_inputActionMapWrapper.TryFindAction(JUMP_ACTION_NAME, out _jumpInputAction, true) == true)
			{
				_jumpInputAction.performed -= JumpInputAction_performed;
				_jumpInputAction.performed += JumpInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(DASH_ACTION_NAME, out _dashInputAction, true) == true)
			{
				_dashInputAction.performed -= DashInputAction_performed;
				_dashInputAction.performed += DashInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(WALL_GRAB_ACTION_NAME, out _wallGrabInputAction, true) == true)
			{
				_wallGrabInputAction.performed -= WallGrabMoveInputAction_performed;
				_wallGrabInputAction.performed += WallGrabMoveInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(RELEASE_WALL_GRAB_ACTION_NAME, out _releaseWallGrabInputAction, true) == true)
			{
				_releaseWallGrabInputAction.performed -= ReleaseWallGrabMoveInputAction_performed;
				_releaseWallGrabInputAction.performed += ReleaseWallGrabMoveInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(WALL_JUMP_ACTION_NAME, out _wallJumpInputAction, true) == true)
			{
				_wallJumpInputAction.performed -= WallJumpMoveInputAction_performed;
				_wallJumpInputAction.performed += WallJumpMoveInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(TAKE_ITEM_ACTION_NAME, out _takeItemInputAction, true) == true)
			{
				_takeItemInputAction.performed -= TakeItemInputAction_performed;
				_takeItemInputAction.performed += TakeItemInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(RELEASE_ITEM_ACTION_NAME, out _releaseItemInputAction, true) == true)
			{
				_releaseItemInputAction.performed -= ReleaseItemInputAction_performed;
				_releaseItemInputAction.performed += ReleaseItemInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(USE_ITEM_ACTION_NAME, out _useItemInputAction, true) == true)
			{
				_useItemInputAction.performed -= UseItemInputAction_performed;
				_useItemInputAction.performed += UseItemInputAction_performed;
			}
			
			if (_inputActionMapWrapper.TryFindAction(GROUND_SMASH_ACTION_NAME, out _groundSmashInputAction, true) == true)
			{
				_groundSmashInputAction.performed -= GroundSmashInputAction_performed;
				_groundSmashInputAction.performed += GroundSmashInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(USE_INTERACTABLE_ACTION_NAME, out _useInteractableInputAction, true) == true)
			{
				_useInteractableInputAction.performed -= UseInteractableInputAction_performed;
				_useInteractableInputAction.performed += UseInteractableInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(LEAVE_INTERACTABLE_ACTION_NAME, out _leaveInteractableInputAction, true) == true)
			{
				_leaveInteractableInputAction.performed -= LeaveInteractableInputAction_performed;
				_leaveInteractableInputAction.performed += LeaveInteractableInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(NAVIGATE_LEFT_INTERACTABLE_ACTION_NAME, out _navigateLeftInteractableInputAction, true) == true)
			{
				_navigateLeftInteractableInputAction.performed -= NavigateLeftInteractableInputAction_performed;
				_navigateLeftInteractableInputAction.performed += NavigateLeftInteractableInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(NAVIGATE_RIGHT_INTERACTABLE_ACTION_NAME, out _navigateRightInteractableInputAction, true) == true)
			{
				_navigateRightInteractableInputAction.performed -= NavigateRightInteractableInputAction_performed;
				_navigateRightInteractableInputAction.performed += NavigateRightInteractableInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(GLIDER_ACTION_NAME, out _gliderInputAction, true) == true)
			{
				_gliderInputAction.performed -= GliderInputAction_performed;
				_gliderInputAction.performed += GliderInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(RELEASE_GLIDER_ACTION_NAME, out _releaseGliderInputAction, true) == true)
			{
				_releaseGliderInputAction.performed -= ReleaseGliderInputAction_performed;
				_releaseGliderInputAction.performed += ReleaseGliderInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(RESET_PLAYER_ACTION_NAME, out _resetPlayerInputAction, true) == true)
			{
				_resetPlayerInputAction.performed -= ResetPlayerInputAction_performed;
				_resetPlayerInputAction.performed += ResetPlayerInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(START_TIMER_ACTION_NAME, out _startTimerInputAction, true) == true)
			{
				_startTimerInputAction.performed -= StartTimerInputAction_performed;
				_startTimerInputAction.performed += StartTimerInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(BLOB_SEPARATION_ACTION_NAME, out _blobSeparationInputAction, true) == true)
			{
				_blobSeparationInputAction.performed -= BlobSeparationInputAction_performed;
				_blobSeparationInputAction.performed += BlobSeparationInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(SHOOT_ACTION_NAME, out _shootInputAction, true) == true)
			{
				_shootInputAction.performed -= ShootInputAction_performed;
				_shootInputAction.performed += ShootInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(CHARGE_SLINGSHOT_ACTION_NAME, out _chargeSlingshotInputAction, true) == true)
			{
				_chargeSlingshotInputAction.performed -= ChargeSlingshotInputAction_performed;
				_chargeSlingshotInputAction.performed += ChargeSlingshotInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(RELEASE_SLINGSHOT_ACTION_NAME, out _releaseSlingshotInputAction, true) == true)
			{
				_releaseSlingshotInputAction.performed -= ReleaseSlingshotInputAction_performed;
				_releaseSlingshotInputAction.performed += ReleaseSlingshotInputAction_performed;
			}

			if (_inputActionMapWrapper.TryFindAction(PAUSE_MENU_ACTION_NAME, out _pauseMenuInputAction, true) == true)
			{
				_pauseMenuInputAction.performed -= PauseMenuInputAction_performed;
				_pauseMenuInputAction.performed += PauseMenuInputAction_performed;
			}
		}

		private void OnDisable()
		{
			_horizontalMoveInputAction.Disable();
			_jumpInputAction.Disable();
			_dashInputAction.Disable();
			_wallGrabInputAction.Disable();
			_releaseWallGrabInputAction.Disable();
			_wallJumpInputAction.Disable();
			_takeItemInputAction.Disable();
			_releaseItemInputAction.Disable();
			_horizontalLookInputAction.Disable();
			_verticalLookInputAction.Disable();
			_verticalLookInputAction.Disable();
			_useItemInputAction.Disable();
			_groundSmashInputAction.Disable();
			_useInteractableInputAction.Disable();
			_leaveInteractableInputAction.Disable();
			_navigateLeftInteractableInputAction.Disable();
			_navigateRightInteractableInputAction.Disable();
			_gliderInputAction.Disable();
			_releaseGliderInputAction.Disable();
			_resetPlayerInputAction.Disable();
			_startTimerInputAction.Disable();
			_blobSeparationInputAction.Disable();
			_shootInputAction.Disable();
			_chargeSlingshotInputAction.Disable();
			_releaseSlingshotInputAction.Disable();
			_pauseMenuInputAction.Disable();

			_jumpInputAction.performed -= JumpInputAction_performed;
			_dashInputAction.performed -= DashInputAction_performed;
			_wallGrabInputAction.performed -= WallGrabMoveInputAction_performed;
			_releaseWallGrabInputAction.performed -= ReleaseWallGrabMoveInputAction_performed;
			_wallJumpInputAction.performed -= WallJumpMoveInputAction_performed;
			_takeItemInputAction.performed -= TakeItemInputAction_performed;
			_releaseItemInputAction.performed -= ReleaseItemInputAction_performed;
			_useItemInputAction.performed -= UseItemInputAction_performed;
			_groundSmashInputAction.performed -= GroundSmashInputAction_performed;
			_useInteractableInputAction.performed -= UseInteractableInputAction_performed;
			_leaveInteractableInputAction.performed -= LeaveInteractableInputAction_performed;
			_navigateLeftInteractableInputAction.performed -= NavigateLeftInteractableInputAction_performed;
			_navigateRightInteractableInputAction.performed -= NavigateRightInteractableInputAction_performed;
			_gliderInputAction.performed -= GliderInputAction_performed;
			_releaseGliderInputAction.performed -= ReleaseGliderInputAction_performed;
			_resetPlayerInputAction.performed -= ResetPlayerInputAction_performed;
			_startTimerInputAction.performed -= StartTimerInputAction_performed;
			_blobSeparationInputAction.performed -= BlobSeparationInputAction_performed;
			_shootInputAction.performed -= ShootInputAction_performed;
			_chargeSlingshotInputAction.performed -= ChargeSlingshotInputAction_performed;
			_releaseSlingshotInputAction.performed -= ReleaseSlingshotInputAction_performed;
			_pauseMenuInputAction.performed -= PauseMenuInputAction_performed;
		}

		private void JumpInputAction_performed(InputAction.CallbackContext obj)
		{
			JumpPerformed?.Invoke(this, obj);
		}

		private void DashInputAction_performed(InputAction.CallbackContext obj)
		{
			DashPerformed?.Invoke(this, obj);
		}

		private void WallGrabMoveInputAction_performed(InputAction.CallbackContext obj)
		{
			WallGrabPerformed?.Invoke(this, obj);
		}

		private void ReleaseWallGrabMoveInputAction_performed(InputAction.CallbackContext obj)
		{
			ReleaseWallGrabPerformed?.Invoke(this, obj);
		}

		private void WallJumpMoveInputAction_performed(InputAction.CallbackContext obj)
		{
			WallJumpPerformed?.Invoke(this, obj);
		}

		private void TakeItemInputAction_performed(InputAction.CallbackContext obj)
		{
			TakeItemPerformed?.Invoke(this, obj);
		}

		private void ReleaseItemInputAction_performed(InputAction.CallbackContext obj)
		{
			ReleaseItemPerformed?.Invoke(this, obj);
		}
		private void UseItemInputAction_performed(InputAction.CallbackContext obj)
		{
			UseItemPerformed?.Invoke(this, obj);
		}

		private void GroundSmashInputAction_performed(InputAction.CallbackContext obj)
		{
			GroundSmashPerformed?.Invoke(this, obj);
		}

		private void UseInteractableInputAction_performed(InputAction.CallbackContext obj)
		{
			UseInteractablePerformed?.Invoke(this, obj);
		}

		private void LeaveInteractableInputAction_performed(InputAction.CallbackContext obj)
        {
			LeaveInteractablePerformed?.Invoke(this, obj);
        }

		private void NavigateLeftInteractableInputAction_performed(InputAction.CallbackContext obj)
        {
			NavigateLeftInteractablePerformed?.Invoke(this, obj);
        }

		private void NavigateRightInteractableInputAction_performed(InputAction.CallbackContext obj)
		{
			NavigateRightInteractablePerformed?.Invoke(this, obj);
		}

		private void GliderInputAction_performed(InputAction.CallbackContext obj)
        {
			GliderPerformed?.Invoke(this, obj);
        }

		private void ReleaseGliderInputAction_performed(InputAction.CallbackContext obj)
		{
			ReleaseGliderPerformed?.Invoke(this, obj);
		}

		private void ResetPlayerInputAction_performed(InputAction.CallbackContext obj)
        {
			ResetPlayerPerformed?.Invoke(this, obj);
        }

		private void StartTimerInputAction_performed(InputAction.CallbackContext obj)
		{
			StartTimerPerformed?.Invoke(this, obj);
		}

		private void BlobSeparationInputAction_performed(InputAction.CallbackContext obj)
		{
			BlobSeparationPerformed?.Invoke(this, obj);
		}

		private void ShootInputAction_performed(InputAction.CallbackContext obj)
		{
			ShootPerformed?.Invoke(this, obj);
		}

		private void ChargeSlingshotInputAction_performed(InputAction.CallbackContext obj)
		{
			ChargeSlingshotPerformed?.Invoke(this, obj);
		}

		private void ReleaseSlingshotInputAction_performed(InputAction.CallbackContext obj)
		{
			ReleaseSlingshotPerformed?.Invoke(this, obj);
		}

		private void PauseMenuInputAction_performed(InputAction.CallbackContext obj)
        {
			PauseMenuPerformed?.Invoke(this, obj);
        }
	}
}