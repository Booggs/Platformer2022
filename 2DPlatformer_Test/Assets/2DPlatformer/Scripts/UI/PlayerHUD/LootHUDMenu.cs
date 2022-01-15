namespace GSGD2.UI
{
	using GSGD2.Gameplay;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	/// <summary>
	/// Subsystem that handle the player health bar. It seek player <see cref="Damageable"/>, listen to its <see cref="Damageable.DamageTaken"/>  and <see cref="Damageable.HealthRestored"\> event and react accordingly.
	/// </summary>
	public class LootHUDMenu : MonoBehaviour
	{
		[SerializeField]
		private TextMeshPro _lootText;

		private LootManager _lootManager = null;

		private void Awake()
		{
			_lootManager = LevelReferences.Instance.Player.GetComponent<LootManager>();
		}
	}
}