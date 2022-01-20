namespace GSGD2.Gameplay
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using GSGD2.Player;
    using UnityEditor;

    public class Shop : AEnvironmentInteractable
    {
        [SerializeField]
        private GameObject _canvasParent = null;

        private LootManager _lootManager = null;
        private ShopItem[] _shopItems = null;
        private bool _shopOpened = false;
        private int _currentItemIndex = 0;

        private void Awake()
        {
            _lootManager = LevelReferences.FindObjectOfType<LootManager>();
            _shopItems = GetComponentsInChildren<ShopItem>(); //Be sure to get shop item list BEFORE the shop items are disabled by the CloseShop() function
            if (_shopItems.Length > 1)
            {
                for (int i = 1; i < _shopItems.Length; i++)
                {
                    _shopItems[i].canvasGroup.alpha = 0f;
                }
            }
            CloseShop();
        }


        #region InteractionInputMethods
        public override void UseInteractable(PlayerReferences playerRefs)
        {
            if (_shopOpened == false)
            {
                OpenShop();
            }
            else UseShop(playerRefs);
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

        private void OpenShop()
        {
            if (_interactableActive == true)
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
                    if (_shopItems.Length > 1)
                    {
                        shopItem.gameObject.SetActive(false);
                        ArrayUtility.RemoveAt<ShopItem>(ref _shopItems, _currentItemIndex);
                        if (_currentItemIndex > 0)
                        {
                            _currentItemIndex--;
                        }
                        else _currentItemIndex = _shopItems.Length - 1;

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
            else _currentItemIndex = _shopItems.Length - 1;

            CheckItemAvailability();
        }

        private void NavigateRight()
        {
            _shopItems[_currentItemIndex].canvasGroup.alpha = 0f;

            if (_currentItemIndex < _shopItems.Length - 1)
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