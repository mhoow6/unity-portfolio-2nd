using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPoolable
{
    public bool Poolable { get; set; }
    public void OnLoad();
    public void OnRelease();
}
