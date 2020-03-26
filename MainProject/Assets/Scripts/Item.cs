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

    public Item(string name, string lore, string guid, Sprite icon)
    {
        Name = name;
        Lore = lore;
        GUID = guid;
        Icon = icon;
    }
    //3d model, 2d sprite
}
