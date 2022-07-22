using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RecoveryItem : DropItem
{
    protected int m_RecoveryScale;

    /// <param name="owner">����������� ����Ʈ�� ĳ����</param>
    public void SetData(Character owner)
    {
        if (owner is Boss)
            m_RecoveryScale = 2;
        else
            m_RecoveryScale = 1;
    }
}
