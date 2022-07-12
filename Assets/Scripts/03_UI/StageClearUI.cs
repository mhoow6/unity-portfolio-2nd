using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearUI : UI
{
    public override UIType Type => UIType.StageClear;

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        //StageManager.Instance.StageResult
    }
}
