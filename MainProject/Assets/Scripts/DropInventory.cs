using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropInventory : InventoryInterface
{
    private static DropInventory _instance = null;
    public static DropInventory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(ResourceManager.GetResource<GameObject>("UI/DropInventory"),
                    GameObject.FindGameObjectWithTag("MainCanvas").transform)
                    .GetComponent<DropInventory>();
            }
            return _instance;
        }
    }
    public override EInventoryType GetInventoryType()
    {
        return EInventoryType.E_DROP;
    }

    public static bool IsOpen { get; private set; }

    public static void ToggleDropInventory()
    {
        if (!IsOpen)
            OpenDropInventory();
        else
            CloseDropInventory();
    }

    public static void OpenDropInventory()
    {
        if(CookInterface.IsOpen)
            CookInterface.CloseCookInventory();
        InventoryAction.CurrentInventoryAction = InventoryDropAction.instance;
        //Instance.Owner = new InventoryData(new Vector2Int(5, 1));
        Instance.gameObject.SetActive(true);
        IsOpen = true;
    }

    public static void CloseDropInventory()
    {
        InventoryAction.CurrentInventoryAction = InventoryIdleAction.instance;
        Instance.Owner = null;
        Instance.gameObject.SetActive(false);
        IsOpen = false;
    }
}
