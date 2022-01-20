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

        public override void UseInteractable()
        {
            _switchTarget.ToggleSwitch();
        }
    }
}