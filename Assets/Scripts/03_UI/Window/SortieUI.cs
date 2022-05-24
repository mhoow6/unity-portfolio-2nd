using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortieUI : UI
{
    public override UIType Type => UIType.Sortie;
    public List<SelectCharacterDisplay> m_SelectCharacterDisplays = new List<SelectCharacterDisplay>();
    public RectTransform SelectCharacterGroupTransform;
    public RectTransform StageBackgroundTransform;

    [SerializeField] LeaderPassiveInfoDisplay m_LeaderPassiveInfoDisplay;
    [SerializeField] StatusDisplay m_StatusDisplay;
    
    Action m_OnBattleButtonClick;

    public void SetData(int worldIdx, int stageIdx)
    {
        // �÷��̾� �����Ϳ��� ��� ã��
        var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == worldIdx && r.StageIdx == stageIdx);
        if (record != null)
        {
            // ��Ƽ �������� ��ϴ�� ���ϱ�
            m_SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader].SetData(record.CharacterLeader);
            // UNDONE: ���� �нú� ��ų�� ���� �˷��ֱ�
            m_LeaderPassiveInfoDisplay.SetData();
            m_SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Second].SetData(record.CharacterSecond);
            m_SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Third].SetData(record.CharacterThird);
        }
        else
        {
            // ����� ������ �ʻ�ȭ ������Ʈ �� ���̰�
            foreach (var display in m_SelectCharacterDisplays)
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

    // Battle Button-Button-OnClickEvent
    public void OnBattleButtonClick()
    {
        m_OnBattleButtonClick?.Invoke();
        m_OnBattleButtonClick = null;
    }

    enum SelectCharacterDisplaySlot
    {
        Leader = 0,
        Second = 1,
        Third = 2,
    }
}
