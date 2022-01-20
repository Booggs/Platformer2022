namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;

    public class MovingPlatformDoor : AEnvironmentInteractable
    {
        private MovingPlatform _movingPlatform;

        private void Awake()
        {
            TryGetComponent<MovingPlatform>(out _movingPlatform);
        }

        public override void UseInteractable()
        {
            if (_movingPlatform != null)
            {
                _movingPlatform.Play(false, true);
            }
        }
    }
}