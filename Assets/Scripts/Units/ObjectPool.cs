using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    protected T _prefab;
    [SerializeField]
    protected int _poolSize = 4;
    [SerializeField]
    protected bool _willGrow = true;
    protected HashSet<T> _pool;

    private void Awake()
    {
        _pool = new HashSet<T>();
        for (int i = 0; i < _poolSize; ++i)
        {
            T obj = Spawn();
            obj.gameObject.SetActive(false);
        }
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Gets an object from the pool.
    /// </summary>
    /// <returns>
    /// Returns a pooled object or null.
    /// </returns>
    public T GetObject()
    {
        T result = null;
        foreach (T item in _pool)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                result = item;
                break;
            }
        }
        if (result == null && _willGrow)
        {
            result = Spawn();
        }
        if (result != null)
        {
            result.gameObject.SetActive(true);
        }
        return result;
    }

    private T Spawn()
    {
        T obj = Instantiate(_prefab, transform.position, transform.rotation, transform);
        _pool.Add(obj);
        return obj;
    }
}