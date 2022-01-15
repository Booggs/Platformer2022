namespace GSGD2.Gameplay
{
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// PickupCommand used to modify player health. It can be workarounded to poison the player, but it shouldn't be used that way, since it will not call TakeDamage() and trigger the chain of events.
	/// </summary>
	[CreateAssetMenu(menuName = "GameSup/LootCommand", fileName = "LootCommand")]
	public class LootCommand : PickupCommand
	{
		[SerializeField]
		private int _lootValue = 1;

		protected override bool ApplyPickup(ICommandSender from)
		{
			LevelReferences.Instance.LootManager.AddLoot(_lootValue);
			return true;
		}
	}
}