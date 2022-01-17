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
        private LootManager _lootManager;

        private void Awake()
        {
            LevelReferences levelReference = LevelReferences.Instance;
            _lootManager = levelReference.LootManager;
            levelReference.PlayerReferences.TryGetPlayerController(out _playerController);

            _playerController.UseInteractablePerformed -= PlayerController_UseInteractablePerformed;
            _playerController.UseInteractablePerformed += PlayerController_UseInteractablePerformed;

            _playerController.LeaveInteractablePerformed -= PlayerController_LeaveInteractablePerformed;
            _playerController.LeaveInteractablePerformed += PlayerController_LeaveInteractablePerformed;

            _playerController.NavigateLeftInteractablePerformed -= PlayerController_NavigateLeftInteractablePerformed;
            _playerController.NavigateLeftInteractablePerformed += PlayerController_NavigateLeftInteractablePerformed;

            _playerController.NavigateRightInteractablePerformed -= PlayerController_NavigateRightInteractablePerformed;
            _playerController.NavigateRightInteractablePerformed += PlayerController_NavigateRightInteractablePerformed;
        }

        private void PlayerController_UseInteractablePerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            Collider[] overlappingColliders = Physics.OverlapSphere(this.gameObject.transform.position, 0.5f);
            foreach (var overlappingCollider in overlappingColliders)
			{
                if (overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>())
                {
                    overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>().UseInteractable(_playerRefs);
                }
			}
        }
        private void PlayerController_LeaveInteractablePerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            Collider[] overlappingColliders = Physics.OverlapSphere(this.gameObject.transform.position, 0.5f);
            foreach (var overlappingCollider in overlappingColliders)
            {
                if (overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>())
                {
                    overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>().LeaveInteractable();
                }
            }
        }
        private void PlayerController_NavigateLeftInteractablePerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            Collider[] overlappingColliders = Physics.OverlapSphere(this.gameObject.transform.position, 0.5f);
            foreach (var overlappingCollider in overlappingColliders)
            {
                if (overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>())
                {
                    overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>().NavigateLeftInteractable();
                }
            }
        }
        private void PlayerController_NavigateRightInteractablePerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            Collider[] overlappingColliders = Physics.OverlapSphere(this.gameObject.transform.position, 0.5f);
            foreach (var overlappingCollider in overlappingColliders)
            {
                if (overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>())
                {
                    overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>().NavigateRightInteractable( );
                }
            }
        }
    }
}
