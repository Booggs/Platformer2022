namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;
    using GSGD2.Utilities;


    public class InteractionManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private PlayerReferences _playerRefs = null;

        private PlayerController _playerController;

        private void Awake()
        {
            LevelReferences levelReference = LevelReferences.Instance;
            levelReference.PlayerReferences.TryGetPlayerController(out _playerController);
            _playerController.UseItemPerformed -= PlayerController_UseItemPerformed;
            _playerController.UseItemPerformed += PlayerController_UseItemPerformed;
        }

        private void PlayerController_UseItemPerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            Collider[] overlappingColliders = Physics.OverlapSphere(this.gameObject.transform.position, 0.5f);
            foreach (var overlappingCollider in overlappingColliders)
			{
                if (overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>())
                {
                    overlappingCollider.gameObject.GetComponentInParent<EnvironmentInteractable>().Interact(_playerRefs);
                }
			}
        }
    }
}
