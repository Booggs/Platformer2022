namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public class EnvironmentInteractable : MonoBehaviour
    {
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
    }
}