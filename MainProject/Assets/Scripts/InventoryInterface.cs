using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryInterface : MonoBehaviour
{
    [SerializeField] protected Vector2Int _inventorySize;

    protected InventorySlot[] _slots;

    public Vector2Int InventorySize { get => _inventorySize; }

    protected InventoryData _owner;
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

    public virtual Item this[Vector2Int index]
    {
        get => Owner[index];
        set
        {
            Owner[index] = value;
            UpdateInterface();
        }
    }

    public virtual Item this[int x, int y]
    {
        get => Owner[x, y];
        set
        {
            Owner[x, y] = value;
            UpdateInterface();
        }
    }

    public void Clear()
    {
        Owner.Clear();
        UpdateInterface();
    }

    public bool AddItem(Item item)
    {
        bool retVal = Owner.AddItem(item);
        UpdateInterface();
        return retVal;
    }

    public bool AppendInventory(InventoryInterface inventory)
    {
        bool retVal = Owner.AppendInventory(inventory.Owner);
        UpdateInterface(); inventory.UpdateInterface();
        return retVal;
    }

    public bool IsCompatibleInterface(InventoryData inventory)
    {
        return inventory != null && (inventory.InventorySize == InventorySize);
    }

    protected void Awake()
    {
        _slots = GetComponentsInChildren<InventorySlot>();
        for (int i = 0; i < _slots.Length; ++i)
        {
            _slots[i].OwnerInventory = this;
        }
    }

    protected void OnEnable()
    {
        UpdateInterface();
    }

    protected void UpdateInterface()
    {
        if (Owner == null)
            return;

        for(int i = 0; i < _slots.Length; ++i)
        {
            _slots[i].SlotItem = Owner[_slots[i].SlotIndex];
        }
    }
}
