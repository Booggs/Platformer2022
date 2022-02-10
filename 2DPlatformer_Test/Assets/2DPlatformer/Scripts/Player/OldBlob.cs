namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class OldBlob : MonoBehaviour, ICommandSender
    {
        [SerializeField]
        private PickupCommand _pickupCommand = null;

        [SerializeField]
        private int _maxHeal = 3;


        public GameObject GetGameObject()
        {
            return gameObject;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerDamageable playerDamageable = other.GetComponentInParent<PlayerDamageable>();
            if (playerDamageable != null)
            {
                _pickupCommand.Apply(this);
                for (int i = 0; i < _maxHeal; i++)
                {
                    playerDamageable.RestoreHealth(1);
                }
                Destroy(gameObject);
            }
        }
    }
}