namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;

    public class InteractableSwitch : AEnvironmentInteractable
    {
        [SerializeField]
        private SwitchTarget _switchTarget = null;

        [Header("Should the switch unlock or remotely interact with the interactable object ?")]
        [SerializeField]
        private bool _unlockerSwitch = false;

        private bool _isSwitchActivated = false;

        public override void UseInteractable()
        {
            ToggleSwitch();
        }

        public void ToggleSwitch()
        {
            if (_isSwitchActivated == false)
            {
                if (_unlockerSwitch == true)
                    _switchTarget.RemoteUnlockInteractable(true);
                else
                {
                    _switchTarget.RemoteUseInteractable();
                }
                _isSwitchActivated = true;
            }
            else
            {
                if (_unlockerSwitch == true)
                    _switchTarget.RemoteUnlockInteractable(false);
                else
                {
                    _switchTarget.RemoteUseInteractable();
                }
                _isSwitchActivated = true;
            }
        }
    }
}