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
    public List<ClearDetailUI> Displays = new List<ClearDetailUI>();
    public List<MissonClearDetailUI> MissionClearDetails = new List<MissonClearDetailUI>();

    public override void OnClosed()
    {
        MissionClearDetails.ForEach(misson => misson.gameObject.SetActive(true));
    }

    public override void OnOpened()
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        GameManager.InputSystem.CameraRotatable = false;

        var result = sm.StageResult;
        var stageData = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == sm.WorldIdx && stage.StageIdx == sm.StageIdx);

        // �������� �̸�
        StageName.text = stageData.StageName;

        // ��� �����ֱ�
        Displays[0].SetData($"{result.MaxCombo}");
        Displays[1].SetData($"{result.Duration.Minutes:00}:{result.Duration.Seconds:00}");
        Displays[2].SetData($"{result.Score}");

        // ����Ʈ ������ ������ǥ�� �ƿ� ���� ����
        if (stageData.Quest1Idx == 0)
        {
            MissionClearDetails.ForEach(misson => misson.gameObject.SetActive(false));
            return;
        }

        // ���� ��ǥ ���
        // �̼� �ý��ۿ� ���� �� �÷��̾ �̹� ���� �̼� ����� ���� ����
        if (sm.MissionSystem.QuestRecords.TryGetValue(stageData.Quest1Idx, out var quest1Record))
            MissionClearDetails[0].SetData(quest1Record.Clear, QuestDescription(stageData.Quest1Idx));
        else
        {
            var userQuest1Record = GameManager.PlayerData.QuestRecords.Find(quest => quest.QuestIdx == stageData.Quest1Idx);
            MissionClearDetails[0].SetData(userQuest1Record.Clear, QuestDescription(stageData.Quest1Idx));
            MissionClearDetails[0].ClearRewards.SetActive(false);
        }

        if (sm.MissionSystem.QuestRecords.TryGetValue(stageData.Quest2Idx, out var quest2Record))
            MissionClearDetails[1].SetData(quest2Record.Clear, QuestDescription(stageData.Quest2Idx));
        else
        {
            var userQuest2Record = GameManager.PlayerData.QuestRecords.Find(quest => quest.QuestIdx == stageData.Quest2Idx);
            MissionClearDetails[1].SetData(userQuest2Record.Clear, QuestDescription(stageData.Quest2Idx));
            MissionClearDetails[1].ClearRewards.SetActive(false);
        }

        if (sm.MissionSystem.QuestRecords.TryGetValue(stageData.Quest3Idx, out var quest3Record))
            MissionClearDetails[2].SetData(quest3Record.Clear, QuestDescription(stageData.Quest3Idx));
        else
        {
            var userQuest3Record = GameManager.PlayerData.QuestRecords.Find(quest => quest.QuestIdx == stageData.Quest3Idx);
            MissionClearDetails[2].SetData(userQuest3Record.Clear, QuestDescription(stageData.Quest3Idx));
            MissionClearDetails[2].ClearRewards.SetActive(false);
        }
    }

    public void OnStageOutBtnClick()
    {
        if (GameManager.Instance.IsTestZone)
        {
            GameManager.UISystem.CloseWindow();
            GameManager.UISystem.OpenWindow(UIType.InGame);
            return;
        }

        var result = StageManager.Instance.StageResult;

        GameManager.Instance.LoadScene(SceneCode.Lobby, 
            onSceneLoaded: () =>
            {
                LobbyManager.Instance.Init();

                var battleResult = GameManager.UISystem.OpenWindow<BattleResultUI>(UIType.BattleResult);
                battleResult.SetData(result);
            });
    }
}
