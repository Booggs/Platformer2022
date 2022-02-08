namespace GSGD2.Utilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using GSGD2.UI;

    public class PauseMenuManager : MonoBehaviour
    {
        [SerializeField]
        private UIManager _uiManager = null;

        [SerializeField]
        private EventSystem _eventSystem = null;

        [SerializeField]
        private GameObject _playButton = null;

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(_playButton);
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        public void Unpause()
        {
            _uiManager.Unpause();
        }
    }
}