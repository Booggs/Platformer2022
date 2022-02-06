namespace GSGD2.UI
{
	using GSGD2.Gameplay;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using GSGD2.Player;
	using GSGD2.Utilities;

	/// <summary>
	/// Subsystem that handle the player health bar. It seek player <see cref="Damageable"/>, listen to its <see cref="Damageable.DamageTaken"/>  and <see cref="Damageable.HealthRestored"\> event and react accordingly.
	/// </summary>
	public class StaminaBarHUDMenu : MonoBehaviour
	{
		[SerializeField]
		private Image _staminaBarForeground = null;

		[SerializeField]
		private CanvasGroup _canvasGroup = null;

		[SerializeField]
		private Timer _fadingTimer;

		private CubeController _cubeController = null;
		private bool _fadingOut = false;

		private void Awake()
		{
			_cubeController = LevelReferences.Instance.Player.GetComponent<CubeController>();
		}

        private void Update()
        {
			float currentStamina = _cubeController.CurrentStamina;
			if (currentStamina < 1f && _fadingTimer.IsRunning == false && _canvasGroup.alpha == 0f)
            {
				_fadingOut = false;
				_fadingTimer.Start();
            }
			if (currentStamina >= 1f && _fadingTimer.IsRunning == false && _canvasGroup.alpha == 1f)
            {
				_fadingOut = true;
				_fadingTimer.Start();
			}
			if (_fadingTimer.IsRunning)
				_fadingTimer.Update();
			if (_fadingOut)
				_canvasGroup.alpha = 1f - _fadingTimer.Progress;
			else _canvasGroup.alpha = _fadingTimer.Progress;
			_staminaBarForeground.fillAmount = currentStamina;
		}
    }
}