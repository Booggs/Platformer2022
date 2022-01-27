namespace GSGD2.Player
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ScaleUpdator : MonoBehaviour
    {
        [SerializeField]
        private PlayerReferences _playerRefs = null;

        [SerializeField]
        private GameObject _sphereParent = null;

        private CubeController _cubeController = null;
        private BoneSphere _boneSphere = null;
        private CharacterCollision _characterCollision = null;
        private float _currentScale = 1f;

        private void Awake()
        {
            _playerRefs.TryGetCubeController(out _cubeController);
            _playerRefs.TryGetBoneSphere(out _boneSphere);
            _playerRefs.TryGetCharacterCollision(out _characterCollision);
        }

        public void UpdateScale()
        {
            UpdateCubeController();
            UpdateBoneSphere();
            UpdateCharacterCollision();
        }


        private void UpdateCubeController()
        {

        }

        private void UpdateBoneSphere()
        {

        }

        private void UpdateCharacterCollision()
        {

        }
    }
}