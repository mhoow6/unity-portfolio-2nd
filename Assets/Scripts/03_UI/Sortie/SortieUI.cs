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

    public int WorldIdx;
    public int StageIdx;
    int m_EnergyCost;

    public void SetData(int worldIdx, int stageIdx)
    {
        WorldIdx = worldIdx;
        StageIdx = stageIdx;

        // 플레이어 데이터에서 기록 찾기
        var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == worldIdx && r.StageIdx == stageIdx);
        if (record != null)
        {
            // 파티 프리셋을 기록대로 정하기
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

        // 에너지 소모량
        var row = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == WorldIdx && stage.StageIdx == StageIdx);
        m_EnergyCost = row.EnergyCost;
        m_EnergyCostText.text = row.EnergyCost.ToString();
    }

    /// <summary> Display의 캐릭터들이 SetData 이후 변경되었을 때 사용 </summary>
    public void UpdatePartyPreset()
    {
        var record = GameManager.PlayerData.StageRecords.Find(r => r.WorldIdx == WorldIdx && r.StageIdx == StageIdx);
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
        // 리더가 있어야 게임시작 가능
        if (SelectCharacterDisplays[(int)SelectCharacterDisplaySlot.Leader].DisplayedCharacter == ObjectCode.NONE)
        {
            var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
            warning.SetData("리더가 있어야 출전이 가능합니다.");
            return;
        }

        // 에너지 소모량만큼 에너지를 가지고 있지 않으면 게임불가능
        if (GameManager.PlayerData.Energy < m_EnergyCost)
        {
            var warning = GameManager.UISystem.OpenWindow<WarningUI>(UIType.Warning, false);
            warning.SetData("에너지가 부족하여 출전할 수 없습니다.");
            return;
        }

        GameManager.PlayerData.Energy -= m_EnergyCost;
        
        var row = TableManager.Instance.StageTable.Find(s => s.WorldIdx == WorldIdx && s.StageIdx == StageIdx);
        GameManager.Instance.LoadScene(
            row.SceneCode,
            onPrevSceneLoad:
            () =>
            {
                GameManager.UISystem.PushToast(ToastType.SceneTransition);
            },
            null,
            onSceneLoaded:
            () =>
            {
                StageManager.Instance.Init(() =>
                {
                    GameManager.UISystem.CloseToast(true);
                }); 
            });
    }

    public override void OnClosed(){ }

    public override void OnOpened()
    {
        
    }
}
public enum SelectCharacterDisplaySlot
{
    Leader = 0,
    Second = 1,
    Third = 2,
}
