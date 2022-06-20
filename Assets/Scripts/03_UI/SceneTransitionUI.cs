using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionUI : Toast
{
    public override ToastType Type => ToastType.SceneTransition;

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        
    }

    public override void OnPushed()
    {
        Initalize = true;
    }
}
