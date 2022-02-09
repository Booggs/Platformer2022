namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using GSGD2.Player;
    using GSGD2.Utilities;

    public class Ben : MonoBehaviour
    {
        [SerializeField]
        private Animator _animator = null;

        [SerializeField]
        private Timer _waveTimer;

        [SerializeField]
        private Timer _danceWaitTimer;

        [SerializeField]
        private Timer _danceDurationTimer;

        private void OnEnable()
        {
            _danceWaitTimer.StateChanged -= DanceWaitTimer_Updated;
            _danceWaitTimer.StateChanged += DanceWaitTimer_Updated;
            _danceDurationTimer.StateChanged -= DanceDurationTimer_Updated;
            _danceDurationTimer.StateChanged += DanceDurationTimer_Updated;
        }

        private void OnDisable()
        {
            _danceWaitTimer.StateChanged -= DanceWaitTimer_Updated;
            _danceDurationTimer.StateChanged -= DanceDurationTimer_Updated;
        }


        public void Triggered(PhysicsTriggerEvent sender, Collider other)
        {
            if (other.GetComponent<PlayerReferences>() != null)
            {
                if (_waveTimer.IsRunning == false)
                {
                    _animator.SetTrigger("Wave");
                    _waveTimer.Start();
                }
                _danceWaitTimer.Start();
            }
        }

        public void TriggerLeft(PhysicsTriggerEvent sender, Collider other)
        {
            if (other.GetComponent<PlayerReferences>() != null)
            {
                _danceWaitTimer.StopTimer();
            }
        }

        private void Update()
        {
            _waveTimer.Update();
            _danceWaitTimer.Update();
            _danceDurationTimer.Update();
        }

        private void DanceWaitTimer_Updated(Timer timer, Timer.State state)
        {
            switch (state)
            {
                case Timer.State.Stopped:
                    break;
                case Timer.State.Running:
                    break;
                case Timer.State.Finished:
                    {
                        _animator.SetInteger("DanceIndex", Random.Range(0, 4));
                        _animator.SetBool("Dancing", true);
                        _animator.SetTrigger("Dance");
                        _danceDurationTimer.Start();
                    }
                    break;
                default:
                    break;
            }
        }

        private void DanceDurationTimer_Updated(Timer timer, Timer.State state)
        {
            switch (state)
            {
                case Timer.State.Stopped:
                    break;
                case Timer.State.Running:
                    break;
                case Timer.State.Finished:
                    {
                        _animator.SetBool("Dancing", false);
                        _animator.SetTrigger("StopDancing");
                    }
                    break;
                default:
                    break;
            }
        }
    }
}