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
        private CubeController _cubeController = null;

        void Update()
        {
            //_debugText.text = "Sticky mode status = " + _cubeController.StickyModeOn.ToString();
        }
    }
}