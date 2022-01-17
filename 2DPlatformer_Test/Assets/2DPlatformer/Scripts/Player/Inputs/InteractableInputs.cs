namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public class InteractableInputs : MonoBehaviour
    {
        [SerializeField]
        private InputActionMapWrapper _inputActionMap = null;

        private InputAction _useInteractable = null;

        private void OnEnable()
        {
            if (_inputActionMap.TryFindAction("UseInteractable", out _useInteractable) == true)
            {
                _useInteractable.performed -= UseInteractable_performed;
                _useInteractable.performed += UseInteractable_performed;
                _useInteractable.Enable();
            }
        }

        private void OnDisable()
        {
            _useInteractable.performed -= UseInteractable_performed;

            _useInteractable.Disable();
        }

        private void UseInteractable_performed(InputAction.CallbackContext obj)
        {
            Debug.Log("Tata test");
        }
    }
}