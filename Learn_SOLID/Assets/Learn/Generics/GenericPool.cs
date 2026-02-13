using System.Collections.Generic;
using UnityEngine;

namespace Generics
{
    public class GenericPool<T> where T : MonoBehaviour, IPoolable
    {
        //Fields
        private T _prefab;
        private List<T> _objects = new List<T>();
        private Transform _parent;


        //Methods
        public GenericPool(T prefab, int size, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < size; i++)
                CreateNewObject();
        }

        private T CreateNewObject()
        {
            T obj = GameObject.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _objects.Add(obj);

            return obj;
        }

        public T GetObject()
        {
            T obj = _objects[0];
            obj.gameObject.SetActive(true);
            _objects.RemoveAt(0);
            obj.OnSpawn();

            return obj;
        }

        public void ReturnGameObject(T obj)
        {
            obj.OnDespawn();
            _objects.Add(obj);
            obj.gameObject.SetActive(false);
        }
    }
}