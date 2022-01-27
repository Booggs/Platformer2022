namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using GSGD2.Player;

    public class DebugHUD : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _debugText = null;
        [SerializeField]
        private CubeController _cubeController = null;
        [SerializeField]
        private CharacterCollision _characterCollision = null;

        void Update()
        {
            _debugText.text = _cubeController.Rigidbody.velocity.ToString();
            //_debugText.text = _cubeController.CurrentState.ToString();
        }
    }
}