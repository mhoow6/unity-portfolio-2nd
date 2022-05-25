using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    public virtual void SetData() { }
    protected virtual void OnClosed() { }
    protected void OnDisable() { OnClosed(); }
}
