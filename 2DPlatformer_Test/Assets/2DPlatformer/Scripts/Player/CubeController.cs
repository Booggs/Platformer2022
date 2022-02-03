namespace GSGD2.Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Utilities;
    using GSGD2.Gameplay;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Class that handle a physics-based character.
    /// </summary>
    [RequireComponent(typeof(DisplacementEstimationUpdater))]
    [RequireComponent(typeof(PlayerController))]
    public class CubeController : MonoBehaviour, IBumperReceiverListener
    {
        #region Enums
        [System.Flags]
        public enum State
        {
            None = 0,
            Grounded = 1 << 0,
            Falling = 1 << 1,
            Bumping = 1 << 2,
            StartJump = 1 << 3, // Only called one frame, then move to Jumping
            Jumping = 1 << 4,
            EndJump = 1 << 5, // Only called one frame, then move to Grounded or Falling
            WallGrab = 1 << 6,
            WallJump = 1 << 7,
            Dashing = 1 << 8,
            DamageTaken = 1 << 9,

            Everything = ~(None)
        }

        private const string LOG_TOKEN = "[{0}] {1}";
        #endregion Enums

        #region Fields
        [Header("References")]
        [SerializeField]
        private PlayerReferences _playerReferences = null;


        [Header("Movement")]
        [SerializeField]
        private float _maxVelocity = 50f;

        /// <summary>
        /// Speed applied to character velocity in the direction of <see cref="InputMovement"/> while in <see cref="State.Grounded"/>
        /// </summary>
        [SerializeField]
        private float _groundMoveSpeed = 1f;

        /// <summary>
        /// Modifier applied when releasing <see cref="InputMovement"/> while on the ground.
        /// </summary>
        [SerializeField]
        private float _groundFriction = 1f;

        private float _defaultGroundFriction = 1f;

        /// <summary>
        /// Speed applied to character velocity in the direction of <see cref="InputMovement"/> while in air.
        /// </summary>
        [SerializeField]
        private float _airMoveSpeed = 1f;

        /// <summary>
        /// Modifier multiplied to the gravity when the character movement is ascending
        /// </summary>
        [SerializeField]
        private float _ascendingGravityScale = 5f;

        /// <summary>
        /// Modifier multiplied to the gravity when the character movement is descending
        /// </summary>
        [SerializeField]
        private float _descendingGravityScale = 5f;

        [SerializeField]
        private float _glidingSpeedDivider = 1.05f;


        [Header("Jump")]
        [SerializeField]
        private Jump _jump = null;

        [SerializeField]
        private bool _changeToFallingStateWhenReleasingJump = false;

        [SerializeField]
        private bool _canJumpDuringDash = true;

        [SerializeField]
        private float _durationToDisableGroundRaycastWhenJumping = 0.5f;

        [SerializeField]
        private bool _resetJumpCountWhenFalling = true;

        [SerializeField]
        private int _allowedJumpCountWhenFalling = 1;

        [SerializeField]
        private bool _resetJumpCountWhenWallJumping = true;

        [SerializeField]
        private int _allowedJumpCountWhenWallJumping = 1;

        [SerializeField]
        private bool _resetJumpCountWhenBumping = true;

        [SerializeField]
        private int _allowedJumpCountWhenBumping = 1;

        private float _defaultJumpHeight = 40f;

        private Dash _dash = null;

        private float _durationToDisableGroundRaycastWhenDashing = 0.5f;

        /*[SerializeField]
		private bool _resetDashCountWhenFalling = true;*/

        private int _allowedDashCountWhenFalling = 1;

        private float _wallRaycastDistanceWhenDashing = 10f;

        private bool _resetDashCountWhenWallJumping = true;

        private int _allowedDashCountWhenWallJumping = 1;

        private bool _resetDashJumpCountWhenBumping = true;

        private int _allowedDashCountWhenBumping = 1;

        private bool _gliding = false;


        [Header("Wall Grab / Jump")]
        [SerializeField]
        private bool _enableWallGrab = true;

        [SerializeField]
        private Timer _stickyModeDuration;

        [SerializeField]
        private float _wallJumpHeight = 3f;

        [SerializeField]
        private float _completeStaminaRegenDuration = 7.5f;

        [Header("Don't touch")]
        [SerializeField]
        private Timer _staminaRegenTimer;


        private bool _stickyModeOn = false;
        private bool _stickingToWall = false;

        /// <summary>
        /// Height applied to the jump force when releasing the wall jump button
        /// </summary>

        /// <summary>
        /// Force applied to the normal vector of the wall (horizontally) to send away the character from the wall
        /// </summary>
        private int _wallJumpWallNormalForce = 8;

        /// <summary>
        /// Delay at which the character cannot grab a wall with the same normal direction after a previous wall jump.
        /// It prevent from "bunny hopping" a wall, while preserving grabbing the opposite wall.
        /// </summary>
        private float _wallGrabDisableDuration = 0.25f;

        private bool _resetWallGrabDisableDurationWhenDashing = true;

        [Header("Bump")]
        [SerializeField]
        private float _durationToDisableControlDuringBump = 0.5f;

        /// <summary>
        /// Speed at which the character mesh parent will be rotated after a change of <see cref="InputMovement"/>
        /// </summary>
        /// <see cref="_uturnCurve"/> to ease the movement.
        [Header("Rotation")]
        [SerializeField]
        private float _turnSpeed = 1f;

        [SerializeField]
        private AnimationCurve _uturnCurve = null;

        // TODO AL : move disable control when damage taken to damageable in order to reduce dependencies here
        [Header("Misc")]
        [SerializeField]
        private Timer _disableControlWhenDamageTaken = null;

        [SerializeField]
        private bool _debugMode = false;

        // Runtime fields
        private PlayerController _playerController = null;
        private Rigidbody _rootRigidbody = null;
        private Rigidbody[] _rigidbodies = null;
        private DisplacementEstimationUpdater _displacementEstimationUpdater = null;
        private CharacterCollision _characterCollision = null;
        private BoneSphere _boneSphere = null;
        private SlingshotHandler _slingshotHandler = null;
        private float _maxVelocitySqr = -1f;
        private float _inputMovement;
        private float _rawInputMovement;
        private State _previousState = State.Falling;
        private State _currentState = State.Falling;
        private float _currentTurnTime = 1f;
        private int _lastMovementDirection;
        private float _currentDurationToDisableGroundRaycastWhenJumping = 0f;
        private float _currentDurationToDisableGroundRaycastWhenDashing = 0f;
        private float _currentWallGrabDisableDuration = 0f;
        private float _currentDurationToDisableControlDuringBump = 0;
        private Vector3 _wallNormalDuringLastWallGrab;
        private bool _shouldChangeToFallingStateWhenReleasingJump = false;
        private bool _hasBeganToFallFromGroundedState = false;
        private float _currentAirSpeed = 0f;
        private float _currentDescendingGravityScale = 0f;
        private float _currentGrabbingStamina = 0f;
        private Collider[] _colliders = null; // TODO AL : move this to PlayerReferences
        private bool _stickingLeft = false;

        private bool _hasBeganToFallFromGroundedStateAndDidJump = false;
        private bool _hasBeganToFallFromGroundedStateAndDidDash = false;

        private bool _willPerformJump = false;
        private bool _willPerformDash = false;
        private bool _willPerformWallGrab = false;
        private bool _willPerformWallJump = false;

        public delegate TResult CanChangeStateFunc<out TResult>(State currentState, State newState);

        private List<CanChangeStateFunc<bool>> _canChangeToMovementStateList = new List<CanChangeStateFunc<bool>>();
        #endregion Fields

        #region Properties
        /// <summary>
        /// Player Rigidbody
        /// </summary>
        public Rigidbody Rigidbody => _rootRigidbody;

        public Rigidbody[] Rigidbodies => _rigidbodies;

        /// <summary>
        /// Player Collider
        /// </summary>
        public Collider[] Colliders => _colliders;

        /// <summary>
        /// Direction provided by <see cref="PlayerController"/>
        /// </summary>
        public float InputMovement => _inputMovement;

        /// <summary>
        /// Normalized direction rounded to the nearest integer (-1, 0, or 1)
        /// </summary>
        public float RawInputMovement => _rawInputMovement;

        /// <summary>
        /// Last movement direction even if the joystick as been released
        /// </summary>
        public int LastMovementDirection => _lastMovementDirection;

        public State CurrentState => _currentState;
        public State PreviousState => _previousState;
        public bool IsGrounded => _currentState == State.Grounded;
        public bool IsWallGrabDisabled => _currentWallGrabDisableDuration < _wallGrabDisableDuration;
        public bool IsBumpingDisableControls => _currentDurationToDisableControlDuringBump < _durationToDisableControlDuringBump;
        public bool HasAWallInFrontOfCharacter => _characterCollision.HasAWallInFrontOfCharacter;
        public bool HasAWallBehindCharacter => _characterCollision.HasAWallBehindCharacter;
        public bool HasASlopeInFrontOfOrBehindCharacter => _characterCollision.HasASlopeInFrontOfOrBehindCharacter;

        public int MaxJumpCount => _jump.MaximumAllowedForcesWhileInAir;

        public float CurrentStamina => _currentGrabbingStamina;

        public bool StickyModeOn => _stickyModeOn;

        public Jump Jump => _jump;


        public bool Gliding
        {
            get
            {
                return _gliding;
            }
            set
            {
                _gliding = value;
            }
        }

        public float AirMoveSpeed
        {
            get
            {
                return _currentAirSpeed;
            }
            set
            {
                _currentAirSpeed = value;
            }
        }

        public float DescendingGravityScale
        {
            get
            {
                return _currentDescendingGravityScale;
            }
            set
            {
                _currentDescendingGravityScale = value;
            }
        }

        #endregion Properties

        #region Events
        public struct CubeControllerEventArgs
        {
            public State previousState;
            public State currentState;

            public CubeControllerEventArgs(State previousState, State currentState)
            {
                this.previousState = previousState;
                this.currentState = currentState;
            }
        }

        public delegate void CubeControllerJumpEvent(CubeController cubeController, CubeControllerEventArgs args);

        /// <summary>
        /// Called as soon as ChangeState as been called, before any reset of runtime values in order to let listener use them
        /// </summary>
        public event CubeControllerJumpEvent StateChanged = null;
        #endregion Events

        #region Methods
        public void AddToCanChangeToMovementState(CanChangeStateFunc<bool> callback)
        {
            _canChangeToMovementStateList.Add(callback);
        }

        public void RemoveFromCanChangeToMovementState(CanChangeStateFunc<bool> callback)
        {
            _canChangeToMovementStateList.Add(callback);
        }

        /// <summary>
        /// Enable or disable jump ability
        /// </summary>
        /// <param name="isEnabled">Is ability should be enabled ?</param>
        public void EnableJump(bool isEnabled)
        {
            _jump.enabled = isEnabled;
        }

        /// <summary>
        /// Enable or disable dash ability
        /// </summary>
        /// <param name="isEnabled">Is ability should be enabled ?</param>
        public void EnableDash(bool isEnabled)
        {
            _dash.enabled = isEnabled;
        }

        public void EnableGrab(bool isEnabled)
        {
            _enableWallGrab = isEnabled;
            if (_staminaRegenTimer.IsRunning == false)
            {
                _staminaRegenTimer.Start(_completeStaminaRegenDuration);
                _staminaRegenTimer.TimeElapsed = (_currentGrabbingStamina * _completeStaminaRegenDuration);
            }
        }

        public void AddMaximumAllowedForceToJump(int toAdd)
        {
            _jump.AddMaximumAllowedForcesWhileInAir(toAdd);
        }

        public void AddMaximumAllowedForceToDash(int toAdd)
        {
            _dash.AddMaximumAllowedForcesWhileInAir(toAdd);
        }

        /// <summary>
        /// Set is whether the player collider is a trigger or not.
        /// </summary>
        /// <param name="isTrigger"></param>
        public void SetColliderTrigger(bool isTrigger)
        {
            foreach (var collider in _colliders)
                collider.isTrigger = isTrigger;
        }

        public void ForceCheckGround()
        {
            CheckGroundAndChangeStateAccordingly();
        }

        public void ResetRigidbodiesVelocity()
        {
            foreach (var rigidbody in _rigidbodies)
            {
                rigidbody.velocity = Vector3.zero;
            }
        }

        #region Interfaces
        void IBumperReceiverListener.OnBump()
        {
            ChangeState(State.Bumping, true);
        }
        #endregion Interfaces

        #region Callbacks
        private void PlayerStartOnPlayerPositionReset(PlayerStart sender, PlayerStart.PlayerStartEventArgs args)
        {
            ResetCurrentValues();
        }
        #endregion Callbacks

        private void Awake()
        {
            //_playerReferences.TryGetRigidbody(out _rigidbody);
            _playerReferences.TryGetPlayerController(out _playerController);
            _playerReferences.TryGetDisplacementEstimationUpdater(out _displacementEstimationUpdater);
            _playerReferences.TryGetCharacterCollision(out _characterCollision);
            _playerReferences.TryGetBoneSphere(out _boneSphere);
            _playerReferences.TryGetSlingshotHandler(out _slingshotHandler);

            _maxVelocitySqr = _maxVelocity * _maxVelocity;

            _defaultGroundFriction = _groundFriction;

            _jump.cubeController = this;
            _jump.displacementEstimationUpdater = _displacementEstimationUpdater;
            _defaultJumpHeight = _jump.JumpHeight;

            _currentAirSpeed = _airMoveSpeed;
            _currentDescendingGravityScale = _descendingGravityScale;

            _rootRigidbody = GetComponentInChildren<Rigidbody>();
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
            ResetCurrentValues();
        }

        private void OnEnable()
        {
            LevelReferences.Instance.PlayerStart.BeforePlayerPositionReset -= PlayerStartOnPlayerPositionReset;
            LevelReferences.Instance.PlayerStart.BeforePlayerPositionReset += PlayerStartOnPlayerPositionReset;
            // TODO AL retake, PlayerDamageable should listen to CubeController and change its state
            _colliders = GetComponentsInChildren<Collider>();
            BindInput(true);
            if (_enableWallGrab == true)
            {
                _staminaRegenTimer.Start(7.5f);
            }
        }

        private void OnDisable()
        {
            if (LevelReferences.HasInstance == true)
            {
                LevelReferences.Instance.PlayerStart.BeforePlayerPositionReset -= PlayerStartOnPlayerPositionReset;
            }
            BindInput(false);
            ResetRigidbodiesVelocity();
        }

        private void OnDestroy()
        {
            // TODO AL : lazy, redo this properly
            if (Gamepad.current != null)
            {
                Gamepad.current.SetMotorSpeeds(0f, 0f);
            }
        }

        #region Inputs binding
        private void BindInput(bool isActive)
        {
            _playerController.JumpPerformed -= PlayerController_JumpPerformed;
            _playerController.DashPerformed -= PlayerController_DashPerformed;
            _playerController.WallGrabPerformed -= PlayerController_WallGrabPerformed;
            _playerController.WallJumpPerformed -= PlayerController_WallJumpPerformed;
            if (isActive == true)
            {
                _playerController.JumpPerformed += PlayerController_JumpPerformed;
                _playerController.DashPerformed += PlayerController_DashPerformed;
                _playerController.WallGrabPerformed += PlayerController_WallGrabPerformed;
                _playerController.WallJumpPerformed += PlayerController_WallJumpPerformed;
            }
        }

        private void PlayerController_DashPerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            switch (_currentState)
            {
                case State.Grounded:
                case State.Falling:
                case State.Bumping:
                case State.Jumping:
                case State.WallJump:
                case State.Dashing:
                    _willPerformDash = true;
                    break;
                case State.StartJump:
                case State.EndJump:
                case State.WallGrab: // TODO AL check if we can enable dash from wallgrab
                case State.DamageTaken:
                default:
                    break;
            }
        }

        private void PlayerController_JumpPerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            switch (_currentState)
            {
                case State.Grounded:
                case State.Falling:
                case State.Bumping:
                case State.Jumping:
                case State.WallJump:
                case State.Dashing:
                    _willPerformJump = true;
                    break;
                case State.StartJump:
                case State.EndJump:
                case State.WallGrab:
                case State.DamageTaken:
                default:
                    break;
            }
        }

        private void PlayerController_WallGrabPerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            switch (_currentState)
            {
                case State.Grounded:
                case State.Falling:
                case State.Bumping:
                case State.Jumping:
                case State.WallJump:
                case State.Dashing:
                case State.StartJump:
                case State.EndJump:
                case State.WallGrab:
                case State.DamageTaken:
                default:
                    break;
            }
            TryWallGrab();
        }

        private void PlayerController_WallJumpPerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            switch (_currentState)
            {
                case State.WallGrab:
                    _willPerformWallJump = true;
                    break;
                case State.Grounded:
                case State.Falling:
                case State.Bumping:
                case State.StartJump:
                case State.Jumping:
                case State.EndJump:
                case State.WallJump:
                case State.Dashing:
                case State.DamageTaken:
                default:
                    break;
            }
        }
        #endregion Inputs binding

        private void ResetCurrentValues()
        {
            _currentDurationToDisableGroundRaycastWhenJumping = _durationToDisableGroundRaycastWhenJumping;
            _currentDurationToDisableGroundRaycastWhenDashing = _durationToDisableGroundRaycastWhenDashing;
            _currentDurationToDisableControlDuringBump = _durationToDisableControlDuringBump;
            _currentWallGrabDisableDuration = _wallGrabDisableDuration;
            _jump.ResetCurrentForceCount();

            _characterCollision.ResetCurrentValues();
            ResetInputs();
            _shouldChangeToFallingStateWhenReleasingJump = _changeToFallingStateWhenReleasingJump;

            _hasBeganToFallFromGroundedState = false;
            _hasBeganToFallFromGroundedStateAndDidJump = false;
            _hasBeganToFallFromGroundedStateAndDidDash = false;
        }

        private void ResetInputs()
        {
            _willPerformJump = false;
            _willPerformDash = false;
            _willPerformWallGrab = false;
            _willPerformWallJump = false;
        }

        private void Update()
        {
            UpdateStates();
            ResetInputs();
        }

        public bool CanChangeState(State newState)
        {
            if (_canChangeToMovementStateList != null)
            {
                bool canChangeState = true;

                for (int i = 0, length = _canChangeToMovementStateList.Count; i < length; i++)
                {
                    var callback = _canChangeToMovementStateList[i];
                    bool result = callback.Invoke(_currentState, newState);
                    if (result == false)
                    {
                        canChangeState = false;
                        break;
                    }
                }

                if (canChangeState == false)
                {
                    DebugLog("{0}.ChangeState({1}) was prevented by someone with CanChangeState Func, will stay at state {2}.", GetType(), _currentState, _previousState);
                    return false;
                }
            }
            return true;
        }

        public bool ChangeState(State newState, bool force = false)
        {
            if (force == false)
            {
                if (_currentState == newState)
                {
                    return false;
                }
            }

            DebugLog($"changin to {newState} from {_currentState}");

            // State Out transition
            switch (_currentState)
            {
                case State.Grounded:
                    {
                        if (newState == State.Falling)
                        {
                            _hasBeganToFallFromGroundedState = true;
                        }
                        //_boneSphere.SpringJoints[4].spring *= 5f;
                    }
                    break;
                case State.Falling:
                    {
                    }
                    break;
                case State.Bumping:
                    break;
                case State.StartJump:
                    {
                    }
                    break;
                case State.Jumping:
                    break;
                case State.EndJump:
                    break;
                case State.WallGrab:
                    {
                    }
                    break;
                case State.WallJump:
                    {
                    }
                    break;
                case State.Dashing:
                    {
                        foreach (var rigidbody in _rigidbodies)
                        {
                            _dash.EndDash(rigidbody);
                        }

                        _characterCollision.ResetMaxDistances();
                    }
                    break;
                case State.DamageTaken:
                    {
                        _disableControlWhenDamageTaken.ForceFinishState();
                    }
                    break;
                default: break;
            }

            _previousState = _currentState;
            _currentState = newState;
            StateChanged?.Invoke(this, new CubeControllerEventArgs(_previousState, newState));

            // State In Transition
            switch (newState)
            {
                case State.Grounded:
                    {
                        ResetCurrentValues();
                        //_boneSphere.SpringJoints[4].spring /= 5f;
                    }
                    break;
                case State.Falling:
                    {

                    }
                    break;
                case State.Bumping:
                    {
                        _currentDurationToDisableControlDuringBump = 0f;

                        ResetRigidbodiesVelocity();

                        if (_resetJumpCountWhenBumping == true)
                        {
                            ResetJumpCount(_allowedJumpCountWhenBumping);
                        }
                        if (_resetDashJumpCountWhenBumping == true)
                        {
                            ResetDashCount(_allowedDashCountWhenBumping);
                        }
                    }
                    break;
                case State.StartJump:
                    {
                        if (_previousState == State.WallJump)
                        {
                            _shouldChangeToFallingStateWhenReleasingJump = false;
                        }
                        // We skip CanChangeState here because Jumping is the direct result of StartJump and we prevent it already in TryJump
                        ChangeState(State.Jumping);
                    }
                    break;
                case State.Jumping:
                    break;
                case State.EndJump:
                    {
                    }
                    break;
                case State.WallGrab:
                    {
                        //_wallNormalDuringLastWallGrab = _characterCollision.WallNormal;
                        _stickingToWall = true;
                    }
                    break;
                case State.WallJump:
                    {
                        _currentWallGrabDisableDuration = 0f;

                        if (_resetJumpCountWhenWallJumping == true)
                        {
                            ResetJumpCount(_allowedJumpCountWhenWallJumping);
                        }
                        if (_resetDashCountWhenWallJumping == true)
                        {
                            ResetDashCount(_allowedDashCountWhenWallJumping);
                        }
                    }
                    break;
                case State.Dashing:
                    {
                        if (_resetWallGrabDisableDurationWhenDashing == true)
                        {
                            ResetWallGrabDisableDuration();
                        }
                        //_characterCollision.SetMaxDistance(_wallRaycastDistanceWhenDashing);

                    }
                    break;
                case State.DamageTaken:
                    {
                        _disableControlWhenDamageTaken.Start();
                    }
                    break;
                default:
                    {
                        //LogError("{0}.ChangeState({0}) to an unhandled state.", GetType().Name);
                    }
                    break;
            }

            return true;
        }

        public void ResetJumpCount(int allowedForceCount)
        {
            _jump.ResetCurrentForceCount(_jump.MaximumAllowedForcesWhileInAir - allowedForceCount);
        }

        private void ResetDashCount(int allowedForceCount)
        {
            //_dash.ResetCurrentForceCount(_dash.MaximumAllowedForcesWhileInAir - allowedForceCount);
        }

        private void ResetWallGrabDisableDuration()
        {
            _currentWallGrabDisableDuration = _wallGrabDisableDuration;
        }

        private void UpdateStates()
        {
            _inputMovement = _playerController.HorizontalMove;
            _rawInputMovement = _inputMovement == 0 ? 0 : (_inputMovement > 0 ? 1 : -1);
            //LookToDirection();

            switch (_currentState)
            {
                case State.Grounded:
                    {
                        CheckGround();
                        //_characterCollision.HandleWallCollisionAndApplyBonusYReplacement(_lastMovementDirection);

                        if (TryJump() == false)
                        {
                            DisableGroundRaycastWhenJumping();
                        }

                        if (TryDash() == false)
                        {
                            DisableGroundRaycastWhenDashing();
                        }
                    }
                    break;
                case State.Falling:
                    {
                        if (Rigidbody.velocity.magnitude < 0.1f)
                        {
                            ChangeState(State.Grounded);
                        }
                        if (DisableControlAfterBump() == false)
                        {
                            return;
                        }
                        CheckGround();
                        _characterCollision.HandleWallCollisionAndApplyBonusYReplacement(_lastMovementDirection);
                        TryStickingToWall();
                        if (TryJump() == false)
                        {
                            DisableGroundRaycastWhenJumping();
                        }
                        if (TryDash() == false)
                        {
                            DisableGroundRaycastWhenDashing();
                        }
                    }
                    break;
                case State.Bumping:
                    {
                        if (DisableControlAfterBump() == false)
                        {
                            return;
                        }
                        CheckGround();
                        _characterCollision.HandleWallCollisionAndApplyBonusYReplacement(_lastMovementDirection);
                        TryStickingToWall();
                        if (TryJump() == false)
                        {
                            DisableGroundRaycastWhenJumping();
                        }
                        if (TryDash() == false)
                        {
                            DisableGroundRaycastWhenDashing();
                        }
                    }
                    break;
                case State.StartJump:
                    {
                    }
                    break;
                case State.Jumping:
                    {
                        if (Rigidbody.velocity.magnitude < 0.1f)
                        {
                            //ChangeState(State.Grounded);
                        }
                        CheckGround();

                        if (_currentState == State.Jumping) // CheckGround can change state
                        {
                            _characterCollision.HandleWallCollisionAndApplyBonusYReplacement(_lastMovementDirection);
                            TryStickingToWall();

                            if (_shouldChangeToFallingStateWhenReleasingJump == true && _playerController.IsJumpButtonPressed == false)
                            {
                                ResetRigidbodiesVelocity();
                                ChangeState(State.Falling);
                            }

                            if (TryJump() == false)
                            {
                                DisableGroundRaycastWhenJumping();
                            }
                            if (TryDash() == false)
                            {
                                DisableGroundRaycastWhenDashing();
                            }
                        }
                    }
                    break;
                case State.EndJump:
                    {
                        CheckGround();
                    }
                    break;
                case State.WallGrab:
                    {
                        TryWallJump();
                        DisableGroundRaycastWhenJumping(); // check this
                    }
                    break;
                case State.WallJump:
                    {
                        DisableWallGrabForAShortTime();
                        CheckGround();
                        _characterCollision.HandleWallCollisionAndApplyBonusYReplacement(_lastMovementDirection);
                        TryStickingToWall();

                        if (TryJump() == false)
                        {
                            DisableGroundRaycastWhenJumping();
                        }
                        if (TryDash() == false)
                        {
                            DisableGroundRaycastWhenDashing();
                        }
                    }
                    break;
                case State.Dashing:
                    {
                        DisableGroundRaycastWhenDashing(); // check this

                        bool hasDashEnded = _dash.UpdateTimer();
                        if (hasDashEnded == false)
                        {
                            _characterCollision.HandleWallCollisionAndApplyBonusYReplacement(_lastMovementDirection);
                            TryStickingToWall();

                            // TODO AL : try jump check
                            if (_canJumpDuringDash == true && TryJump() == false)
                            {
                                DisableGroundRaycastWhenJumping();
                            }
                        }
                        else
                        {
                            CheckGround();
                            if (_currentState == State.Dashing) // if still falling after the checking the ground
                            {
                                ChangeState(State.Falling); // check this, maybe add a new state DashFalling
                            }
                        }
                    }
                    break;
                case State.DamageTaken:
                    {
                        DisableGroundRaycastWhenJumping();
                        DisableGroundRaycastWhenDashing();

                        if (_disableControlWhenDamageTaken.Update() == true)
                        {
                            CheckGround();
                        }
                    }
                    break;
                default: break;
            }

            bool DisableControlAfterBump()
            {
                // Disable controls a short time after a bump
                if (IsBumpingDisableControls == true)
                {
                    // in any case, check if we passed the apex of a bumped jump
                    // then if half of the duration passed, we can check if we're grounded and interrupt the timer to release controls
                    // Or release control mid air after the timer has passed
                    if (_rootRigidbody.velocity.y < 0 || (_currentDurationToDisableControlDuringBump * 2 > _durationToDisableControlDuringBump && IsGrounded == true))
                    {
                        _currentDurationToDisableControlDuringBump = _durationToDisableControlDuringBump;
                    }
                    _currentDurationToDisableControlDuringBump += Time.deltaTime;
                    return false;
                }
                return true;
            }

            void DisableWallGrabForAShortTime()
            {
                //Debug.LogFormat("_currentWallGrabDisableDuration {0}", _currentState);
                if (_currentWallGrabDisableDuration < _wallGrabDisableDuration)
                {
                    _currentWallGrabDisableDuration += Time.deltaTime;
                }
            }

            void CheckGround()
            {
                // Check ground only if a little time has passed after we jump to avoid false Falling information
                if (_currentDurationToDisableGroundRaycastWhenJumping >= _durationToDisableGroundRaycastWhenJumping/* && _currentState != State.WallGrab*/
                    && _currentDurationToDisableGroundRaycastWhenDashing >= _durationToDisableGroundRaycastWhenDashing
                    )
                {
                    CheckGroundAndChangeStateAccordingly();
                }
            }

            void TryStickingToWall()
            {
                if (_stickyModeOn == true && _stickingToWall == false)
                {
                    bool foundWall = false;
                    _characterCollision.MidLeftRaycaster.RaycastAll(out RaycastHit[] midLeftHits);
                    _characterCollision.MidRightRaycaster.RaycastAll(out RaycastHit[] midRightHits);
                    foreach (RaycastHit raycastHit in midLeftHits)
                    {
                        if (raycastHit.normal.z > 0.7 && foundWall == false)
                        {
                            foundWall = true;
                            _stickingLeft = true;
                            break;
                        }
                        else
                        {
                            foundWall = false;
                            _stickingLeft = false;
                        }
                    }
                    foreach (RaycastHit raycastHit in midRightHits)
                    {
                        if (raycastHit.normal.z < -0.7 && foundWall == false)
                        {
                            foundWall = true;
                            _stickingLeft = false;
                            break;
                        }
                        else
                        {
                            foundWall = false;
                            _stickingLeft = false;
                        }
                    }
                    if (foundWall == true)
                    {
                        StickToWall();
                    }
                }
            }

            void TryWallJump()
            {
                if (_willPerformWallJump == true)
                {
                    // Consume input in every cases
                    _willPerformWallJump = false;

                    bool canChangeState = CanChangeState(State.WallJump);
                    if (canChangeState == true)
                    {
                        var impulseDirection = _characterCollision.WallNormal * _wallJumpWallNormalForce + new Vector3(0f, Mathf.Sqrt(2 * (_wallJumpHeight * _ascendingGravityScale) * Mathf.Abs(Physics.gravity.y)), Mathf.Sqrt(2 * (_wallJumpHeight / 4 * _ascendingGravityScale) * Mathf.Abs(Physics.gravity.y)) * (_stickingLeft ? 1 : -1));
                        Debug.DrawRay(transform.position, transform.position + impulseDirection, Color.blue, 10f);
                        foreach (var rigidbody in _rigidbodies)
                        {
                            rigidbody.AddForce(impulseDirection, ForceMode.Impulse);
                        }
                        _stickingToWall = false;
                        ChangeState(State.WallJump);
                    }
                }
            }

            bool TryJump()
            {
                if (IsWallGrabDisabled == false && _willPerformJump == true && _currentState != CubeController.State.Jumping)
                {
                    _willPerformJump = false;
                    if (_hasBeganToFallFromGroundedState == true && _hasBeganToFallFromGroundedStateAndDidJump == false && _resetJumpCountWhenFalling == true)
                    {
                        ResetJumpCount(_allowedJumpCountWhenFalling);
                        _hasBeganToFallFromGroundedStateAndDidJump = true;
                    }

                    bool canJump = _jump.CanApplyForce() && CanChangeState(State.StartJump);

                    if (canJump == true)
                    {
                        ResetRigidbodiesVelocity();
                        foreach (var rigidbody in _rigidbodies)
                        {
                            _jump.TryApplyForce(rigidbody);
                        }
                        //_jump.TryApplyForce(Rigidbody);
                        ChangeState(State.StartJump);
                        _currentDurationToDisableGroundRaycastWhenJumping = 0;
                    }
                    return canJump;
                }
                return false;
            }

            bool TryDash()
            {
                /*if (_willPerformDash == true && (Rigidbody.velocity.z >= 0.5f || Rigidbody.velocity.z <= -0.5f))
                {
                    _willPerformDash = false;
                    bool result = false;
                    _characterCollision.HandleWallCollisionAndApplyBonusYReplacement(_lastMovementDirection);
                    if (HasAWallInFrontOfCharacter == false)
                    {

                        if (_hasBeganToFallFromGroundedState == true && _hasBeganToFallFromGroundedStateAndDidDash == false && _resetJumpCountWhenFalling == true)
                        {
                            ResetDashCount(_allowedDashCountWhenFalling);
                            _hasBeganToFallFromGroundedStateAndDidDash = true;
                        }


                        result = _dash.CanApplyForce() && CanChangeState(State.Dashing);
                        if (result == true)
                        {
                            foreach (var rigidbody in _rigidbodies)
                                result = _dash.TryApplyForce(rigidbody);
                            ChangeState(State.Dashing);
                            _currentDurationToDisableGroundRaycastWhenDashing = 0;
                        }
                    }
                    return result;
                }*/
                return false;
            }

            void DisableGroundRaycastWhenJumping()
            {
                if (_currentDurationToDisableGroundRaycastWhenJumping < _durationToDisableGroundRaycastWhenJumping)
                {
                    //Debug.LogFormat("DisableGroundRaycastWhenJumping {0}", _currentState);
                    _currentDurationToDisableGroundRaycastWhenJumping += Time.deltaTime;
                }
            }

            void DisableGroundRaycastWhenDashing()
            {
                if (_currentDurationToDisableGroundRaycastWhenDashing < _durationToDisableGroundRaycastWhenDashing)
                {
                    //Debug.LogFormat("DisableGroundRaycastWhenDashing {0}", _currentState);
                    _currentDurationToDisableGroundRaycastWhenDashing += Time.deltaTime;
                }
            }

            if (_stickyModeDuration.IsRunning == true)
            {
                _stickyModeDuration.Update();
                _currentGrabbingStamina = 1 - _stickyModeDuration.Progress;
                if (_currentGrabbingStamina <= 0)
                {
                    _willPerformWallJump = true;
                    SetStickyMode(false);
                    TryWallJump();
                }
            }
            if (_staminaRegenTimer.IsRunning == true)
            {
                _staminaRegenTimer.Update();
                _currentGrabbingStamina = _staminaRegenTimer.Progress;
            }
        }

        private void FixedUpdate()
        {
            Vector3 velocity = _rootRigidbody.velocity;
            bool isGrounded = IsGrounded;

            switch (_currentState)
            {
                case State.Grounded:
                    {
                        ApplyMovementOrDecelerationForce(isGrounded);
                        NullifyMovementAgainstAWallInFrontOf();
                        NullifyMovementAgainstAWallBehind();
                        // If grounded, project velocity by the floor normal to handle slopes
                        //velocity = Vector3.ProjectOnPlane(velocity, _characterCollision.GroundNormal);
                        ApplyGravity(ref velocity);
                    }
                    break;
                case State.Falling:
                    {
                        if (DisableControlDuringBump() == false)
                        {
                            return;
                        }
                        ApplyMovementOrDecelerationForce(isGrounded);
                        NullifyMovementAgainstAWallInFrontOf();
                        NullifyMovementAgainstAWallBehind();
                        ApplyGravity(ref velocity);
                        if (_gliding == true)
                        {
                            foreach (var rigidbody in _rigidbodies)
                            {
                                rigidbody.velocity = velocity;
                            }
                        }
                    }
                    break;
                case State.Bumping:
                    {
                        if (DisableControlDuringBump() == false)
                        {
                            return;
                        }
                        ApplyMovementOrDecelerationForce(isGrounded);
                        NullifyMovementAgainstAWallInFrontOf();
                        NullifyMovementAgainstAWallBehind();
                        ApplyGravity(ref velocity);

                    }
                    break;
                case State.StartJump:
                    {
                    }
                    break;
                case State.Jumping:
                    {
                        if (IsWallGrabDisabled == true)
                        {
                            //Debug.LogFormat("_currentDurationToDisableControlDuringBump {0}", _currentState);
                            ApplyGravityImmediate();
                            return;
                        }
                        ApplyMovementOrDecelerationForce(isGrounded);
                        NullifyMovementAgainstAWallInFrontOf();
                        NullifyMovementAgainstAWallBehind();
                        ApplyGravity(ref velocity);
                    }
                    break;
                case State.EndJump:
                    break;
                case State.WallGrab:
                    {
                        // Nullify velocity when wall grabbing
                        velocity = new Vector3(0f, 0f, 0.25f * AirMoveSpeed * (_stickingLeft == true ? -1 : 1));
                    }
                    break;
                case State.WallJump:
                    {
                        if (IsWallGrabDisabled == true)
                        {
                            //Debug.LogFormat("_currentDurationToDisableControlDuringBump {0}", _currentState);
                            ApplyGravityImmediate();
                            return;
                        }
                        ApplyMovementOrDecelerationForce(isGrounded);
                        ApplyGravity(ref velocity);

                    }
                    break;
                case State.Dashing:
                    {
                        DebugLog("while Dashing : WallLeft:{0} | WallRight:{1} | Slope:{2}", HasAWallBehindCharacter, HasAWallInFrontOfCharacter, HasASlopeInFrontOfOrBehindCharacter);

                        NullifyMovementAgainstAWallInFrontOf();
                        /*if (_rawInputMovement != 0 && _rawInputMovement != _dash.LastMovementDirection)
                        {
                            NullifyMovementAgainstAWallBehind();
                        }*/
                        NullifyMovementAgainstAWallBehind();

                        if (HasAWallInFrontOfCharacter == true)
                        {
                            // Place character closer to the wall to enable dash / wall grabbing combo
                            Vector3 replacementPosition = transform.position;
                            replacementPosition.z = _characterCollision.GetReplacementZPosition(_dash.LastMovementDirection);
                            transform.position = replacementPosition;
                            //Debug.DrawLine(replacementPosition, replacementPosition + Vector3.up * 8, Color.blue, 10);

                            _dash.StopTimer();
                            velocity = Vector3.zero;
                            ApplyGravity(ref velocity);
                        }

                        if (HasASlopeInFrontOfOrBehindCharacter == true)
                        {
                            Vector3 replacementPosition = _characterCollision.LastSlopeRaycastHitResult.point;
                            transform.position = replacementPosition;
                            //Debug.DrawLine(replacementPosition, replacementPosition + Vector3.up * 8, Color.blue, 10);

                            _dash.StopTimer();
                            velocity = Vector3.zero;
                            ApplyGravity(ref velocity);
                        }
                    }
                    break;
                case State.DamageTaken:
                    {
                        ApplyGravity(ref velocity);
                    }
                    break;
                default: break;
            }

            // Clamp maximum velocity
            if (velocity.sqrMagnitude > _maxVelocitySqr)
            {
                //velocity = Vector3.ClampMagnitude(velocity, _maxVelocity);
            }

            // Then apply velocity to rigidbody
            bool DisableControlDuringBump()
            {
                if (_currentDurationToDisableControlDuringBump < _durationToDisableControlDuringBump)
                {
                    //Debug.LogFormat("_currentDurationToDisableControlDuringBump {0}", _currentState);
                    ApplyGravityImmediate();
                    return false;
                }
                return true;
            }

            void ApplyMovementOrDecelerationForce(bool isGrounded)
            {
                // Apply input's movement if any, or else apply a decelerrationForce
                if (_inputMovement != 0)
                {
                    // do not enter if we're in WallJump with a wall in front of us to avoid wall clipping / stucking
                    if (_currentState != State.WallJump || (HasAWallInFrontOfCharacter == false && HasAWallBehindCharacter == false))
                    {
                        velocity.z = _inputMovement * (isGrounded == true ? _groundMoveSpeed : _currentAirSpeed);
                    }
                }
                else if (isGrounded == true)
                {
                    velocity.z = Mathf.MoveTowards(velocity.z, 0, _groundFriction);
                }

                //Debug.Log("Decellerating : " + (isGrounded == true && (_isOnPlatform == false || _isPlatformMoving == true)));
            }

            void NullifyMovementAgainstAWallInFrontOf()
            {
                // Check if we collide against a wall and nullify horizontal movement if so
                if (HasAWallInFrontOfCharacter == true && _inputMovement < 0)
                {
                    // let only vertical movement pass through
                    velocity = new Vector3(0f, velocity.y, 0f);
                }
            }

            void NullifyMovementAgainstAWallBehind()
            {
                // Check if we collide against a wall and nullify horizontal movement if so
                if (HasAWallBehindCharacter == true && _inputMovement > 0)
                {
                    // let only vertical movement pass through
                    velocity = new Vector3(0f, velocity.y, 0f);
                }
            }

            if (velocity.z < 0.1f && _playerController.HorizontalMove == 0)
            {
                velocity.z = 0f;
            }
            if (_slingshotHandler.Slingshotting == false && _slingshotHandler.ChargingSlingshot == false)
            {

                if (_gliding == true)
                {
                    velocity.y = velocity.y / _glidingSpeedDivider;
                }
                for (int i = 1; i < _rigidbodies.Length; i++)
                {
                    _rigidbodies[i].velocity = _rigidbodies[i].velocity * 0.975f;
                }
                Rigidbody.velocity = velocity;


            }

            Debug.DrawLine(transform.position, transform.position + velocity * 5, Color.yellow);
        }

        private void TryWallGrab()
        {
            if (_enableWallGrab == false)
            {
                return;
            }

            if (_stickyModeOn == false && _stickyModeDuration.IsRunning == false && _currentGrabbingStamina > 0.1)
            {
                SetStickyMode(true);
            }
            else if (_stickyModeOn == true && _stickyModeDuration.IsRunning == true)
            {
                SetStickyMode(false);
            }
        }

        private void SetStickyMode(bool status)
        {
            if (status == true && _stickyModeOn == false)
            {
                _stickyModeDuration.Start();
                _stickyModeDuration.TimeElapsed = (1 - _currentGrabbingStamina) * _stickyModeDuration.Duration;
                _characterCollision.SlopeNormalThreshold = 0;
                _stickyModeOn = true;
                _staminaRegenTimer.ForceFinishState();
            }
            else if (status == false && _stickyModeOn == true)
            {
                _characterCollision.SlopeNormalThreshold = _characterCollision.DefaultSlopeNormalThreshold;
                _stickyModeOn = false;
                _stickingToWall = false;
                _stickyModeDuration.ForceFinishState();
                _staminaRegenTimer.Start(_completeStaminaRegenDuration);
                _staminaRegenTimer.TimeElapsed = (_currentGrabbingStamina * _completeStaminaRegenDuration);
            }
        }

        private void StickToWall()
        {
            bool canChangeState = CanChangeState(State.WallGrab);
            if (canChangeState == true)
            {
                ChangeState(State.WallGrab);
            }
        }

        private void ApplyGravity(ref Vector3 velocity)
        {
            float gravityScale = (velocity.y > 0 ? _ascendingGravityScale : _currentDescendingGravityScale);
            velocity.y += Physics.gravity.y * gravityScale * Time.deltaTime;
        }

        private void ApplyGravityImmediate()
        {
            Vector3 gravityVelocity = _rootRigidbody.velocity;
            ApplyGravity(ref gravityVelocity);
            if (_stickingToWall == false)
            {
                foreach (var rigidbody in _rigidbodies)
                {
                    rigidbody.velocity = gravityVelocity;
                }
            }
        }

        private void CheckGroundAndChangeStateAccordingly()
        {
            bool isTouchingGround = _characterCollision.CheckGround();
            // We voluntarily skip the CanChangeState check here because all ChangeState in this function is triggered by the environment and not by the player input.
            if (isTouchingGround == true)
            {
                if (_currentState == State.Jumping || _currentState == State.WallJump)
                {
                    ChangeState(State.EndJump);
                }
                ChangeState(State.Grounded);
            }
            else
            {
                if (_currentState == State.Grounded)
                {
                    if (Rigidbody.velocity.magnitude > 0.1f)
                    {
                        ChangeState(State.Falling);
                    }
                }
            }
        }

        public void UpdateScale(float scale)
        {
            _jump.JumpHeight = _defaultJumpHeight * scale;
            foreach (Rigidbody rigidbody in _rigidbodies)
            {
                rigidbody.mass = 1 * scale;
            }
            _groundFriction = _defaultGroundFriction * (1 + (1 - scale));
            SphereCollider rootCollider = _colliders[0] as SphereCollider;
            rootCollider.radius = _boneSphere.RootColliderSize * scale;

            for (int i = 1; i < _colliders.Length; i++)
            {
                SphereCollider collider = _colliders[i] as SphereCollider;
                collider.radius = _boneSphere.ColliderSize * scale;
            }
        }

        public void LaunchBlob(Vector3 impulse)
        {
            foreach (var rigidbody in _rigidbodies)
            {
                rigidbody.AddForce(impulse, ForceMode.Impulse);
            }
        }

        private void LookToDirection()
        {
            //float currentMovementDirection;
            //if (Mathf.Abs(_inputMovement) > Mathf.Epsilon) // lazy deadzone
            //{
            //    currentMovementDirection = _inputMovement;
            //}
            //else
            //{
            //    currentMovementDirection = _lastMovementDirection;
            //}
            //int movementDirection = (currentMovementDirection > 0 ? 1 : -1);

            //if (_lastMovementDirection != movementDirection)
            //{
            //    _currentTurnTime = 0;
            //    _lastMovementDirection = movementDirection;
            //}

            //_currentTurnTime = Mathf.Clamp(_currentTurnTime + _turnSpeed * Time.deltaTime, 0, 1f);

            //// ideas to improve perfs : do not set the rotation if the _currentTurnTime is at 1f since last frame
            //Quaternion rotation = Quaternion.LookRotation(Vector3.forward * movementDirection);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _uturnCurve.Evaluate(_currentTurnTime));
        }

        #region Editor
        private void OnValidate()
        {
            _groundMoveSpeed = Mathf.Clamp(_groundMoveSpeed, 0, float.MaxValue);
            _groundFriction = Mathf.Clamp(_groundFriction, 0, float.MaxValue);
            _currentAirSpeed = Mathf.Clamp(_currentAirSpeed, 0, float.MaxValue);
            _wallJumpHeight = Mathf.Clamp(_wallJumpHeight, 0, float.MaxValue);

            _jump?.Validate();
            _dash?.Validate();
            _ascendingGravityScale = Mathf.Clamp(_ascendingGravityScale, 0, float.MaxValue);
            _currentDescendingGravityScale = Mathf.Clamp(_currentDescendingGravityScale, 0, float.MaxValue);
            _turnSpeed = Mathf.Clamp(_turnSpeed, 0, float.MaxValue);
        }

        private void DebugLog(string message, params object[] args)
        {
            if (_debugMode == true)
            {
                Log(message, args);
            }
        }

        private void Log(string message, params object[] args)
        {
            Debug.LogFormat(string.Format(LOG_TOKEN, GetType().Name, message), args);
        }

        private void LogError(string message, params object[] args)
        {
            Debug.LogErrorFormat(string.Format(LOG_TOKEN, GetType().Name, message), args);
        }
        #endregion Editor
    }
    #endregion Methods
}