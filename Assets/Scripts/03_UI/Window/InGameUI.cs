using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : UI
{
    public override UIType Type => UIType.InGame;

    [SerializeField] VirtualJoystick Joystick;


    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        GameManager.Instance.InputSystem.Controller = Joystick;
        GameManager.Instance.InputSystem.CameraRotatable = true;
    }
}
