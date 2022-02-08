namespace GSGD2.Utilities
{
	using System.Collections;
	using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif // UNITY_EDITOR

	/// <summary>
	/// Quick class used to handle MainMenu scene.
	/// </summary>
	public class MainMenuManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject _main = null;

		[SerializeField]
		private GameObject _options = null;

		[SerializeField]
		private GameObject _keybindings = null;

		[SerializeField]
		private GameObject _settings = null;


		public void Quit()
		{
#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif //UNITY_EDITOR
		}

		public void ReturnToMainMenuCanvas()
        {
			_main.SetActive(true);
			_options.SetActive(false);
			_keybindings.SetActive(false);
			_settings.SetActive(false);
		}

		public void ReturnToOptionsCanvas()
        {
			_main.SetActive(false);
			_options.SetActive(true);
			_keybindings.SetActive(false);
			_settings.SetActive(false);
		}

		public void OpenKeybindings()
        {
			_main.SetActive(false);
			_options.SetActive(false);
			_keybindings.SetActive(true);
			_settings.SetActive(false);
		}

		public void OpenSettings()
        {
			_main.SetActive(false);
			_options.SetActive(false);
			_keybindings.SetActive(false);
			_settings.SetActive(true);
		}

		public void OpenOptions()
		{
			_main.SetActive(false);
			_options.SetActive(true);
			_keybindings.SetActive(false);
			_settings.SetActive(false);
		}
	}
}