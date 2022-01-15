namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public class Teleporter : EnvironmentInteractable
    {
        [SerializeField]
        private Transform teleportLocation;

        [SerializeField]
        private Teleporter targetTeleporter;
        
        protected override void Interact()
        {
            
        }

        private void Teleport(GameObject player)
        {
            player.transform.position = targetTeleporter.teleportLocation.position;
        }
    }
}