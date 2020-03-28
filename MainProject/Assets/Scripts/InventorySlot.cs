using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int SlotIndex;

    Image _icon;

    Item _slotItem;

    public InventoryInterface OwnerInventory { get; set; }

    public Item SlotItem
    {
        get => _slotItem;
        set
        {
            _slotItem = value;
            UpdateIcon();
        }
    }

    void UpdateIcon()
    {
        if (_slotItem == null)
        {
            _icon.color = Color.clear;
        }
        else
        {
            _icon.sprite = _slotItem.Icon;
            _icon.color = Color.white;
        }
    }

    private void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>();
        _icon = images[images.Length - 1];
    }

    public void AddItem(Item item)
    {
        OwnerInventory[SlotIndex] = item;
    }

    public Item RemoveItem()
    {
        Item item = SlotItem;
        OwnerInventory[SlotIndex] = null;
        return item;
    }

    int clickCount = 0;
    float clickTime = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        //EventSystem.current.SetSelectedGameObject(gameObject);

        if(clickCount == 0 || Time.time - clickTime > 0.6f)
        {
            clickCount = 1;
            if (SlotItem != null)
            {
                MouseInventory.Instance.SelectItem(this);
                MouseInventory.Instance.TargetSlot = this;
            }
        }
        else
        {
            if (SlotItem != null)
            {
                Debug.Log("Use " + SlotItem.Name);
                InventoryAction.CurrentInventoryAction.Use(this);
                clickCount = 0;
            }
        }

        clickTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //EventSystem.current.SetSelectedGameObject(null);

        if (MouseInventory.Instance.IsDrag)
        {
            InventorySlot targetSlot = MouseInventory.Instance.TargetSlot;

            if (targetSlot == null)
            {
                Debug.Log("Drop");
                InventoryAction.CurrentInventoryAction.Drop(this);
                return;
            }

            if (targetSlot.SlotItem != null)
            {
                Debug.Log("Swap");
                InventoryAction.CurrentInventoryAction.Swap(MouseInventory.Instance.TargetSlot, this);
            }
            else
            {
                Debug.Log("Move");
                InventoryAction.CurrentInventoryAction.Move(MouseInventory.Instance.TargetSlot);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (MouseInventory.Instance.IsDrag)
        {
            MouseInventory.Instance.TargetSlot = this;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (MouseInventory.Instance.IsDrag && MouseInventory.Instance.TargetSlot == this)
        {
            MouseInventory.Instance.TargetSlot = null;
        }
    }
}
