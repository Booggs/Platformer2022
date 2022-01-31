namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BlobItem : Item
    {
		public override void ChangeState(State newState)
		{
            base.ChangeState(newState);
            switch(newState)
            {
				case State.Idle:
				case State.Highlighted:
				case State.Held:
					{
                        SpawnedBlob spawnedBlob = GetComponent<SpawnedBlob>();
                        if (spawnedBlob != null)
                        {
                            spawnedBlob.KillBlob();
                        }
					}
                    break;
                default:
                    break;
			}

        }
	}

}