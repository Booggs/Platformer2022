namespace GSGD2.UI
{
	using GSGD2.Gameplay;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using GSGD2.Player;

	/// <summary>
	/// Subsystem that handle the player health bar. It seek player <see cref="Damageable"/>, listen to its <see cref="Damageable.DamageTaken"/>  and <see cref="Damageable.HealthRestored"\> event and react accordingly.
	/// </summary>
	public class StaminaBarHUDMenu : MonoBehaviour
	{
		[SerializeField]
		private Image _healthbarForeground = null;

		private CubeController _cubeController = null;

		private void Awake()
		{
			_cubeController = LevelReferences.Instance.Player.GetComponent<CubeController>();
		}

        private void Update()
        {
			_healthbarForeground.fillAmount = _cubeController.CurrentStamina;
		}
    }
}