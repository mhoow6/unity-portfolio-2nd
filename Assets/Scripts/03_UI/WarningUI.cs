using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class WarningUI : UI
{
    public Text MessageText;
    public RectTransform RectTransform;

    public override UIType Type => UIType.Warning;
        
    public void SetData(string message)
    {
        MessageText.text = message;
    }

    public void OnBackgroundClick()
    {
        GameManager.UISystem.CloseWindow(false);
    }

    public override void OnClosed()
    {
        //var sys = GameManager.UISystem;
        //sys.BlockRaycast = false;

        StopAllCoroutines();
    }

    public override void OnOpened()
    {
        var sys = GameManager.UISystem;
        //sys.BlockRaycast = true;

        RectTransform.localScale = new Vector3(1, 0, 1);
        RectTransform.DOScaleY(1f, sys.SCALE_TWEENING_SPEED);
        StartCoroutine(AutoCloseCoroutine());
    }

    IEnumerator AutoCloseCoroutine()
    {
        yield return new WaitForSeconds(3f);

        var sys = GameManager.UISystem;
        if (sys.CurrentWindow.Type == UIType.Warning)
        {
            RectTransform.DOScaleY(0f, sys.SCALE_TWEENING_SPEED).OnComplete(() =>
            {
                sys.CloseWindow(false);
            });
        }
    }
}
