using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterMainCamera : Register
{
    public override void RegisterToGameManager()
    {
        GameManager.Instance.MainCam = GetComponent<Camera>();
    }
}
