namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;


    public class InteractionManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private PlayerReferences _playerRefs = null;

        private PlayerController _playerController;
        private AEnvironmentInteractable _currentEnvironmentInteractable;

        public AEnvironmentInteractable CurrentEnvironmentInteractable
        {
            get
            {
                return _currentEnvironmentInteractable;
            }
            set
            {
                _currentEnvironmentInteractable = value;
            }
        }

        public PlayerReferences PlayerRefs => _playerRefs;

        private void Awake()
        {
            LevelReferences levelReference = LevelReferences.Instance;
            levelReference.PlayerReferences.TryGetPlayerController(out _playerController);
        }

        #region EventsListener
        private void OnEnable()
        {
            _playerController.UseInteractablePerformed -= PlayerController_UseInteractablePerformed;
            _playerController.UseInteractablePerformed += PlayerController_UseInteractablePerformed;

            _playerController.LeaveInteractablePerformed -= PlayerController_LeaveInteractablePerformed;
            _playerController.LeaveInteractablePerformed += PlayerController_LeaveInteractablePerformed;

            _playerController.NavigateLeftInteractablePerformed -= PlayerController_NavigateLeftInteractablePerformed;
            _playerController.NavigateLeftInteractablePerformed += PlayerController_NavigateLeftInteractablePerformed;

            _playerController.NavigateRightInteractablePerformed -= PlayerController_NavigateRightInteractablePerformed;
            _playerController.NavigateRightInteractablePerformed += PlayerController_NavigateRightInteractablePerformed;
        }

        private void OnDisable()
        {
            _playerController.UseInteractablePerformed -= PlayerController_UseInteractablePerformed;
            _playerController.LeaveInteractablePerformed -= PlayerController_LeaveInteractablePerformed;
            _playerController.NavigateLeftInteractablePerformed -= PlayerController_NavigateLeftInteractablePerformed;
            _playerController.NavigateRightInteractablePerformed -= PlayerController_NavigateRightInteractablePerformed;
        }
        #endregion

        private void PlayerController_UseInteractablePerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_currentEnvironmentInteractable != null)
            {
                _currentEnvironmentInteractable.UseInteractable();
            }

            // DEPRECATED
            // Used in order to interact with Interactables without using UnityEvents
            /*Collider[] overlappingColliders = Physics.OverlapSphere(this.gameObject.transform.position, 0.5f);
            foreach (var overlappingCollider in overlappingColliders)
            {
                EnvironmentInteractable interactable = null;
                if ((interactable = overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>()) == true)
                {
                    interactable.UseInteractable(_playerRefs);
                }
            }*/
        }

        private void PlayerController_LeaveInteractablePerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_currentEnvironmentInteractable != null)
            {
                _currentEnvironmentInteractable.LeaveInteractable();
            }
        }

        private void PlayerController_NavigateLeftInteractablePerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_currentEnvironmentInteractable != null)
            {
                _currentEnvironmentInteractable.NavigateLeftInteractable();
            }
        }

        private void PlayerController_NavigateRightInteractablePerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            if (_currentEnvironmentInteractable != null)
            {
                _currentEnvironmentInteractable.NavigateRightInteractable();
            }
        }
    }
}
