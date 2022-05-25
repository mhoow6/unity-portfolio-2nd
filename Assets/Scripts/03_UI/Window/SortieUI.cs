using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortieUI : UI
{
    public override UIType Type => UIType.Sortie;
    public List<SelectCharacterDisplay> SelectCharacterDisplays = new List<SelectCharacterDisplay>();
    public RectTransform SelectCharacterGroupTransform;
    public RectTransform StageBackgroundTransform;

    [SerializeField] LeaderPassiveInfoDisplay m_LeaderPassiveInfoDisplay;
    [SerializeField] StatusDisplay m_StatusDisplay;
    
    Action m_OnBattleButtonClick;

    int m_WorldIdx;
    int m_StageIdx;

    public void SetData(int worldIdx, int stageIdx)
    {
        m_WorldIdx = worldIdx;
        m_StageIdx = stageIdx;

        // �÷��̾� �����Ϳ��� ��� ã��
        var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == worldIdx && r.StageIdx == stageIdx);
        if (record != null)
        {
            // ��Ƽ �������� ��ϴ�� ���ϱ�
            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader].SetData(record.CharacterLeader);
            // UNDONE: ���� �нú� ��ų�� ���� �˷��ֱ�
            m_LeaderPassiveInfoDisplay.SetData();
            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Second].SetData(record.CharacterSecond);
            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Third].SetData(record.CharacterThird);
        }
        else
        {
            // ����� ������ �ʻ�ȭ ������Ʈ �� ���̰�
            foreach (var display in SelectCharacterDisplays)
                display.PortraitVisible = false;

            // ��� ������ ���� �����
            GameManager.PlayerData.StageRecords.Add(new StageRecordData()
            {
                WorldIdx = worldIdx,
                StageIdx = stageIdx,
                Clear = false,
                CharacterLeader = ObjectCode.NONE,
                CharacterSecond = ObjectCode.NONE,
                CharacterThird = ObjectCode.NONE,
            });
        }

        // ��� �غ� ��ư ����
        m_OnBattleButtonClick = () =>
        {
            GameManager.Instance.LoadStage(worldIdx, stageIdx);
        };
    }

    #region UI �ʼ� ���� �޼ҵ�
    public override void OnClosed()
    {
        
    }

    public override void OnOpened()
    {
        m_StatusDisplay.SetData();
    }
    #endregion

    /// <summary> Battle Button-Button-OnClickEvent </summary>
    public void OnBattleButtonClick()
    {
        m_OnBattleButtonClick?.Invoke();
        m_OnBattleButtonClick = null;
    }

    /// <summary> Display�� ĳ���͵��� SetData ���� ����Ǿ��� �� ��� </summary>
    public void UpdatePartyPreset()
    {
        var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == m_WorldIdx && r.StageIdx == m_StageIdx);
        if (record == null)
            return;
        else
        {
            record.CharacterLeader = SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader].DisplayedCharacter;
            record.CharacterSecond = SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Second].DisplayedCharacter;
            record.CharacterThird = SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Third].DisplayedCharacter;
        }
    }

    enum SelectCharacterDisplaySlot
    {
        Leader = 0,
        Second = 1,
        Third = 2,
    }
}
