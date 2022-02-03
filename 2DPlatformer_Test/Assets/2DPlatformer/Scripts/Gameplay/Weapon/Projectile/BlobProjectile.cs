namespace GSGD2.Gameplay
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GSGD2.Player;
	using GSGD2.Extensions;

	/// <summary>
	/// Base class for non-physics driven projectile. Move forward at given <see cref="_speed"/>.
	/// It handle redirection from <see cref="RedirectProjectileListener"/>.
	/// </summary>
	/// 
	public class BlobProjectile : Projectile
	{
		[SerializeField]
		private HealthBlob _healthBlobPrefab = null;

        protected override void OnDestroyOnCollisionEnter(PhysicsTriggerEvent triggerEvent, Collider other)
        {
			if (other.isTrigger == false && _destroyOnCollision == true)
			{
				Instantiate<HealthBlob>(_healthBlobPrefab, transform.position, Quaternion.identity);
				_gameObjectDestroyer.Destroy();
			}
		}
    }
}