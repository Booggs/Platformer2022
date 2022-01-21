namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;

    public class SwitchTarget : MonoBehaviour
    {
        private AEnvironmentInteractable _targetInteractable = null;

        private void Awake()
        {
            TryGetComponent<AEnvironmentInteractable>(out _targetInteractable);
        }

        public void RemoteUnlockInteractable(bool newStatus)
        {
            _targetInteractable.ActivateInteractable(newStatus);
        }

        public void RemoteUseInteractable()
        {
            _targetInteractable.UseInteractable();
        }
    }
}