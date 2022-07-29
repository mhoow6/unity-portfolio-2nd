using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffable
{
    /// <summary>버프 시간 </summary>
    public int Duration();

    /// <summary>버프 효과 </summary>
    public void Affect();

    /// <summary>버프 제거 </summary>
    public void Remove();
}
