using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    public virtual void SetData() { }
    protected virtual void OnClosed() { }
    protected virtual void OnInit() { }

    protected void Awake() { OnInit(); }
    protected void OnDisable() { OnClosed(); }
}
