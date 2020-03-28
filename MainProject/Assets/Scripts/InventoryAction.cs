using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryAction
{
    public static InventoryAction CurrentInventoryAction = InventoryIdleAction.instance;

    public abstract void Move(InventorySlot targetSlot);

    public abstract void Swap(InventorySlot targetSlot, InventorySlot lastSlot);

    public abstract void Drop(InventorySlot lastSlot);

    public abstract void Use(InventorySlot lastSlot);
}

public class InventoryIdleAction : InventoryAction
{
    public static InventoryIdleAction instance = new InventoryIdleAction();

    public override void Drop(InventorySlot lastSlot)
    {
        MouseInventory.Instance.DropItem();
    }

    public override void Move(InventorySlot targetSlot)
    {
        targetSlot.AddItem(MouseInventory.Instance.DropItem());
    }

    public override void Swap(InventorySlot targetSlot, InventorySlot lastSlot)
    {
        lastSlot.AddItem(targetSlot.RemoveItem());
        targetSlot.AddItem(MouseInventory.Instance.DropItem());
    }

    public override void Use(InventorySlot lastSlot)
    {
        lastSlot.RemoveItem();
    }
}

public class InventoryCookAction : InventoryIdleAction
{
    public static new InventoryCookAction instance = new InventoryCookAction();

    public override void Drop(InventorySlot lastSlot)
    {
        if(lastSlot.OwnerInventory.GetInventoryType() == EInventoryType.E_PLAYER)
        {
            if(!CookInterface.Instance.IsFull())
            {
                CookInterface.Instance.AddItem(MouseInventory.Instance.DropItem());
            }
            else
            {
                lastSlot.AddItem(MouseInventory.Instance.DropItem());
            }
        }
        else
        {
            if (!PlayerInventory.Instance.IsFull())
            {
                PlayerInventory.Instance.AddItem(MouseInventory.Instance.DropItem());
            }
            else
            {
                lastSlot.AddItem(MouseInventory.Instance.DropItem());
            }
        }
    }
}

public class InventoryDropAction : InventoryIdleAction
{
    public static new InventoryDropAction instance = new InventoryDropAction();

    public override void Drop(InventorySlot lastSlot)
    {
        if (lastSlot.OwnerInventory.GetInventoryType() == EInventoryType.E_PLAYER)
        {
            if (!CookInterface.Instance.IsFull())
            {
                CookInterface.Instance.AddItem(MouseInventory.Instance.DropItem());
            }
            else
            {
                lastSlot.AddItem(MouseInventory.Instance.DropItem());
            }
        }
        else
        {
            if (!PlayerInventory.Instance.IsFull())
            {
                PlayerInventory.Instance.AddItem(MouseInventory.Instance.DropItem());
            }
            else
            {
                lastSlot.AddItem(MouseInventory.Instance.DropItem());
            }
        }
    }
}
