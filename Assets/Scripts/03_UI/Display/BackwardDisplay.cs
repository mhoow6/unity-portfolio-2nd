using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwardDisplay : Display
{
    public void OnBackwardBtnClick()
    {
        GameManager.Instance.UISystem.CloseWindow();
    }

    public void OnHomeBtnClick()
    {
        var sys = GameManager.Instance.UISystem;

        // 메인화면은 기본적으로 열려있는 창이기 때문에 모든 창을 닫아주기만 하면 된다.
        sys.CloseAllWindow();
    }
}
