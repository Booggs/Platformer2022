namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public class EnvironmentInteractable : MonoBehaviour
    {
        protected bool _interactableActive = true;

        public bool InteractableActive => _interactableActive;

        public virtual void UseInteractable(PlayerReferences playerRefs)
        {
            print("interactable activated");
        }

        public virtual void LeaveInteractable()
        {
            print("interactable deactivated");
        }

        public virtual void NavigateLeftInteractable()
        {
            print("navigating left");
        }

        public virtual void NavigateRightInteractable()
        {
            print("navigating right");
        }

        public void ActivateInteractable(bool status)
        {
            _interactableActive = status;
        }
    }
}