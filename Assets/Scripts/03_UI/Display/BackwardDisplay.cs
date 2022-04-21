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

        // ����ȭ���� �⺻������ �����ִ� â�̱� ������ ��� â�� �ݾ��ֱ⸸ �ϸ� �ȴ�.
        sys.CloseAllWindow();
    }
}
