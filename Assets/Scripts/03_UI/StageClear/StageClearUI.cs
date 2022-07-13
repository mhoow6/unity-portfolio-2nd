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

        // 도전 목표 결과
        MissionClearDetails[0].SetData(result.Clear, QuestDescription(stageData.Quest1Idx));
        MissionClearDetails[1].SetData(result.Clear, QuestDescription(stageData.Quest2Idx));
        MissionClearDetails[2].SetData(result.Clear, QuestDescription(stageData.Quest3Idx));
    }

    public void OnStageOutBtnClick()
    {
        if (GameManager.Instance.IsTestZone)
        {
            GameManager.UISystem.CloseWindow();
            GameManager.UISystem.OpenWindow(UIType.InGame);
            return;
        }

        GameManager.Instance.LoadScene(SceneCode.Lobby, onPrevSceneLoad:
            () =>
            {
                GameManager.UISystem.PushToast(ToastType.SceneTransition);
            }, 
            onSceneLoaded: () =>
            {
                GameManager.UISystem.CloseToast(true);

                GameManager.UISystem.OpenWindow(UIType.MainLobby);

                var battleResult = GameManager.UISystem.OpenWindow<BattleResultUI>(UIType.BattleResult);
                battleResult.SetData(StageManager.Instance.StageResult);
            });
    }
}
