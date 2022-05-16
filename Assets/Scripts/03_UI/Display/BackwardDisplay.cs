using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwardDisplay : Display
{
    public void OnBackwardBtnClick()
    {
        GameManager.UISystem.CloseWindow();
    }

    public void OnHomeBtnClick()
    {
        var sys = GameManager.UISystem;
        sys.CloseAllWindow();
        sys.OpenWindow(UIType.MainMenu);
    }
}
