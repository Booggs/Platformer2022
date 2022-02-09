namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;

    public class Shop : AEnvironmentInteractable
    {
        [SerializeField]
        private GameObject _canvasParent = null;

        private LootManager _lootManager = null;
        private List<ShopItem> _shopItems = new List<ShopItem>();
        private bool _shopOpened = false;
        private int _currentItemIndex = 0;

        private void Awake()
        {
            _lootManager = LevelReferences.FindObjectOfType<LootManager>();
            //Be sure to get shop item list BEFORE the shop items are disabled by the CloseShop() function
            foreach (ShopItem shopItem in GetComponentsInChildren<ShopItem>())
            {
                _shopItems.Add(shopItem);
            }
            if (_shopItems.Count > 1)
            {
                for (int i = 1; i < _shopItems.Count; i++)
                {
                    _shopItems[i].canvasGroup.alpha = 0f;
                }
            }
            CloseShop();
        }


        #region InteractionInputMethods
        public override void UseInteractable()
        {
            if (_shopOpened == false)
            {
                OpenShop();
            }
            else UseShop(_playerRefs);
        }

        public override void LeaveInteractable()
        {
            if (_shopOpened == true)
            {
                CloseShop();
            }
        }

        public override void NavigateLeftInteractable()
        {
            if (_shopOpened == true)
            {
                NavigateLeft();
            }
        }

        public override void NavigateRightInteractable()
        {
            if (_shopOpened == true)
            {
                NavigateRight();
            }
        }
        #endregion

        public void OpenShop()
        {
            if (_unlockedAtStart == true)
            {
                _shopOpened = true;
                _canvasParent.SetActive(true);
                CheckItemAvailability();
            }
        }

        private void UseShop(PlayerReferences playerRefs)
        {
            ShopItem shopItem = _shopItems[_currentItemIndex];
            if (shopItem.ItemCost <= _lootManager.CurrentLoot)
            {
                shopItem.BuyItem();
                _lootManager.AddLoot(-shopItem.ItemCost);

                if (shopItem.BuyLimit > 0 && shopItem.BoughtAmount >= shopItem.BuyLimit)
                {
                    if (_shopItems.Count > 1)
                    {
                        shopItem.gameObject.SetActive(false);

                        _shopItems.RemoveAt(_currentItemIndex);
                        if (_currentItemIndex > 0)
                        {
                            _currentItemIndex--;
                        }
                        else _currentItemIndex = _shopItems.Count - 1;

                        CheckItemAvailability();
                    }
                    else
                    {
                        ActivateInteractable(false);
                        CloseShop();
                    }
                }
                CheckItemAvailability();
            }
        }

        private void CloseShop()
        {
            _shopOpened = false;
            _canvasParent.SetActive(false);
        }

        private void NavigateLeft()
        {
            _shopItems[_currentItemIndex].canvasGroup.alpha = 0f;

            if (_currentItemIndex > 0)
            {
                _currentItemIndex--;
            }
            else _currentItemIndex = _shopItems.Count - 1;

            CheckItemAvailability();
        }

        private void NavigateRight()
        {
            _shopItems[_currentItemIndex].canvasGroup.alpha = 0f;

            if (_currentItemIndex < _shopItems.Count - 1)
            {
                _currentItemIndex++;
            }
            else _currentItemIndex = 0;

            CheckItemAvailability();
        }

        private void CheckItemAvailability()
        {
            float tmpAlpha = 1f;
            if (_shopItems[_currentItemIndex].ItemCost > _lootManager.CurrentLoot)
            {
                tmpAlpha = 0.5f;
            }
            else
            {
                tmpAlpha = 1f;
            }
            _shopItems[_currentItemIndex].canvasGroup.alpha = tmpAlpha;
        }
    }
}