namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;

    public class SwitchTarget : MonoBehaviour
    {
        [Header("Should the switch unlock or remotely interact with the interactable object ?")]
        [SerializeField]
        private bool _unlockerSwitch = false;

        private bool _isSwitchActivated = false;
        private AEnvironmentInteractable _targetInteractable = null;

        private void Awake()
        {
            TryGetComponent<AEnvironmentInteractable>(out _targetInteractable);
        }

        public void ToggleSwitch()
        {
            if (_isSwitchActivated == false)
            {
                if (_unlockerSwitch == true)
                    _targetInteractable.ActivateInteractable(true);
                else
                {
                    _targetInteractable.UseInteractable();
                }
                _isSwitchActivated = true;
            }
            else
            {
                if (_unlockerSwitch == true)
                    _targetInteractable.ActivateInteractable(false);
                else
                {
                    _targetInteractable.UseInteractable();
                }
                _isSwitchActivated = true;
            }
        }
    }
}