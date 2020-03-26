using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerDownHandler
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
        _icon = GetComponentsInChildren<Image>()[1];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
