using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryData
{
    private Item[] _inventory;

    [SerializeField] private Vector2Int _inventorySize;
    public Vector2Int InventorySize { get => _inventorySize; }

    public Item this[int x, int y]
    {
        get => _inventory[x * InventorySize.y + y];
        set => _inventory[x * InventorySize.y + y] = value;
    }

    public Item this[Vector2Int index]
    {
        get => _inventory[index.x * InventorySize.y + index.y];
        set => _inventory[index.x * InventorySize.y + index.y] = value;
    }

    public bool AddItem(Item item)
    {
        int index = Array.FindIndex(_inventory, (i) => i == null);

        if (index == -1)
            return false;

        _inventory[index] = item;
        return true;
    }

    public bool AppendInventory(InventoryData data)
    {
        for(int i = 0; i < data._inventory.Length; ++i)
        {
            if (!AddItem(data._inventory[i]))
                return false;
        }
        return true;
    }

    public void Clear()
    {
        _inventory = new Item[_inventorySize.x * _inventorySize.y];
    }

    public InventoryData(Vector2Int size)
    {
        _inventorySize = size;
        _inventory = new Item[size.x * size.y];
    }
}
