using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolSystem : IGameSystem
{
    GameObject m_Root;
    // 키: 프리팹 경로 값: 오브젝트
    Dictionary<string, List<IPoolable>> m_PoolMap = new Dictionary<string, List<IPoolable>>();

    const string DUMMY_OBJECT_NAME = "DummyObject";

    public void Init() => m_Root = new GameObject("@Pool");
    public void Init(GameObject poolRoot) => m_Root = poolRoot;

    public void Tick() { }

    #region 더미 오브젝트 풀링
    public PoolableObject LoadDummyObject()
    {
        if (m_PoolMap.TryGetValue(DUMMY_OBJECT_NAME, out var list))
        {
            var find = list.Find(obj => obj.Poolable);
            if (find != null)
            {
                find.OnLoad();
                find.Poolable = false;
                return find as PoolableObject;
            }
            else
                return LoadPrimitiveDummyObject(list);
        }
        else
            return LoadPrimitiveDummyObject(list);
    }

    PoolableObject LoadPrimitiveDummyObject(List<IPoolable> pool)
    {
        var load = new GameObject(DUMMY_OBJECT_NAME).AddComponent<PoolableObject>();
        load.transform.SetParent(m_Root.transform);

        load.OnLoad();
        load.Poolable = false;

        if (pool != null)
            pool.Add(load);
        else
        {
            pool = new List<IPoolable>();
            pool.Add(load);
            m_PoolMap.Add(DUMMY_OBJECT_NAME, pool);
        }

        return load;
    }
    #endregion

    #region 프리팹 오브젝트 풀링
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
        }
        
        return InstantaitePrefab<T>(prefabPath, list);
    }

    public T Load<T>(GameObject prefab, Transform parent = null) where T : Component, IPoolable
    {
        string prefabName = prefab.name.EraseBracketInName();
        if (m_PoolMap.TryGetValue(prefabName, out var list))
        {
            var find = list.Find(obj => obj.Poolable);
            if (find != null)
            {
                find.OnLoad();
                find.Poolable = false;
                return find as T;
            }
        }

        return InstantiatePrefab<T>(prefab, list, parent);
    }

    T InstantiatePrefab<T>(GameObject prefab, List<IPoolable> pool, Transform parent = null) where T : Component, IPoolable
    {
        var _inst = UnityEngine.Object.Instantiate(prefab);
        T inst = null;

        if (parent)
            inst.transform.SetParent(parent);
        else
            inst.transform.SetParent(m_Root.transform);

        inst.name = inst.name.EraseBracketInName();
        inst.OnLoad();
        inst.Poolable = false;

        if (pool == null)
        {
            pool = new List<IPoolable>();
            m_PoolMap.Add(inst.name, pool);
        }

        pool.Add(inst);
        return inst as T;
    }

    T InstantaitePrefab<T>(string prefabPath, List<IPoolable> pool) where T : Component, IPoolable
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
    #endregion

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
}
