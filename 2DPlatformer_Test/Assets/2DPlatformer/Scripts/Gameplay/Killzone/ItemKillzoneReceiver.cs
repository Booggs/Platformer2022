namespace GSGD2.Gameplay
{
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// Item version of <see cref="AKillZoneReceiver"/>
	/// </summary>
	public class ItemKillzoneReceiver : AKillZoneReceiver
	{
		[SerializeField]
		private Item _item = null;

		[SerializeField]
		private bool _shouldDestroyItem = false;

		public override void OnEnterKillzone(Killzone killzone)
		{
			if (_shouldDestroyItem)
				Destroy(gameObject);
			else _item.ResetWorldPosition();
		}
	}
}