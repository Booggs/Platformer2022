namespace GSGD2.UI
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.InputSystem;
	using GSGD2.Player;
	using GSGD2.Utilities;

	/// <summary>
	/// Manager class that handle global functionnality around UI. It is a proxy to UI subsystem and can enable or disable them.
	/// </summary>
	public class UIManager : MonoBehaviour
	{
		[SerializeField]
		private Canvas _mainCanvas = null;

		[SerializeField]
		private PlayerHUDMenu _playerHUD = null;

		[SerializeField]
		private PauseMenuManager _pauseMenu = null;

		private bool _paused = false;
		private PlayerController _playerController = null;

		public Canvas MainCanvas => _mainCanvas;
		public PlayerHUDMenu PlayerHUD => _playerHUD;

        private void Awake()
        {
			LevelReferences.Instance.PlayerReferences.TryGetPlayerController(out _playerController);
        }

        private void OnEnable()
        {
			_playerController.PauseMenuPerformed -= PauseMenuToggle;
			_playerController.PauseMenuPerformed += PauseMenuToggle;
		}

        private void OnDisable()
        {
			_playerController.PauseMenuPerformed -= PauseMenuToggle;
		}

        public void ShowPlayerHUD(bool isActive)
		{
			_playerHUD.SetActive(isActive);
		}

		public void PauseMenuToggle(PlayerController sender, InputAction.CallbackContext obj)
        {
			if (_paused == false)
            {
				_paused = true;
				ShowPlayerHUD(false);
				_pauseMenu.gameObject.SetActive(true);
            }
			else
            {
				_paused = false;
				ShowPlayerHUD(true);
				_pauseMenu.gameObject.SetActive(false);
			}
        }

		public void Unpause()
        {
			_paused = false;
			ShowPlayerHUD(true);
			_pauseMenu.gameObject.SetActive(false);
		}
	}
}