using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AlarmVolume : MonoBehaviour, IPoolable
{
    public void Alarm(Action onAlarmEndCallback)
    {
        onAlarmEndCallback?.Invoke();
    }

    #region ������Ʈ Ǯ��
    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }
    bool m_Poolable;
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
