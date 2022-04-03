using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : MonoBehaviour
{
    protected void Start() => RegisterToGameManager();
    public virtual void RegisterToGameManager() { }

    protected void OnDestroy() => ReleaseToGameManager();
    public virtual void ReleaseToGameManager() { }
}
