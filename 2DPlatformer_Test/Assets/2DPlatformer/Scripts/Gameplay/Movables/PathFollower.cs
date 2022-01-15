namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PathFollower : MonoBehaviour
    {
        [SerializeField]
        private Path _path = null;

        /*
        [SerializeField]
        private bool _stopOnEachStep = false;

        [SerializeField]
        private bool _moveOnGameStart = true;
        */

        [SerializeField]
        private float _speed = 10f;

        [SerializeField]
        private float _destinationThreshold = 0.5f;

        private int _currentPathIndex = 0;

        private bool _movingForward = true;

        private void Start()
        {
            transform.position = TryGetNextWaypoint();
        }

        private void Update()
        {
            Vector3 nextWaypoint = TryGetNextWaypoint();

            MoveToNextWaypoint(nextWaypoint);
        }

        //Recursive function : calls itself until condition is resolved
        private Vector3 TryGetNextWaypoint()
        {
            Vector3 nextWaypoint;
            if (_path.TryGetNextWaypoint(ref _currentPathIndex, out nextWaypoint) == true)
            {
                //if I'm near destination
                if (Vector3.Distance(transform.position, nextWaypoint) < _destinationThreshold)
                {
                    // Check for current movement direction, then increment index if going forward or decrement if path end has been reached
                    if (_movingForward == true)
                    {
                        _currentPathIndex++;
                    }
                    else _currentPathIndex--;

                    if (_path.IsLoop == false)
                    {
                        if (_currentPathIndex >= _path.PathLength - 1)
                        { 
                            _movingForward = false;
                        }
                        else if (_currentPathIndex <= 0)
                        {
                            _movingForward = true;
                        }
                    }
                    TryGetNextWaypoint();
                }
            }
            return nextWaypoint;
        }
        private void MoveToNextWaypoint(Vector3 destination)
        {
            transform.position += (destination - transform.position).normalized * _speed * Time.deltaTime;
        }
    }
}