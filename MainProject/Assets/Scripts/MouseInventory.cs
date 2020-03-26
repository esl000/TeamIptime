using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventorySlot))]
public class MouseInventory : InventoryInterface
{
    private static MouseInventory _instance = null;
    public static MouseInventory Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = Instantiate(ResourceManager.GetResource<GameObject>("UI/MouseInventory"),
                    GameObject.FindGameObjectWithTag("MainCanvas").transform)
                    .GetComponent<MouseInventory>();
                _instance.Initialize();
            }
            return _instance;
        }
    }

    public InventorySlot TargetSlot { get; set; }
    public InventorySlot OriginItemSlot { get; private set; }
    public bool IsDrag { get; private set; }

    private void Initialize()
    {
        _inventorySize = Vector2Int.one;
        Owner = new InventoryData(Vector2Int.one);
    }

    public void SelectItem(InventorySlot originSlot)
    {
        Owner[Vector2Int.zero] = originSlot.RemoveItem();
        OriginItemSlot = originSlot;
        UpdateInterface();
        IsDrag = true;
    }

    public Item DropItem()
    {
        Item dropItem = Owner[Vector2Int.zero];
        Owner[Vector2Int.zero] = null;
        IsDrag = false;
        TargetSlot = null;
        UpdateInterface();
        return dropItem;
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }
}
