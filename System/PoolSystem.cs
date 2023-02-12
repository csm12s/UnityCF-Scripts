﻿using System.Collections.Generic;

using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

//Simple ring buffer style pool system.
//You don't need to return the object to the pool,
//it just get pushed back to the
//end of the queue again.
//So use it only for object with short lifespan
//(projectile, particle system that auto disable)
public class PoolSystem : MonoBehaviour
{
    /// <summary>
    /// 100
    /// </summary>
    public static int poolSize = 100;


    public static PoolSystem Instance { get; private set; }

    public static void Create()
    {
        GameObject obj = new GameObject("PoolSystem");
        Instance = obj.AddComponent<PoolSystem>();
    }

    Dictionary<Object, Queue<Object>> m_Pools =
        new Dictionary<Object, Queue<Object>>();

    public void InitPool(Object prefab, int size, 
        Transform parentTransform = null)
    {
        if (m_Pools.ContainsKey(prefab))
        {
            return;
        }

        Queue<Object> queue = new Queue<Object>();

        for (int i = 0; i < size; ++i)
        {
            var newObj = Instantiate(prefab, parentTransform);
            SetActive(newObj, false);
            queue.Enqueue(newObj);
        }

        m_Pools[prefab] = queue;
    }

    public T GetInstance<T>(T prefab) where T : Object
    {
        Queue<Object> queue;
        if (m_Pools.TryGetValue(prefab, out queue))
        {
            Object obj;

            if (queue.Count > 0)
            {
                obj = queue.Dequeue();
            }
            else
            {
                obj = Instantiate(prefab);
            }

            SetActive(obj, true);
            queue.Enqueue(obj);

            return obj as T;
        }

        //Debug.LogError("No pool was init with this prefab");
        return null;
    }

    static void SetActive(Object obj, bool active)
    {
        GameObject go = null;

        if (obj is Component component)
        {
            go = component.gameObject;
        }
        else
        {
            go = obj as GameObject;
        }

        go.SetActive(active);
    }
}
