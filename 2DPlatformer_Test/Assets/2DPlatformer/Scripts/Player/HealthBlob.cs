namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class HealthBlob : MonoBehaviour
    {
        [SerializeField]
        private float _verticalForce = 6f;

        [SerializeField]
        private float _horizontalRange = 2f;

        [SerializeField]
        private bool _startJump = true;

        private int _healingAmount = 1;
        private Rigidbody[] _rigidbodies;

        private void Awake()
        {
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
            if (_startJump)
                LaunchHealthBlob();
        }

        public int HealingAmount
        {
            get
            {
                return _healingAmount;
            }
            set
            {
                _healingAmount = value;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerDamageable playerDamageable = other.GetComponentInParent<PlayerDamageable>();
            if (playerDamageable != null && playerDamageable.IsInjured)
            {
                playerDamageable.RestoreHealth(_healingAmount);
                Destroy(gameObject);
            }
        }

        public void LaunchHealthBlob()
        {
            Vector3 impulse = new Vector3(0, _verticalForce, Random.Range(-_horizontalRange - 2, _horizontalRange + 2));
            foreach (var rigidbody in _rigidbodies)
            {
                rigidbody.AddForce(impulse, ForceMode.Impulse);
            }
        }
    }

}