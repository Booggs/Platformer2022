namespace GSGD2.Gameplay
{
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// PickupCommand used to modify player health. It can be workarounded to poison the player, but it shouldn't be used that way, since it will not call TakeDamage() and trigger the chain of events.
	/// </summary>
	[CreateAssetMenu(menuName = "GameSup/DamageBoostPickupCommand", fileName = "DamageBoostPickupCommand")]
	public class DamageBoostPickupCommand : PickupCommand
	{
		protected override bool ApplyPickup(ICommandSender from)
		{
			if (LevelReferences.Instance.PlayerReferences.TryGetDamageHandler(out DamageHandler damageHandler) == true)
			{
				damageHandler.UpgradeDamage();
				return true;
			}
			return false;
		}
	}
}