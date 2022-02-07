namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;

    public class DamageHandler : MonoBehaviour
    {
        [SerializeField]
        private DamageDealer _damageDealer = null;

        [SerializeField]
        private Damage _dashDamage = null;

        [SerializeField]
        private Damage _groundSmashDamage = null;

        [SerializeField]
        private Damage _slingshotDamage = null;

        private int _damageUpgradeLevel = 0;


        private DamageStates _currentState = 0;

        public DamageStates CurrentState => _currentState;
        public int DamageUpgrades => _damageUpgradeLevel;

        public enum DamageStates
        {
            None = 0,
            Dashing = 1,
            GroundSmashing = 2,
            Slingshotting = 3
        }

        public void UpdateState(DamageStates newState)
        {
            if (_currentState != newState)
            {
                _currentState = newState;
                switch (_currentState)
                {
                    case DamageStates.None:
                        {
                            _damageDealer.Damage = null;
                        }
                        break;
                    case DamageStates.Dashing:
                        {
                            _damageDealer.Damage = _dashDamage;
                        }
                        break;
                    case DamageStates.GroundSmashing:
                        {
                            _damageDealer.Damage = _groundSmashDamage;
                        }
                        break;
                    case DamageStates.Slingshotting:
                        {
                            _damageDealer.Damage = _slingshotDamage;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void UpgradeDamage()
        {
            _dashDamage.DamageValue++;
            _groundSmashDamage.DamageValue++;
            _slingshotDamage.DamageValue++;
            _damageUpgradeLevel++;
        }
    }
}