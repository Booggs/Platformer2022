namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;

    public class Zipline : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Transform _ziplineStart = null;

        [SerializeField]
        private Transform _ziplineTarget = null;

        [SerializeField]
        private BoxCollider _ziplineTrigger = null;

        private ZiplineHandler _ziplineHandler = null;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;

        public Transform ZiplineStart => _ziplineStart;
        public Transform ZiplineTarget => _ziplineTarget;

        private void Awake()
        {
            _startPosition = _ziplineStart.position;
            _targetPosition = _ziplineTarget.position;

            // Place collider at the center of zipline
            _ziplineTrigger.transform.position = new Vector3(0f, (_startPosition.y + _targetPosition.y) / 2, (_startPosition.z + _targetPosition.z) / 2);
            _ziplineTrigger.transform.rotation = Quaternion.AngleAxis(Vector3.Angle(_startPosition, _targetPosition) / 2, _ziplineTrigger.transform.right);
            _ziplineTrigger.size = new Vector3(_ziplineTrigger.size.x, _ziplineTrigger.size.y, Vector3.Distance(_startPosition, _targetPosition));
        }

        public void PlayerOnZipline(PhysicsTriggerEvent sender, Collider other)
        {
            _ziplineHandler = other.GetComponentInParent<ZiplineHandler>();
            Rigidbody otherRigidbody = other.GetComponentInParent<Rigidbody>();
            if (_ziplineHandler != null && otherRigidbody.velocity.y < 0)
            {
                _ziplineHandler.SetZipline(this);
            }
        }

        public void PlayerNotOnZipline(PhysicsTriggerEvent sender, Collider other)
        {
            _ziplineHandler = other.GetComponentInParent<ZiplineHandler>();
            if (_ziplineHandler != null)
            {
                _ziplineHandler.LeaveZipline(this);
            }
        }
    }
}