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

        [SerializeField]
        private GameObject _keybindingsMenu = null;

        [SerializeField]
        private GameObject _pauseMenu = null;

        [SerializeField]
        private GameObject _returnButton = null;


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

        public void OpenKeybindings()
        {
            _keybindingsMenu.SetActive(true);
            _pauseMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(_returnButton);
        }

        public void ReturnToPause()
        {
            _keybindingsMenu.SetActive(false);
            _pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_playButton);
        }
    }
}