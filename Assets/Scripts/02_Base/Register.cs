using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : MonoBehaviour
{
    protected void Awake() => RegisterToGameManager();
    public virtual void RegisterToGameManager() { }
}
