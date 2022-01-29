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
            _currentScale = _playerDamageable.CurrentHealth / 10;
            transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
            UpdateCubeController();
            UpdateBoneSphere();
            UpdateCharacterCollision();
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
    }
}