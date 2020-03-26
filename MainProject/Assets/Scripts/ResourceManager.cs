using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    private static ResourceManager _instance = null;
    private Dictionary<string, Object> _resourceMap;

    public static ResourceManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceManager();
            }
            return _instance;
        }
    }

    ResourceManager()
    {
        _resourceMap = new Dictionary<string, Object>();
    }

    public static T GetResource<T>(string path) where T :Object
    {
        T obj;

        if (instance._resourceMap.ContainsKey(path))
        {
            obj = (T)instance._resourceMap[path];
        }
        else
        {
            obj = Resources.Load<T>(path);
            if (obj != null)
            {
                instance._resourceMap.Add(path, obj);
            }
        }

        return obj;
    }

}
