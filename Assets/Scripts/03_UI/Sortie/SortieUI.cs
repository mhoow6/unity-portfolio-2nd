using DatabaseSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortieUI : UI
{
    public override UIType Type => UIType.Sortie;
    public List<SelectCharacterUI> SelectCharacterDisplays = new List<SelectCharacterUI>();
    public RectTransform SelectCharacterGroupTransform;
    public RectTransform StageBackgroundTransform;
    public SelectCharacterUI SelectCharacterUIPrefab;

    [SerializeField] Text m_EnergyCostText;
    [SerializeField] LeaderPassiveInfoUI m_LeaderPassiveInfoDisplay;
    [SerializeField] StatusUI m_StatusDisplay;

    int m_WorldIdx;
    int m_StageIdx;
    int m_EnergyCost;

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
            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader].IsLeaderSlot = true;
            m_LeaderPassiveInfoDisplay.SetData(record.CharacterLeader);

            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Second].SetData(record.CharacterSecond);
            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Second].IsLeaderSlot = false;

            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Third].SetData(record.CharacterThird);
            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Third].IsLeaderSlot = false;
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

        // ������ �Ҹ�
        var row = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == m_WorldIdx && stage.StageIdx == m_StageIdx);
        m_EnergyCost = row.EnergyCost;
        m_EnergyCostText.text = row.EnergyCost.ToString();
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
            m_LeaderPassiveInfoDisplay.SetData(record.CharacterLeader);

            record.CharacterSecond = SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Second].DisplayedCharacter;
            record.CharacterThird = SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Third].DisplayedCharacter;
        }
    }

    /// <summary> Battle Button-Button-OnClickEvent </summary>
    public void OnBattleButtonClick()
    {
        // ������ �־�� ���ӽ��� ����
        if (SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader].DisplayedCharacter == ObjectCode.NONE)
        {
            var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
            warning.SetData("������ �־�� ������ �����մϴ�.");
            return;
        }

        // ������ �Ҹ𷮸�ŭ �������� ������ ���� ������ ���ӺҰ���
        if (GameManager.PlayerData.Energy < m_EnergyCost)
        {
            var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
            warning.SetData("�������� �����Ͽ� ������ �� �����ϴ�.");
            return;
        }

        GameManager.PlayerData.Energy -= m_EnergyCost;
        
        var row = TableManager.Instance.StageTable.Find(s => s.WorldIdx == m_WorldIdx && s.StageIdx == m_StageIdx);
        GameManager.Instance.LoadScene(
            row.SceneCode,
            onPrevSceneLoading: () => { GameManager.UISystem.PushToast(ToastType.SceneTransition); },
            null,
            onSceneLoaded:
            () =>
            {
                GameManager.UISystem.CloseToast(true);
                StageManager.Instance.Init(); 
            });
    }

    public override void OnClosed()
    {

    }

    public override void OnOpened()
    {
        m_StatusDisplay.SetData();
    }
}
public enum SelectCharacterDisplaySlot
{
    Leader = 0,
    Second = 1,
    Third = 2,
}
