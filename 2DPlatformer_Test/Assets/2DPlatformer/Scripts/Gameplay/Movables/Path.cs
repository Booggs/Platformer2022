namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Path : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _path = null;

        private bool _isLoop = false;

        public bool IsLoop => _isLoop;

        public int PathLength => _path.Length;

        //ref enable the possibility to change index inside this function
        public bool TryGetNextWaypoint(ref int index, out Vector3 nextWaypoint)
        {
            if (index >= PathLength)
            {
                if (_isLoop == true)
                {
                    index = 0;
                }
            }

            if (index < PathLength)
            {
                nextWaypoint = _path[index].position;
                return true;
            }
            else
            {
                nextWaypoint = Vector3.zero;
                return false;
            }
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < _path.Length - 1; i++)
            {
                Gizmos.DrawLine(_path[i].position, _path[i + 1].position);
            }
            if (_isLoop)
            {
                Gizmos.DrawLine(_path[_path.Length - 1].position, _path[0].position);
            }
        }
    }
}