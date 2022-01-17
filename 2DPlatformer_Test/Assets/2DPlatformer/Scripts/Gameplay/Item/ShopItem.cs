namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GSGD2.Player;

    public class ShopItem : MonoBehaviour, ICommandSender
    {
        [SerializeField]
        private PickupCommand _pickupCommand = null;

        [SerializeField]
        private int _itemCost = 5;

        private CanvasGroup _canvasGroup = null;


        public CanvasGroup canvasGroup => _canvasGroup;
        public int ItemCost => _itemCost;

        private void Awake()
        {
            _pickupCommand._destroyPickupOnApply = false;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void BuyItem()
        {
            _pickupCommand.Apply(this);
            print("buying item");
        }

        GameObject ICommandSender.GetGameObject() => gameObject;
    }
}