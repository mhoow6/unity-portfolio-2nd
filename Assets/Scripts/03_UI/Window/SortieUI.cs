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

        // 플레이어 데이터에서 기록 찾기
        var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == worldIdx && r.StageIdx == stageIdx);
        if (record != null)
        {
            // 파티 프리셋을 기록대로 정하기
            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader].SetData(record.CharacterLeader);
            // UNDONE: 리더 패시브 스킬이 뭔지 알려주기
            m_LeaderPassiveInfoDisplay.SetData();
            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Second].SetData(record.CharacterSecond);
            SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Third].SetData(record.CharacterThird);
        }
        else
        {
            // 기록이 없으면 초상화 오브젝트 안 보이게
            foreach (var display in SelectCharacterDisplays)
                display.PortraitVisible = false;

            // 기록 데이터 새로 만들기
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

        // 출격 준비 버튼 세팅
        m_OnBattleButtonClick = () =>
        {
            GameManager.Instance.LoadStage(worldIdx, stageIdx);
        };
    }

    #region UI 필수 구현 메소드
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

    /// <summary> Display의 캐릭터들이 SetData 이후 변경되었을 때 사용 </summary>
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
