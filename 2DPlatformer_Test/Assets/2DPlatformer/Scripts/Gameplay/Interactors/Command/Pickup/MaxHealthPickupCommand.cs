namespace GSGD2.Gameplay
{
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// PickupCommand used to modify player health. It can be workarounded to poison the player, but it shouldn't be used that way, since it will not call TakeDamage() and trigger the chain of events.
	/// </summary>
	[CreateAssetMenu(menuName = "GameSup/MaxHealthPickupCommand", fileName = "MaxHealthPickupCommand")]
	public class MaxHealthPickupCommand : PickupCommand
	{
		[SerializeField]
		private int _healthToAdd = 1;

		protected override bool ApplyPickup(ICommandSender from)
		{
			if (LevelReferences.Instance.PlayerReferences.TryGetPlayerDamageable(out PlayerDamageable playerDamageable) == true)
			{
				if (_healthToAdd > 0)
				{
					return playerDamageable.AddMaxHealth(_healthToAdd);
				}
			}
			return false;
		}
	}
}