namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public class EnvironmentInteractable : MonoBehaviour
    {
        private PlayerController _playerController = null;

        private void Awake()
        {
            LevelReferences levelReference = LevelReferences.Instance;
            levelReference.PlayerReferences.TryGetPlayerController(out _playerController);
            _playerController.UseItemPerformed -= PlayerController_UseItemPerformed;
            _playerController.UseItemPerformed += PlayerController_UseItemPerformed;
        }

        private void PlayerController_UseItemPerformed(PlayerController sender, InputAction.CallbackContext obj)
        {
            print("Interacting with object");
            Interact();
        }

        protected virtual void Interact()
        {

        }
    }
}