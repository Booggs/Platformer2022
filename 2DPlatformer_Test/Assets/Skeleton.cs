namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using GSGD2.Utilities;

    public class Skeleton : MonoBehaviour
    {
        [SerializeField]
        private Timer _playerLossTimer;

        [SerializeField]
        private Timer _attackCooldown;


        [Header("Patrolling")]
        [SerializeField]
        private bool _patrol = false;

        [SerializeField]
        private Transform[] _patrolPoints = new Transform[2];

        [SerializeField]
        private Timer _patrolWait;

        private int _currentPatrolIndex = 0;


        [Header("References")]
        [SerializeField]
        private NavMeshAgent _navMeshAgent = null;

        [SerializeField]
        private Raycaster _raycaster = null;

        [SerializeField]
        private Animator _skeletonAnimator = null;

        [SerializeField]
        private Collider _damageTrigger = null;


        [Header("READ ONLY")]
        [SerializeField]
        private EnemyState currentState = 0;

        [SerializeField]
        private PlayerDamageable _player = null;

        [SerializeField]
        private bool _seeingPlayer = false;

        [SerializeField]
        private Vector3 destination;

        [SerializeField]
        private float distanceToTarget;

        [SerializeField]
        private bool withinAttackDistance;


        private PlayerDamageable tmpPlayer = null;
        private float _attackingDistance = 2;
        private float _cachedMovementSpeed;
        private EnemyState _currentState = 0;
        private bool _attacking = false;

        public EnemyState CurrentState => _currentState;

        public enum EnemyState
        {
            Idle = 0,
            Pursuing = 1,
            Attacking = 2,
            AttackingCooldown = 3
        }

        public delegate void SkeletonEvent(Skeleton skeleton, EnemyState newState);

        public event SkeletonEvent StateChanged = null;

        private void OnEnable()
        {
            _playerLossTimer.StateChanged -= PlayerLoss_Updated;
            _attackCooldown.StateChanged -= AttackCooldownTimer_Updated;
            _patrolWait.StateChanged -= PatrolTimer_Updated;

            _playerLossTimer.StateChanged += PlayerLoss_Updated;
            _attackCooldown.StateChanged += AttackCooldownTimer_Updated;
            _patrolWait.StateChanged += PatrolTimer_Updated;

            _cachedMovementSpeed = _navMeshAgent.speed;
            _attackingDistance = _navMeshAgent.stoppingDistance;
            _damageTrigger.enabled = false;
        }

        private void OnDisable()
        {
            _playerLossTimer.StateChanged -= PlayerLoss_Updated;
            _attackCooldown.StateChanged -= AttackCooldownTimer_Updated;
            _patrolWait.StateChanged -= PatrolTimer_Updated;
        }

        private void Update()
        {
            CheckForPlayer();
            destination = _navMeshAgent.destination;
            if (_player != null)
            {
                distanceToTarget = Vector3.Distance(_player.transform.position, transform.position);
                withinAttackDistance = Vector3.Distance(_player.transform.position, transform.position) <= _attackingDistance;
            }
            else
            {
                distanceToTarget = 0;
                withinAttackDistance = false;
            }
            currentState = _currentState;

            switch (_currentState)
            {
                case EnemyState.Idle:
                    {
                        if (_patrol == true && _patrolPoints.Length >= 2)
                        {
                            _navMeshAgent.destination = _patrolPoints[_currentPatrolIndex].transform.position;
                            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                            {
                                if ((!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f) && _patrolWait.IsRunning == false)
                                {
                                    _patrolWait.Start();
                                }
                            }
                        }
                    }
                    break;
                case EnemyState.Pursuing:
                    {
                        if (_player != null)
                        {
                            _navMeshAgent.destination = _player.transform.position;
                            if (Vector3.Distance(_player.transform.position, transform.position) <= _attackingDistance)
                            {
                                Attack();
                            }
                        }
                        else ChangeState(EnemyState.Idle);
                    }
                    break;
                case EnemyState.Attacking:
                    break;
                case EnemyState.AttackingCooldown:
                    break;
                default:
                    break;
            }
            _playerLossTimer.Update();
            _attackCooldown.Update();
            _patrolWait.Update();
        }

        private void CheckForPlayer()
        {
            if (_raycaster.BoxcastAll(out RaycastHit[] BoxcastHits, QueryTriggerInteraction.Ignore, false))
            {
                foreach (RaycastHit raycastHit in BoxcastHits)
                {
                    if (raycastHit.collider.GetComponentInParent<PlayerDamageable>() != null)
                    {
                        tmpPlayer = raycastHit.collider.GetComponentInParent<PlayerDamageable>();
                        break;
                    }
                }
                if (tmpPlayer != null)
                {
                    RaycastHit raycastHit;
                    if (Physics.Linecast(_raycaster.WorldPosition, tmpPlayer.transform.position, out raycastHit, _raycaster.LayerMask) && raycastHit.collider.GetComponentInParent<PlayerDamageable>())
                    {
                        _player = tmpPlayer;
                        _playerLossTimer.ResetTimeElapsed();
                        _seeingPlayer = true;
                        if (_currentState == EnemyState.Idle)
                            ChangeState(EnemyState.Pursuing);
                    }
                    else if (_player != null && _playerLossTimer.IsRunning == false)
                    {
                        _playerLossTimer.Start();
                        _seeingPlayer = false;
                    }
                }
            }
        }

        #region Timers
        private void PlayerLoss_Updated(Timer timer, Timer.State state)
        {
            switch (state)
            {
                case Timer.State.Stopped:
                    break;
                case Timer.State.Running:
                    break;
                case Timer.State.Finished:
                    {
                        ChangeState(EnemyState.Idle);
                        _player = null;
                    }
                    break;
                default:
                    break;
            }
        }

        private void AttackCooldownTimer_Updated(Timer timer, Timer.State state)
        {
            switch (state)
            {
                case Timer.State.Stopped:
                    break;
                case Timer.State.Running:
                    break;
                case Timer.State.Finished:
                    {
                        if (_player != null && Vector3.Distance(_player.transform.position, transform.position) <= _attackingDistance)
                        {
                            Attack();
                        }
                        else ChangeState(EnemyState.Pursuing);
                    }
                    break;
                default:
                    break;
            }
        }

        private void PatrolTimer_Updated(Timer timer, Timer.State state)
        {
            switch (state)
            {
                case Timer.State.Stopped:
                    break;
                case Timer.State.Running:
                    break;
                case Timer.State.Finished:
                    {
                        if (_currentPatrolIndex == _patrolPoints.Length - 1)
                            _currentPatrolIndex = 0;
                        else _currentPatrolIndex++;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion;

        private void ChangeState(EnemyState newState)
        {
            _currentState = newState;
            switch (_currentState)
            {
                case EnemyState.Idle:
                    {
                        _attacking = false;
                    }
                    break;
                case EnemyState.Pursuing:
                    {
                        _navMeshAgent.speed = _cachedMovementSpeed;
                        _attacking = false;
                    }
                    break;
                case EnemyState.Attacking:
                    {
                        _navMeshAgent.speed = 0;
                        _attacking = true;
                    }
                    break;
                case EnemyState.AttackingCooldown:
                    {
                        _attacking = false;
                        _attackCooldown.Start();
                    }
                    break;
                default:
                    break;
            }
            StateChanged?.Invoke(this, newState);
        }

        private void Attack()
        {
            ChangeState(EnemyState.Attacking);
        }

        public void AttackEnd()
        {
            ChangeState(EnemyState.AttackingCooldown);
            _damageTrigger.enabled = false;
        }

        public void EnableDamage()
        {
            _damageTrigger.enabled = true;
        }
    }
}