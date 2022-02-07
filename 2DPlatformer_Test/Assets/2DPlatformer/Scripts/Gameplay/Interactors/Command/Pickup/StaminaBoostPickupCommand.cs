namespace GSGD2.Gameplay
{
	using System.Collections;
	using UnityEngine;
	using GSGD2.Player;

	/// <summary>
	/// PickupCommand used to modify player health. It can be workarounded to poison the player, but it shouldn't be used that way, since it will not call TakeDamage() and trigger the chain of events.
	/// </summary>
	[CreateAssetMenu(menuName = "GameSup/StaminaBoostPickupCommand", fileName = "StaminaBoostPickupCommand")]
	public class StaminaBoostPickupCommand : PickupCommand
	{
		protected override bool ApplyPickup(ICommandSender from)
		{
			if (LevelReferences.Instance.PlayerReferences.TryGetCubeController(out CubeController cubeController) == true)
			{
				cubeController
				return true;
			}
			return false;
		}
	}
}