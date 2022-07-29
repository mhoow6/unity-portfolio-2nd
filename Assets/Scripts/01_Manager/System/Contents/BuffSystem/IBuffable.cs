using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    /// <summary>���� �ð� </summary>
    public int Duration();

    /// <summary>���� ȿ�� </summary>
    public void Affect();

    /// <summary>���� ���� </summary>
    public void Remove();
}
