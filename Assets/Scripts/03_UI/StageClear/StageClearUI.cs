using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using System.Text.RegularExpressions;

public class StageClearUI : UI
{
    public override UIType Type => UIType.StageClear;

    public Text StageName;
    public List<ClearDetailDisplay> Displays = new List<ClearDetailDisplay>();
    public List<MissionClearDetailDisplay> MissionClearDetails = new List<MissionClearDetailDisplay>();

    public override void OnClosed()
    {
        MissionClearDetails.ForEach(misson => misson.gameObject.SetActive(true));
    }

    public override void OnOpened()
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        var result = sm.StageResult;
        var stageData = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == sm.WorldIdx && stage.StageIdx == sm.StageIdx);

        // 스테이지 이름
        StageName.text = stageData.StageName;

        // 결과 보여주기
        Displays[0].SetData($"{result.Combo}");
        Displays[1].SetData($"{result.Duration.Minutes:00}:{result.Duration.Seconds:00}");
        Displays[2].SetData($"{result.Score}");

        // 퀘스트 없으면 도전목표가 아예 없는 거임
        if (stageData.Quest1Idx == 0)
        {
            MissionClearDetails.ForEach(misson => misson.gameObject.SetActive(false));
            return;
        }

        // 도전 목표 결과
        MissionClearDetails[0].SetData(sm.MissionSystem.QuestRecords[stageData.Quest1Idx].Clear, QuestDescription(stageData.Quest1Idx));
        MissionClearDetails[1].SetData(sm.MissionSystem.QuestRecords[stageData.Quest2Idx].Clear, QuestDescription(stageData.Quest2Idx));
        MissionClearDetails[2].SetData(sm.MissionSystem.QuestRecords[stageData.Quest3Idx].Clear, QuestDescription(stageData.Quest3Idx));
    }

    public void OnStageOutBtnClick()
    {
        if (GameManager.Instance.IsTestZone)
        {
            GameManager.UISystem.CloseWindow();
            GameManager.UISystem.OpenWindow(UIType.InGame);
            return;
        }

        Time.timeScale = 1;
        GameManager.Instance.LoadScene(SceneCode.Lobby, 
            onSceneLoaded: () =>
            {
                LobbyManager.Instance.Init();

                var battleResult = GameManager.UISystem.OpenWindow<BattleResultUI>(UIType.BattleResult);
                battleResult.SetData(StageManager.Instance.StageResult);
            });
    }
}
