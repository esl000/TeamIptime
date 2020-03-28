using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookInterface : InventoryInterface
{
    private static CookInterface _instance = null;
    public static CookInterface Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Instantiate(ResourceManager.GetResource<GameObject>("UI/CookInventory"),
                    GameObject.FindGameObjectWithTag("MainCanvas").transform)
                    .GetComponent<CookInterface>();
            }
            return _instance;
        }
    }
    public override EInventoryType GetInventoryType()
    {
        return EInventoryType.E_COOKING;
    }

    public static bool IsOpen { get; private set; }

    public static void ToggleCookInventory()
    {
        if (!IsOpen)
            OpenCookInventory();
        else
            CloseCookInventory();
    }

    public static void OpenCookInventory()
    {
        if(DropInventory)
        InventoryAction.CurrentInventoryAction = InventoryCookAction.instance;
        Instance.Owner = new InventoryData(new Vector2Int(5, 1));
        Instance.gameObject.SetActive(true);
        IsOpen = true;
    }

    public static void CloseCookInventory()
    {
        InventoryAction.CurrentInventoryAction = InventoryIdleAction.instance;
        PlayerInventory.Instance.AppendInventory(Instance);
        Instance.Owner = null;
        Instance.gameObject.SetActive(false);
        IsOpen = false;
    }

    public void Cook()
    {
        Debug.Log("Finish Cook");
        //조합식을 사용 아이템 리턴
        Item axe = new Item("axe", "", "", ResourceManager.GetResource<Sprite>("RPG_inventory_icons/axe"));
        Clear();
        PlayerInventory.Instance.AddItem(axe);
    }

    public void Cancel()
    {
        CloseCookInventory();
    }
}
