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
	public class SkeletonHealthBar : MonoBehaviour
	{
		[SerializeField]
		private Image _healthBarForeground = null;

		[SerializeField]
		private CanvasGroup _canvasGroup = null;

		[SerializeField]
		private Timer _fadingTimer;

		private EnemyDamageable _damageable = null;
		private bool _fadingOut = false;

		private void Awake()
		{
			_damageable = GetComponentInParent<EnemyDamageable>();
		}

        private void Update()
        {
			transform.rotation = Quaternion.Euler(0f, 270f, 0f);
			float currentHealthPerc = _damageable.CurrentHealth / _damageable.MaxHealth;
			if (currentHealthPerc < 1f && _fadingTimer.IsRunning == false && _canvasGroup.alpha == 0f)
            {
				_fadingOut = false;
				_fadingTimer.Start();
            }
			if (currentHealthPerc >= 1f && _fadingTimer.IsRunning == false && _canvasGroup.alpha == 1f)
            {
				_fadingOut = true;
				_fadingTimer.Start();
			}
			if (_fadingTimer.IsRunning)
				_fadingTimer.Update();
			if (_fadingOut)
				_canvasGroup.alpha = 1f - _fadingTimer.Progress;
			else _canvasGroup.alpha = _fadingTimer.Progress;
			_healthBarForeground.fillAmount = currentHealthPerc;
		}
    }
}