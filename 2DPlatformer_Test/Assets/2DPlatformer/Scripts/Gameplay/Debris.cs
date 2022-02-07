namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Utilities;

    public class Debris : MonoBehaviour
    {
        [SerializeField]
        private Timer _lifetime;

        private void Awake()
        {
            _lifetime.Start();
        }

        private void Update()
        {
            _lifetime.Update();
            if (_lifetime.IsRunning == false)
            {
                Destroy(gameObject);
            }
        }
    }

}