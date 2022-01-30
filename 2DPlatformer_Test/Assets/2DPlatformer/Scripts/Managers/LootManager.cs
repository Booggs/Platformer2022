namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LootManager : MonoBehaviour
    {
        private int _currentLoot = 50;

        public int CurrentLoot
        {
            get
            {
                return _currentLoot;
            }
            set
            {
                _currentLoot = value;
            }
        }

        public delegate void LootManagerEvent(LootManager sender, int currentLoot);
        public event LootManagerEvent LootAdded = null;

        public void AddLoot(int value)
        {
            _currentLoot += value;

            LootAdded?.Invoke(this, _currentLoot);
        }
    }
}