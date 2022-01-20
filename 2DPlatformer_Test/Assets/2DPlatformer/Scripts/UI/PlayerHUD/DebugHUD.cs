namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using GSGD2.Player;

    public class DebugHUD : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _debugText = null;
        [SerializeField]
        private InteractionManager _interactionManager = null;

        void Update()
        {
            string debugText = null;
            if (_interactionManager.CurrentEnvironmentInteractable != null)
                debugText = _interactionManager.CurrentEnvironmentInteractable.name;
            else debugText = null;

            _debugText.text = debugText;
        }
    }
}