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
        // 플레이어 데이터에서 기록 찾기
        var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == worldIdx && r.StageIdx == stageIdx);
        if (record != null)
        {
            // 파티 프리셋을 기록대로 정하기
            m_SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader].SetData(record.CharacterLeader);
            // UNDONE: 리더 패시브 스킬이 뭔지 알려주기
            m_LeaderPassiveInfoDisplay.SetData();
            m_SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Second].SetData(record.CharacterSecond);
            m_SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Third].SetData(record.CharacterThird);
        }
        else
        {
            // 기록이 없으면 초상화 오브젝트 안 보이게
            foreach (var display in m_SelectCharacterDisplays)
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
