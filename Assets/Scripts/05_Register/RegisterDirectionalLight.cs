using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterDirectionalLight : Register
{
    public override void RegisterToGameManager()
    {
        GameManager.Instance.DirectLight = GetComponent<Light>();
    }
}
