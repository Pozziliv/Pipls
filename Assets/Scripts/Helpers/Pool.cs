using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T : MonoBehaviour
{
    private T _object;
    private List<T> _list = new List<T>();

    public Pool(T objectToSpawn)
    {
        _object = objectToSpawn;
    }

    public T CreateObject()
    {
        foreach (var item in _list)
        {
            if (item.gameObject.activeSelf)
            {
                return item;
            }
        }

        var obj = Instantiate(_object);
        return obj;
    }
}
