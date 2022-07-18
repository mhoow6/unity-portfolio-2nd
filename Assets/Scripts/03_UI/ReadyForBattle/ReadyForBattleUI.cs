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

    // ����Ʈ ��ǥ
    public MissionDisplayUI Mission1;
    public MissionDisplayUI Mission2;
    public MissionDisplayUI Mission3;

    // UNDONE: ���� ����� �� ���� Ƽ�� 3��

    public Text EnergyCost;
    public StatusUI StatusDisplay;

    // �������� ����
    int m_WorldIdx;
    int m_StageIdx;

    public override UIType Type => UIType.ReadyForBattle;

    const float STAGE_DESCRIPTION_SPEED = 3f;


    // ���� �غ� ��ư
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
        
    }

    public void SetData(int worldIdx, int stageIdx)
    {
        var stageData = TableManager.Instance.StageTable.Find(s => s.WorldIdx == worldIdx && s.StageIdx == stageIdx);
        var playerData = GameManager.PlayerData;
        m_WorldIdx = worldIdx;
        m_StageIdx = stageIdx;

        // ����Ʈ ��ǥ
        var mission1Record = playerData.QuestRecords.Find(r => r.QuestIdx == stageData.Quest1Idx);
        bool mission1Clear = false;
        if (mission1Record != null)
            mission1Clear = mission1Record.Clear;

        var mission2Record = playerData.QuestRecords.Find(r => r.QuestIdx == stageData.Quest2Idx);
        bool mission2Clear = false;
        if (mission2Record != null)
            mission2Clear = mission2Record.Clear;

        var mission3Record = playerData.QuestRecords.Find(r => r.QuestIdx == stageData.Quest3Idx);
        bool mission3Clear = false;
        if (mission3Record != null)
            mission1Clear = mission3Record.Clear;

        // �������� ����
        StageName.text = $"Act.{stageIdx} {stageData.StageName}";
        StageDescription.text = string.Empty;
        StageDescription.DOText(stageData.StageDescription, STAGE_DESCRIPTION_SPEED);

        // �������� �̹���
        StageImage.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{stageData.StageImage}");

        // ��� ��ǥ
        Mission1.SetData(QuestDescription(stageData.Quest1Idx), mission1Clear);
        Mission2.SetData(QuestDescription(stageData.Quest2Idx), mission2Clear);
        Mission3.SetData(QuestDescription(stageData.Quest3Idx), mission3Clear);

        // ������ �Һ�
        EnergyCost.text = stageData.EnergyCost.ToString();
    }
}
