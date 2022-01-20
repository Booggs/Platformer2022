namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public abstract class AEnvironmentInteractable : MonoBehaviour
    {
        protected bool _interactableActive = true;

        public bool InteractableActive => _interactableActive;

        public void SetPlayerInteractable(PhysicsTriggerEvent physicsTriggerEvent, Collider other)
        {
            InteractionManager interactionManager = CheckForPlayer(other);
            if(interactionManager != null)
            {
                interactionManager.CurrentEnvironmentInteractable = this;
            }
        }

        public void ResetPlayerInteractable(PhysicsTriggerEvent physicsTriggerEvent, Collider other)
        {
            InteractionManager interactionManager = CheckForPlayer(other);
            if (interactionManager != null)
            {
                interactionManager.CurrentEnvironmentInteractable = null;
                LeaveInteractable();
            }
        }

        public virtual void UseInteractable(PlayerReferences playerRefs)
        {
            return;
        }

        public virtual void LeaveInteractable()
        {
            return;
        }

        public virtual void NavigateLeftInteractable()
        {
            return;
        }

        public virtual void NavigateRightInteractable()
        {
            return;
        }

        public void ActivateInteractable(bool status)
        {
            _interactableActive = status;
        }

        protected void OnTriggerEnter(Collider other)
        {
            print(other.gameObject.name);
        }

        private InteractionManager CheckForPlayer(Collider other)
        {
            if (other.GetComponentInParent<InteractionManager>() != null)
                return other.GetComponentInParent<InteractionManager>();
            else return null;
        }
    }
}