using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyObject : MonoBehaviour, IPoolable
{
    #region 오브젝트 풀링
    bool m_Poolable;
    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }

    public void OnLoad()
    {
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
