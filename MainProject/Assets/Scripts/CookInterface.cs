using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryInterface))]
public class CookInterface : MonoBehaviour
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

    public InventoryData Data { get; private set; }
    public bool IsOpen { get; private set; }

    InventoryInterface CookInventoryInterface { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        CookInventoryInterface = GetComponent<InventoryInterface>();
    }

    public static void ToggleCookInventory()
    {
        if (!Instance.IsOpen)
            OpenCookInventory();
        else
            CloseCookInventory();
    }

    public static void OpenCookInventory()
    {
        Instance.Data = new InventoryData(new Vector2Int(5, 1));
        Instance.CookInventoryInterface.Owner = Instance.Data;
        Instance.gameObject.SetActive(true);
        Instance.IsOpen = true;
    }

    public static void CloseCookInventory()
    {
        GameObject.FindGameObjectWithTag("PlayerInventory").GetComponent<InventoryInterface>().AppendInventory(Instance.CookInventoryInterface);
        Instance.Data = null;
        Instance.CookInventoryInterface.Owner = null;
        Instance.gameObject.SetActive(false);
        Instance.IsOpen = false;
    }

    public void Cook()
    {
        Debug.Log("Finish Cook");
        //조합식을 사용 아이템 리턴
        Item axe = new Item("axe", "", "", ResourceManager.GetResource<Sprite>("RPG_inventory_icons/axe"));
        CookInventoryInterface.Clear();
        GameObject.FindGameObjectWithTag("PlayerInventory").GetComponent<InventoryInterface>().AddItem(axe);
    }

    public void Cancel()
    {
        CloseCookInventory();
    }
}
