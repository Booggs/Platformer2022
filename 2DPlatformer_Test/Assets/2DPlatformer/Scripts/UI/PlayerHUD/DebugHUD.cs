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
        private PlayerController _playerController = null;
        [SerializeField]
        private CharacterCollision _characterCollision = null;
        [SerializeField]
        private Rotator _rotator = null;
        [SerializeField]
        private SlingshotHandler _slingshotHandler = null;

        void Update()
        {
            //_debugText.text = _slingshotHandler.SlingshotTimer.TimeElapsed.ToString("F3");
            _debugText.text = _cubeController.CurrentState.ToString();
        }
    }
}