namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public class Teleporter : AEnvironmentInteractable
    {
        [SerializeField]
        private Transform teleportLocation;

        [SerializeField]
        private Teleporter targetTeleporter;
        
        public override void UseInteractable()
        {
            Teleport(_playerRefs.gameObject);
        }

        private void Teleport(GameObject player)
        {
            player.transform.position = targetTeleporter.teleportLocation.position;
        }
    }
}