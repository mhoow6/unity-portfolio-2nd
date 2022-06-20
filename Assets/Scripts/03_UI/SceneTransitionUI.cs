using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionUI : UI
{
    public override UIType Type => UIType.SceneTransition;

    public override void OnClosed()
    {
        GameManager.UISystem.OnSceneLoadingWindow = null;
    }

    public override void OnOpened()
    {
        GameManager.UISystem.OnSceneLoadingWindow = this;
    }
}
