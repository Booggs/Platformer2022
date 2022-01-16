namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class LootHUD : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text = null;

        private void OnEnable()
        {
            var lootManager = LevelReferences.Instance.LootManager;
            lootManager.LootAdded -= LootManagerOnLootAdded;
            lootManager.LootAdded += LootManagerOnLootAdded;

            UpdateValues(lootManager.CurrentLoot);
        }

        private void OnDisable()
        {
            if (LevelReferences.HasInstance == true)
            {
                LevelReferences.Instance.LootManager.LootAdded -= LootManagerOnLootAdded;
            }
        }

        private void LootManagerOnLootAdded(LootManager sender, int currentLoot)
        {
            UpdateValues(currentLoot);
        }

        private void UpdateValues(int loot)
        {
            _text.text = loot.ToString();
        }
    }
}