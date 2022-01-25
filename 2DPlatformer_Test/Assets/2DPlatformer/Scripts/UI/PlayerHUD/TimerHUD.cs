namespace GSGD2.Utilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using GSGD2.Player;
    using UnityEngine.InputSystem;
    using System;

    public class TimerHUD : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _timerElapsedText = null;
        [SerializeField]
        private TextMeshProUGUI _distanceText = null;
        [SerializeField]
        private TextMeshProUGUI _averageSpeedText = null;
        [SerializeField]
        private CubeController _cubeController = null;
        [SerializeField]
        private PlayerController _playerController = null;
        [SerializeField]
        private Timer _testTimer;

        private Vector3 tmpPreviousLocation = new Vector3(0, 0, 0);
        private string tmpTimerElapsedText = null;
        private string tmpDistanceText = null;
        private string tmpAverageSpeedText = null;
        private float currentMovedDistance = 0;

        private void OnEnable()
        {
            _playerController.StartTimerPerformed -= TimerInput_performed;
            _playerController.StartTimerPerformed += TimerInput_performed;
        }

        private void OnDisable()
        {
            _playerController.StartTimerPerformed -= TimerInput_performed;
        }

        void Update()
        {
            if (_testTimer.IsRunning == true)
            {
                _testTimer.Update();
                tmpTimerElapsedText = "Time elapsed : " + Math.Round(_testTimer.Progress * _testTimer.Duration, 2).ToString() + " s";
                tmpAverageSpeedText = "Current speed : " + Math.Round(_cubeController.Rigidbody.velocity.magnitude, 2).ToString() + " m/s";
                currentMovedDistance += Vector3.Distance(_cubeController.transform.position, tmpPreviousLocation);
                tmpDistanceText = "Distance moved since start of timer : " + Math.Round(currentMovedDistance, 2).ToString() + " m";
            }
            _timerElapsedText.text = tmpTimerElapsedText;
            _averageSpeedText.text = tmpAverageSpeedText;
            _distanceText.text = tmpDistanceText;
            tmpPreviousLocation = _cubeController.transform.position;
        }

        private void TimerInput_performed(PlayerController playerController, InputAction.CallbackContext obj)
        {
            if (_testTimer.IsRunning == false)
            {
                _testTimer.Start();
                tmpPreviousLocation = _cubeController.transform.position;
                currentMovedDistance = 0f;
            }
            else _testTimer.ForceFinishState();
        }
    }
}