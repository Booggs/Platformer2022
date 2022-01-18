namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;
    using TMPro;

    public class ShopItem : MonoBehaviour, ICommandSender
    {
        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI _itemNameDisplay = null;
        [SerializeField]
        private TextMeshProUGUI _itemCostDisplay = null;

        [Header("Item Data")]
        [SerializeField]
        private string _itemName = null;
        [SerializeField]
        private int _itemCost = 0;
        [SerializeField]
        private PickupCommand _pickupCommand = null;
        [SerializeField]
        private int _buyLimit;

        private int _boughtAmount = 0;

        private CanvasGroup _canvasGroup = null;


        public CanvasGroup canvasGroup => _canvasGroup;
        public int ItemCost => _itemCost;
        public PickupCommand pickupCommand => _pickupCommand;
        public int BuyLimit => _buyLimit;
        public int BoughtAmount => _boughtAmount;

        private void Awake()
        {
            _pickupCommand.DestroyPickupOnApply = false;
            _canvasGroup = GetComponent<CanvasGroup>();
            _itemNameDisplay.text = _itemName;
            _itemCostDisplay.text = "Cost : " + _itemCost;
        }

        public void BuyItem()
        {
            _pickupCommand.Apply(this);
            _boughtAmount++;
        }

        GameObject ICommandSender.GetGameObject() => gameObject;
    }
}