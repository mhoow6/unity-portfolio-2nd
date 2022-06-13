using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using DG.Tweening;

public class ReadyForBattleUI : UI
{
    public Text StageName;
    public Image StageImage;
    public Text StageDescription;

    // 퀘스트 목표
    public MissionDisplayUI Mission1;
    public MissionDisplayUI Mission2;
    public MissionDisplayUI Mission3;

    // UNDONE: 몬스터 드랍템 중 상위 티어 3개

    public Text EnergyCost;
    public StatusUI StatusDisplay;

    // 스테이지 정보
    int m_WorldIdx;
    int m_StageIdx;

    public override UIType Type => UIType.ReadyForBattle;

    const float STAGE_DESCRIPTION_SPEED = 3f;


    // 전투 준비 버튼
    public void OnBattleBtnClick()
    {
        var window = GameManager.UISystem.OpenWindow<SortieUI>(UIType.Sortie);
        window.SetData(m_WorldIdx, m_StageIdx);
    }
    
    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        StatusDisplay.SetData();
    }

    public void SetData(int worldIdx, int stageIdx)
    {
        var row = TableManager.Instance.StageTable.Find(s => s.WorldIdx == worldIdx && s.StageIdx == stageIdx);
        var playerData = GameManager.PlayerData;
        m_WorldIdx = worldIdx;
        m_StageIdx = stageIdx;

        // 퀘스트 목표
        var mission1Record = playerData.QuestRecords.Find(r => r.QuestIdx == row.Quest1Idx);
        bool mission1Clear = false;
        if (mission1Record != null)
            mission1Clear = mission1Record.Clear;

        var mission2Record = playerData.QuestRecords.Find(r => r.QuestIdx == row.Quest2Idx);
        bool mission2Clear = false;
        if (mission2Record != null)
            mission2Clear = mission2Record.Clear;

        var mission3Record = playerData.QuestRecords.Find(r => r.QuestIdx == row.Quest3Idx);
        bool mission3Clear = false;
        if (mission3Record != null)
            mission1Clear = mission3Record.Clear;

        // 스테이지 설명
        StageName.text = $"Act.{stageIdx} {row.StageName}";
        StageDescription.text = string.Empty;
        StageDescription.DOText(row.StageDescription, STAGE_DESCRIPTION_SPEED);

        // 스테이지 이미지
        StageImage.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{row.StageImage}");

        // 긴급 목표
        Mission1.SetData(QuestDescription(row.Quest1Idx), mission1Clear);
        Mission2.SetData(QuestDescription(row.Quest2Idx), mission2Clear);
        Mission3.SetData(QuestDescription(row.Quest3Idx), mission3Clear);

        // UNDONE: 스테이지에서 얻을 수 있는 아이템 리스트

        // 에너지 소비
        EnergyCost.text = row.EnergyCost.ToString();
    }

    string QuestDescription(int questIdx)
    {
        string result = string.Empty;
        var row = TableManager.Instance.QuestTable.Find(q => q.Index == questIdx);

        switch (row.Type)
        {
            case QuestType.KILL_ENEMY:
                result = $"{row.PurposeCount}기 이상의 적을 처치한다.";
                break;
            case QuestType.GET_DAMAGED:
                result = $"피격횟수가 {row.PurposeCount}번을 넘지 않을 것";
                break;
            case QuestType.INCAPCITATED:
                result = $"전투 불능이 된 캐릭터 {row.PurposeCount}명 이하";
                break;
        }

        return result;
    }
}
