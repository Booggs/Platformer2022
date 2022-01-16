namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public class EnvironmentInteractable : MonoBehaviour
    {
        public virtual void Interact(PlayerReferences playerRefs)
        {
            print("interactable activated");
        }
    }
}