namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TargetBlob : MonoBehaviour
    {
        [SerializeField]
        private bool _isDecoy = false;

        public bool Decoy => _isDecoy;
    }

}