using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : InventoryInterface
{
    private static PlayerInventory _instance = null;
    public static PlayerInventory Instance
    {
        get
        {
            if (_instance == null)
            {
                PlayerInventory instance = FindObjectOfType<PlayerInventory>();
                if (instance == null)
                    _instance = Instantiate(ResourceManager.GetResource<GameObject>("UI/PlayerInventory"),
                    GameObject.FindGameObjectWithTag("MainCanvas").transform)
                    .GetComponent<PlayerInventory>();
                else
                    _instance = instance;
            }
            return _instance;
        }
    }


    public override EInventoryType GetInventoryType()
    {
        return EInventoryType.E_PLAYER;
    }
}
