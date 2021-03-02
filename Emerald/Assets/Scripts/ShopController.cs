﻿using System.Collections.Generic;
using Aura2API;
using TMPro;
using UiControllers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using C = ClientPackets;
using Image = UnityEngine.UI.Image;
using Network = Emerald.Network;
using S = ServerPackets;

public class ShopController : MonoBehaviour {
    [SerializeField] private GameObject shopWindow;
    [SerializeField] private GameObject shopItemContainer;
    [SerializeField] private GameObject shopItem;
    [SerializeField] private GameObject npcDialogue;
    [SerializeField] private TextMeshProUGUI shopPageText;
    [SerializeField] private GameObject inventoryWindow; // 125 100
    [SerializeField] private ShopWindowController shopWindowController; 
    [SerializeField] private Texture2D repairCursor;
    [SerializeField] internal GameSceneManager gameManager; // 125 100
    private UserItem currentHoveredItem;

    
    private int currentItemPage;
    private List<UserItem> goods = new List<UserItem>();

    private Vector3 inventorySavedPosition;

    public bool IsRepairOptionSelected { get; private set; }

    public void Start()
    {
        shopWindowController.ShopController = this;
        repairCursor.Resize(20, 20);
    }

    public void SetIsRepairingOption()
    {
        IsRepairOptionSelected = !IsRepairOptionSelected;
        if (IsRepairOptionSelected)
        {
            Cursor.SetCursor(repairCursor, new Vector2(20, 20), CursorMode.Auto);
        }
    }

    private readonly List<GameObject> shopItemContainers = new List<GameObject>();

    public bool IsShopWindowOpen()
    {
        return shopWindow.activeSelf;
    }

    private void CmdSellItem(UserItem item)
    {
        Network.Enqueue(new C.SellItem() {UniqueID = item.UniqueID, Count = item.Count});
    }
    private void CmdBuyItem(UserItem item) =>
        Network.Enqueue(new C.BuyItem() { ItemIndex = item.UniqueID, Count = 1, Type = PanelType.Buy});
    
    private void CmdBuyItem(ulong itemUniqueId, uint count) =>
        Network.Enqueue(new C.BuyItem() { ItemIndex = itemUniqueId, Count = count, Type = PanelType.Buy});

    private void CmdRepairItem(UserItem item) =>
        Network.Enqueue(new C.RepairItem() { UniqueID = item.UniqueID });

    private void CmdSpecialRepairItem(UserItem item) =>
        Network.Enqueue(new C.SRepairItem() {UniqueID = item.UniqueID});
    
    
    public void SetNpcGoods(List<UserItem> shopItems) {
        shopWindowController.SetInitialNpcGoods(shopItems);
    }

    public void BuyItem(ulong itemUniqueID, uint count)
    {
        // Check money
        // Check space
        CmdBuyItem(itemUniqueID, count);
    }

    public void SetCurrentHoveredItem(UserItem item)
    {
        this.currentHoveredItem = item;
    }
    public void ResetCurrentHoveredItem(UserItem item)
    {
        if (this.currentHoveredItem == item)
        {
            currentHoveredItem = null;
        }
    }

    public void SellItem(UserItem item)
    {
        CmdSellItem(item);
    }
}
