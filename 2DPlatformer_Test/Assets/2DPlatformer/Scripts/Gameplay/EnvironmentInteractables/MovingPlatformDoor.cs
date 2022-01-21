namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;

    public class MovingPlatformDoor : AEnvironmentInteractable
    {
        [SerializeField]
        private bool _staysOpen = false;

        private MovingPlatform _movingPlatform;
        private bool _doorOpen = false;

        private void Awake()
        {
            TryGetComponent<MovingPlatform>(out _movingPlatform);
        }

        public override void UseInteractable()
        {
            if (_movingPlatform != null)
            {
                if (_doorOpen == false)
                {
                    _doorOpen = true;
                    _movingPlatform.Play(false, true);
                }
                else if (_doorOpen == true && _staysOpen == false)
                {
                    _doorOpen = false;
                    _movingPlatform.PlayReverse(false, true);
                }
            }
        }
    }
}