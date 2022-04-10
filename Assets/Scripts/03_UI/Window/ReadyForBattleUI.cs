using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TableSystem;

public class ReadyForBattleUI : UI
{
    public Text StageName;
    public Image StageImage;
    public Text StageDescription;

    // ����Ʈ ��ǥ
    public MissionDisplay Mission1;
    public MissionDisplay Mission2;
    public MissionDisplay Mission3;

    // ���� ����� �� ���� Ƽ�� 3��
    public ItemDisplay Item1Display;
    public ItemDisplay Item2Display;
    public ItemDisplay Item3Display;

    public Text EnergyCost;

    public StatusDisplay StatusDisplay;

    public override UIType Type => UIType.ReadyForBattle;

    bool m_Init;
    List<StageTable> m_StageTable;
    List<QuestTable> m_QuestTable;
    PlayerData m_PlayerData;


    // ���� �غ� ��ư
    public void OnBattleBtnClick()
    {

    }
    
    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        if (!m_Init)
        {
            m_StageTable = TableManager.Instance.StageTable;
            m_QuestTable = TableManager.Instance.QuestTable;
            m_PlayerData = GameManager.Instance.PlayerData;

            m_Init = true;
        }

        StatusDisplay.SetData();
    }

    public void SetData(int worldIdx, int stageIdx)
    {
        var row = m_StageTable.Find(s => s.WorldIdx == worldIdx && s.StageIdx == stageIdx);

        // ����Ʈ ��ǥ
        var mission1Record = m_PlayerData.QuestRecords.Find(r => r.QuestIdx == row.Quest1Idx);
        bool mission1Clear = false;
        if (mission1Record != null)
            mission1Clear = mission1Record.Clear;

        var mission2Record = m_PlayerData.QuestRecords.Find(r => r.QuestIdx == row.Quest2Idx);
        bool mission2Clear = false;
        if (mission2Record != null)
            mission2Clear = mission2Record.Clear;

        var mission3Record = m_PlayerData.QuestRecords.Find(r => r.QuestIdx == row.Quest3Idx);
        bool mission3Clear = false;
        if (mission3Record != null)
            mission1Clear = mission3Record.Clear;


        StageName.text = $"Act.{stageIdx} {row.StageName}";
        StageDescription.text = row.StageDescription;

        Mission1.SetData(QuestDescription(row.Quest1Idx), mission1Clear);
        Mission2.SetData(QuestDescription(row.Quest2Idx), mission2Clear);
        Mission3.SetData(QuestDescription(row.Quest3Idx), mission3Clear);

        // TODO: ���� ������ ����Ʈ

        // ������ �Һ�
        EnergyCost.text = row.EnergyCost.ToString();
    }

    string QuestDescription(int questIdx)
    {
        string result = string.Empty;
        var row = m_QuestTable.Find(q => q.Index == questIdx);

        switch (row.Type)
        {
            case QuestType.KILL_ENEMY:
                result = $"{row.PurposeCount}�� �̻��� ���� óġ�Ѵ�.";
                break;
            case QuestType.GET_DAMAGED:
                result = $"�ǰ�Ƚ���� {row.PurposeCount}�� ���� ���� ��";
                break;
            case QuestType.INCAPCITATED:
                result = $"���� �Ҵ��� �� ĳ���� {row.PurposeCount}�� ����";
                break;
        }

        return result;
    }
}
