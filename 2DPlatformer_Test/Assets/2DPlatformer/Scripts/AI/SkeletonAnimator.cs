namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Utilities;

    public class SkeletonAnimator : MonoBehaviour
    {
        [SerializeField]
        private Skeleton _skeleton = null;

        [SerializeField]
        private Animator _animator = null;

        private Vector3 _previousPosition = Vector3.zero;
        private float velocity = 0f;

        private void OnEnable()
        {
            _skeleton.StateChanged -= SkeletonStateChanged;
            _skeleton.StateChanged += SkeletonStateChanged;

            _previousPosition = transform.position;
        }

        private void OnDisable()
        {
            _skeleton.StateChanged -= SkeletonStateChanged;
        }

        private void Update()
        {
            velocity = ((transform.position - _previousPosition).magnitude) / Time.deltaTime;
            _previousPosition = transform.position;
            float value = Mathf.Abs(velocity);
            _animator.SetFloat("IdleRunBlend", value);
        }

        private void SkeletonStateChanged(Skeleton skeleton, Skeleton.EnemyState newState)
        {
            switch (newState)
            {
                case Skeleton.EnemyState.Idle:
                    break;
                case Skeleton.EnemyState.Pursuing:
                    {
                        _animator.SetTrigger("Pursue");
                    }
                    break;
                case Skeleton.EnemyState.Attacking:
                    {
                        _animator.SetTrigger("Attack");
                    }
                    break;
                case Skeleton.EnemyState.AttackingCooldown:
                    {
                        _animator.SetTrigger("Cooldown");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}