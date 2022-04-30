using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolSystem : GameSystem
{
    GameObject m_Root;
    // Ű: ������ ��� ��: ������Ʈ
    Dictionary<string, List<IPoolable>> m_PoolMap = new Dictionary<string, List<IPoolable>>();

    const string m_DUMMY_OBJECT_NAME = "DummyObject";

    #region ���� �ý���
    public void Init()
    {
        m_Root = new GameObject("@Pool");
    }

    public void Tick() { }

    public void Init(GameObject poolRoot)
    {
        m_Root = poolRoot;
    }
    #endregion

    #region ���� ������Ʈ Ǯ��
    public DummyObject LoadDummyObject()
    {
        if (m_PoolMap.TryGetValue(m_DUMMY_OBJECT_NAME, out var list))
        {
            var find = list.Find(obj => obj.Poolable);
            if (find != null)
            {
                find.OnLoad();
                find.Poolable = false;
                return find as DummyObject;
            }
            else
                return LoadPrimitiveDummyObject(list);
        }
        else
            return LoadPrimitiveDummyObject(list);
    }

    DummyObject LoadPrimitiveDummyObject(List<IPoolable> pool)
    {
        var load = new GameObject(m_DUMMY_OBJECT_NAME).AddComponent<DummyObject>();
        load.transform.SetParent(m_Root.transform);

        load.OnLoad();
        load.Poolable = false;

        if (pool != null)
            pool.Add(load);
        else
        {
            pool = new List<IPoolable>();
            pool.Add(load);
            m_PoolMap.Add(m_DUMMY_OBJECT_NAME, pool);
        }

        return load;
    }
    #endregion

    #region ������ ������Ʈ Ǯ��
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
