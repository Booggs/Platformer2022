namespace GSGD2.Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Gameplay;

    public class ScaleUpdator : MonoBehaviour
    {
        [SerializeField]
        private PlayerReferences _playerRefs = null;

        [SerializeField]
        private GameObject _sphereParent = null;

        [SerializeField]
        private GameObject _cameraAimParent = null;

        [SerializeField]
        private float _maxScale = 1.15f;

        [SerializeField]
        private float _minScale = 0.7f;


        private CubeController _cubeController = null;
        private BoneSphere _boneSphere = null;
        private CharacterCollision _characterCollision = null;
        private PlayerDamageable _playerDamageable = null;
        private float _currentScale = 1f;

        private void Awake()
        {
            _playerRefs.TryGetCubeController(out _cubeController);
            _playerRefs.TryGetBoneSphere(out _boneSphere);
            _playerRefs.TryGetCharacterCollision(out _characterCollision);
            _playerRefs.TryGetPlayerDamageable(out _playerDamageable);
        }

        public void UpdateScale()
        {
            float maxHealth = _playerDamageable.MaxHealth;
            float currentHealth = _playerDamageable.CurrentHealth;
            float defaultHealth = _playerDamageable.DefaultHealth;
            float tmpScale;
            if (currentHealth < maxHealth)
            {
                tmpScale = 1f + (currentHealth - defaultHealth) * 0.03f;
            }
            else tmpScale = 1f + (maxHealth - defaultHealth) * 0.03f;


            if (tmpScale > _maxScale)
            {
                tmpScale = _maxScale;
            }
            else if (tmpScale < _minScale)
            {
                tmpScale = _minScale;
            }
            if (_currentScale == tmpScale)
                return;
            _currentScale = tmpScale;
            transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f * _currentScale, transform.position.z);
            UpdateCubeController();
            UpdateBoneSphere();
            UpdateCharacterCollision();
            UpdateCameraDistance();
        }

        private void UpdateCubeController()
        {
            _cubeController.UpdateScale(_currentScale);
        }

        private void UpdateBoneSphere()
        {
            _boneSphere.UpdateOnScaleChange(_currentScale);
        }

        private void UpdateCharacterCollision()
        {
            _characterCollision.UpdateScale(_currentScale);
        }

        private void UpdateCameraDistance()
        {
            float sign = 1;
            if (_currentScale < 1)
            {
                sign = -1;
            }
            _cameraAimParent.transform.localPosition = Vector3.zero;
            _cameraAimParent.transform.localPosition = new Vector3((1 - _currentScale) * 10 * sign, _cameraAimParent.transform.localPosition.y, _cameraAimParent.transform.localPosition.z);
        }
    }
}