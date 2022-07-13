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
    public List<MissionClearDetailDisplay> MissionClearDetails = new List<MissionClearDetailDisplay>();

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

        var quest1Data = TableManager.Instance.QuestTable.Find(quest => quest.Index == stageData.Quest1Idx);
        var quest1DescriptionData = TableManager.Instance.QuestDescriptionTable.Find(quest => quest.Type == quest1Data.Type);
        MissionClearDetails[0].SetData(result.Clear, string.Format(quest1DescriptionData.Description, quest1Data.PurposeCount));

        var quest2Data = TableManager.Instance.QuestTable.Find(quest => quest.Index == stageData.Quest2Idx);
        var quest2DescriptionData = TableManager.Instance.QuestDescriptionTable.Find(quest => quest.Type == quest2Data.Type);
        MissionClearDetails[1].SetData(result.Clear, string.Format(quest2DescriptionData.Description, quest2Data.PurposeCount));

        var quest3Data = TableManager.Instance.QuestTable.Find(quest => quest.Index == stageData.Quest3Idx);
        var quest3DescriptionData = TableManager.Instance.QuestDescriptionTable.Find(quest => quest.Type == quest3Data.Type);
        MissionClearDetails[2].SetData(result.Clear, string.Format(quest3DescriptionData.Description, quest3Data.PurposeCount));
    }

    public void OnStageOutBtnClick()
    {
        Debug.Log($"반응");
        //GameManager.Instance.LoadScene(SceneCode.Lobby, onSceneLoaded:)
    }
}
