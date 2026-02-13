using System;
using System.Collections.Generic;
using UnityEngine;

public class GenericScripts : MonoBehaviour
{
    
    public Bullet bulletPrefab;
    public PoolingSystem<Bullet> bulletPool;
    private void Start()
    {
        bulletPool = new PoolingSystem<Bullet>(bulletPrefab);
    }

   

    private T GetFirst<T>(List<T> value)
    {
        return value[0];
    }
}

#region Pooling

public class PoolingSystem<T> where T : Component,IPoolable
{
    private T _prefab;
    List<T> _objects=new List<T>();

    public PoolingSystem(T prefab)
    {
        _prefab = prefab;

        for (int i = 0; i < 6; i++)
        {
            CreateNew();
        }
    }

    private T CreateNew()
    {
        T obj=GameObject.Instantiate(_prefab);
        obj.gameObject.SetActive(false);
        _objects.Add(obj);

        return obj;
    }

    public T Get()
    {
        if (_objects.Count == 0)
            CreateNew();
        
        T obj=_objects[0];
        _objects.RemoveAt(0);
        
        obj.gameObject.SetActive(true);
        
        return obj;
    }

    public void Retunr(T obj)
    {
               obj.gameObject.SetActive(false);
               _objects.Add(obj);
    }
    #endregion
}


