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
        private HealthBlob _healthBlobPrefab = null;

        [SerializeField]
        private int _healthBlobs = 3;

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
                for (int i = 0; i < _healthBlobs; i++)
                {
                    Instantiate<HealthBlob>(_healthBlobPrefab, transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }
    }
}