namespace GSGD2.Gameplay
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Conveniance class that can instantiate a particle with its GameObject transform position and rotation.
	/// </summary>
	public class LootInstancier : MonoBehaviour
	{
		[SerializeField]
		private PickupInteractor _lootPickup = null;

		// Simplified command for unity event
		public void DoInstantiateLoot() => Instantiate(out _);

		public void Instantiate(out PickupInteractor lootPickup)
		{
			lootPickup = Instantiate(_lootPickup, transform.position, transform.rotation);
		}
	}
}