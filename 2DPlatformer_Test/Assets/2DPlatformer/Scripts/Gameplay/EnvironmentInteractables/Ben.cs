namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using GSGD2.Player;
    using GSGD2.Utilities;
    using TMPro;

    public class Ben : AEnvironmentInteractable
    {
        [Header("Dialogues")]
        [SerializeField]
        private TextMeshPro _dialogueText = null;

        [SerializeField]
        private GameObject _dialogueBackground = null;

        [SerializeField]
        private Timer _timeBetweenLines;

        [SerializeField]
        private string[] _startingDialogues;

        [SerializeField]
        private string[] _slingshotDialogues;

        [SerializeField]
        private string[] _endingDialogues;

        private int _dialogueIndex = 0;
        private int _dialogueStage = 0;


        [Header("Animations")]
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
            _danceDurationTimer.StateChanged -= DanceDurationTimer_Updated;
            _timeBetweenLines.StateChanged -= DialogueTimer_Updated;

            _danceWaitTimer.StateChanged += DanceWaitTimer_Updated;
            _danceDurationTimer.StateChanged += DanceDurationTimer_Updated;
            _timeBetweenLines.StateChanged += DialogueTimer_Updated;
        }

        private void OnDisable()
        {
            _danceWaitTimer.StateChanged -= DanceWaitTimer_Updated;
            _danceDurationTimer.StateChanged -= DanceDurationTimer_Updated;
            _timeBetweenLines.StateChanged -= DialogueTimer_Updated;
        }

        public void Triggered(PhysicsTriggerEvent sender, Collider other)
        {
            PlayerReferences playerRefs = other.GetComponent<PlayerReferences>();
            if (playerRefs != null)
            {
                _dialogueText.enabled = true;
                _dialogueBackground.SetActive(true);
                if (_waveTimer.IsRunning == false)
                {
                    _animator.SetTrigger("Wave");
                    _waveTimer.Start();
                }
                _danceWaitTimer.Start();

                if (playerRefs.TryGetSlingshotHandler(out SlingshotHandler slingshotHandler) && slingshotHandler.Enabled && _dialogueStage == 0)
                {
                    _dialogueStage = 1;
                }

                if (playerRefs.TryGetBlobSeparation(out BlobSeparation blobSeparation) && blobSeparation.Enabled && _dialogueStage == 1)
                {
                    _dialogueStage = 2;
                }

                if (_timeBetweenLines.IsRunning == false)
                {
                    _timeBetweenLines.Start();
                }
            }
        }

        public void TriggerLeft(PhysicsTriggerEvent sender, Collider other)
        {
            if (other.GetComponent<PlayerReferences>() != null)
            {
                _danceWaitTimer.StopTimer();
                _dialogueText.text = "";
                _dialogueText.enabled = false;
                _dialogueBackground.SetActive(false);
            }
        }

        private void Update()
        {
            _waveTimer.Update();
            _danceWaitTimer.Update();
            _danceDurationTimer.Update();
            _timeBetweenLines.Update();
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
                        _danceWaitTimer.Start();
                    }
                    break;
                default:
                    break;
            }
        }

        private void DialogueTimer_Updated(Timer timer, Timer.State state)
        {
            switch (state)
            {
                case Timer.State.Stopped:
                    break;
                case Timer.State.Running:
                    {
                        if (_dialogueStage == 0)
                        {
                            _dialogueText.text = _startingDialogues[_dialogueIndex];
                        }
                        else if (_dialogueStage == 1)
                        {
                            _dialogueText.text = _slingshotDialogues[_dialogueIndex];
                        }
                        else
                        {
                            _dialogueText.text = _endingDialogues[_dialogueIndex];
                        }
                    }
                    break;
                case Timer.State.Finished:
                    {
                        if (_dialogueStage == 0)
                        {
                            if (_dialogueIndex < _startingDialogues.Length - 1)
                            {
                                _dialogueIndex++;
                                _timeBetweenLines.Start();
                            }
                            else _dialogueIndex = 0;
                        }

                        else if (_dialogueStage == 1)
                        {
                            if (_dialogueIndex < _slingshotDialogues.Length - 1)
                            {
                                _dialogueIndex++;
                                _timeBetweenLines.Start();
                            }
                            else _dialogueIndex = 0;
                        }

                        else
                        {
                            if (_dialogueIndex < _endingDialogues.Length - 1)
                            {
                                _dialogueIndex++;
                                _timeBetweenLines.Start();
                            }
                            else _dialogueIndex = 0;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}