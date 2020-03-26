using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public string GUID { get; private set; }
    public string Name { get; private set; }
    public string Lore { get; private set; }

    public Sprite Icon { get; private set; }
    //3d model, 2d sprite
}

[Serializable]
public class InventoryData
{
    private Item[,] _inventory;

    [SerializeField] private Vector2Int _inventorySize;
    public Vector2Int InventorySize { get => _inventorySize; }

    public Item this[int x, int y]
    {
        get => _inventory[x, y];
        set => _inventory[x, y] = value;
    }

    public Item this[Vector2Int index]
    {
        get => _inventory[index.x, index.y];
        set => _inventory[index.x, index.y] = value;
    }

    public InventoryData(Vector2Int size)
    {
        _inventorySize = size;
        _inventory = new Item[size.x, size.y];
    }
}

public class InventoryInterface : MonoBehaviour
{
    [SerializeField] private Vector2Int _inventorySize;

    InventorySlot[] _slots;

    public Vector2Int InventorySize { get => _inventorySize; }

    private InventoryData _owner;
    public InventoryData Owner
    {
        get => _owner;
        set
        {
            if(_owner != value && IsCompatibleInterface(value))
            {
                _owner = value;
                UpdateInterface();
            }
        }
    }

    public bool IsCompatibleInterface(InventoryData inventory)
    {
        return inventory.InventorySize == InventorySize;
    }

    private void Awake()
    {
        _slots = GetComponentsInChildren<InventorySlot>();
    }

    private void OnEnable()
    {
        UpdateInterface();
    }

    void UpdateInterface()
    {
        for(int i = 0; i < _slots.Length; ++i)
        {
            _slots[i].SlotItem = Owner[_slots[i].SlotIndex];
        }
    }
}
