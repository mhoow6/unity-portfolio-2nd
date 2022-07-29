using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSystem : IGameSystem
{
    List<IBuffable> m_Buffs = new List<IBuffable>();

    public void Init()
    {
        
    }

    public void Tick()
    {
        
    }

    public void DoBuff(IBuffable buff)
    {
        // 버프 관리용도로 추가
        m_Buffs.Add(buff);

        StageManager.Instance.DoCoroutine(BuffCoroutine(buff));
    }

    IEnumerator BuffCoroutine(IBuffable buff)
    {
        buff.Affect();

        yield return new WaitForSeconds(buff.Duration());

        buff.Remove();

        m_Buffs.Remove(buff);
    }
}
