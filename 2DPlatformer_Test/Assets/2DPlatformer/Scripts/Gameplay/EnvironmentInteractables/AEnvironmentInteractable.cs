namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public abstract class AEnvironmentInteractable : MonoBehaviour
    {
        [SerializeField]
        protected bool _unlockedAtStart = true;

        protected PlayerReferences _playerRefs = null;

        public bool InteractableActive => _unlockedAtStart;

        public void SetPlayerInteractable(PhysicsTriggerEvent physicsTriggerEvent, Collider other)
        {
            InteractionManager interactionManager = CheckForPlayer(other);
            if(interactionManager != null)
            {
                interactionManager.CurrentEnvironmentInteractable = this;
                _playerRefs = interactionManager.PlayerRefs;
            }
        }

        public void ResetPlayerInteractable(PhysicsTriggerEvent physicsTriggerEvent, Collider other)
        {
            InteractionManager interactionManager = CheckForPlayer(other);
            if (interactionManager != null)
            {
                interactionManager.CurrentEnvironmentInteractable = null;
                LeaveInteractable();
                _playerRefs = null;
            }
        }

        public virtual void UseInteractable()
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
            _unlockedAtStart = status;
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