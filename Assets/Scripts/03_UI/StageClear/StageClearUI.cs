using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class StageClearUI : UI
{
    public override UIType Type => UIType.StageClear;

    public Text StageName;
    public List<ClearDetailDisplay> Displays = new List<ClearDetailDisplay>();

    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        var result = StageManager.Instance.StageResult;
        var sm = StageManager.Instance;
        var stageData = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == sm.WorldIdx && stage.StageIdx == sm.StageIdx);

        // 스테이지 이름
        StageName.text = stageData.StageName;

        // 결과 보여주기
        Displays[0].SetData($"{result.Combo}");
        Displays[1].SetData($"{result.Duration.Minutes:00}:{result.Duration.Seconds:00}");
        Displays[2].SetData($"{result.Score}");
    }
}
