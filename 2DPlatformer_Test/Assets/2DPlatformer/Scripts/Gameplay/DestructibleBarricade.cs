namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Utilities;

    public class DestructibleBarricade : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _debrisPrefab = null;

        [SerializeField]
        private int _debrisNumber = 10;

        [SerializeField]
        private float _explosionForce = 75f;

        [SerializeField]
        private GameObjectDestroyer _destroyer = null;

        private List<Rigidbody> _debrisRigidbodies = new List<Rigidbody>();
        private SlingshotHandler _slingshotHandler = null;
        private bool _destroying = false;

        private void OnTriggerEnter(Collider other)
        {
            _slingshotHandler = other.GetComponentInParent<SlingshotHandler>();
            if (_slingshotHandler != null && _slingshotHandler.Slingshotting && _destroying == false)
            {
                DestroyBarricade();
            }
        }

        private void DestroyBarricade()
        {
            _destroying = true;
            for (int i = 0; i < _debrisNumber; i++)
            {
                _debrisRigidbodies.Add(Instantiate(_debrisPrefab, new Vector3(transform.position.x, transform.position.y + Random.Range(-1, 1), transform.position.z), Random.rotation));
                float scale = Random.Range(1.15f, 2.15f);
                _debrisRigidbodies[i].transform.localScale = new Vector3(scale, scale, scale);
            }
            foreach (Rigidbody rigidbody in _debrisRigidbodies)
            {
                rigidbody.AddExplosionForce(_explosionForce, _slingshotHandler.transform.position, 2);
            }
            _destroyer.Destroy();
        }
    }
}