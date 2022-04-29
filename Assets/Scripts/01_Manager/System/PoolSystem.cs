using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolSystem : GameSystem
{
    GameObject m_Root;
    // 키: 프리팹 경로 값: 오브젝트
    Dictionary<string, List<IPoolable>> m_PoolMap = new Dictionary<string, List<IPoolable>>();

    public void Init()
    {
        m_Root = new GameObject("@Pool");
    }

    public T Load<T>(string prefabPath) where T : Component, IPoolable
    {
        if (m_PoolMap.TryGetValue(prefabPath, out var list))
        {
            var find = list.Find(obj => obj.Poolable);
            if (find != null)
            {
                find.OnLoad();
                find.Poolable = false;
                return find as T;
            }
                
            else
                return LoadFromResourceFolder<T>(prefabPath, list);
        }
        else
            return LoadFromResourceFolder<T>(prefabPath, list);
    }

    T LoadFromResourceFolder<T>(string prefabPath, List<IPoolable> pool) where T : Component, IPoolable
    {
        var load = Resources.Load<T>(prefabPath);
        if (load != null)
        {
            var instantiate = Object.Instantiate(load, m_Root.transform);

            instantiate.OnLoad();
            instantiate.Poolable = false;

            if (pool != null)
                pool.Add(instantiate);
            else
            {
                pool = new List<IPoolable>();
                pool.Add(instantiate);
                m_PoolMap.Add(prefabPath, pool);
            }

            return instantiate;
        }
        return null;
    }

    public void Release(IPoolable obj)
    {
        foreach (var kvp in m_PoolMap)
        {
            string prefabPath = kvp.Key;
            var pool = kvp.Value;

            var find = pool.Find(o => o.Equals(obj));
            if (find != null)
            {
                find.OnRelease();
                find.Poolable = true;
                break;
            }
        }
    }

    public void Tick() { }
}
