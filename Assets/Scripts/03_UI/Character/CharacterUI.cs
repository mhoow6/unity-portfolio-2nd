using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using System.Linq;

public class CharacterUI : UI, IGameEventListener
{
    public override UIType Type => UIType.Character;

    public ObjectCode SelectedCharacter;

    [Header("# Left")]
    public Text SelectedCharacterLevel;
    public Text SelectedCharacterName;
    public Text SelectedCharacterType;
    public List<GameObject> SelectedCharacterTypeIcons = new List<GameObject>(3);
    public SplitSlider SelectedCharacterExpSlider;
    public Text SelectedCharacterExp;

    [Header("# Bottom")]
    public GameObject CharacterListElementPrefab;
    public GameObject CharacterListParent;
    List<CharacterListElement> m_CharacterList = new List<CharacterListElement>();

    [Header("# Right")]
    public Text SelectedCharacterWeapon;
    public Text Hp;
    public Text Sp;
    public Text Damage;
    public Text Defense;
    public Text Critical;
    public GameObject PartyJoinButton;
    public GameObject PartyOutButton;
    public GameObject PartyAlreadyJoinObject;

    bool m_UseForParty;
    bool m_IsLeaderSlot;
    ObjectCode m_FirstSelectedCharacter;
    int m_WorldIdx;
    int m_StageIdx;


    public void Listen(GameEvent gameEvent){}

    public void Listen(GameEvent gameEvent, params object[] args)
    {
        switch (gameEvent)
        {
            case GameEvent.LOBBY_ShowCharacter:
                {
                    ObjectCode selectedCharacter = (ObjectCode)args[0];
                    SelectedCharacter = selectedCharacter;

                    ShowCharacterInfo(SelectedCharacter);

                    // 캐릭터 생성
                    LobbyManager.Instance.LobbySystem.SpawnCharacterUICharacter(selectedCharacter);
                    break;
                }
            default:
                break;
        }

    }

    public override void OnClosed()
    { 
        GameEventSystem.RemoveListener(this);

        var lobbyManager = LobbyManager.Instance;
        // 메인 카메라 끄기
        lobbyManager.MainCam.gameObject.SetActive(false);

        // 생성시킨 캐릭터 삭제시키기
        lobbyManager.LobbySystem.DestroyCharacterUICharacter();

        m_CharacterList.ForEach(element => Destroy(element.gameObject));
        m_CharacterList.Clear();
    }

    public override void OnOpened()
    {
        // 옵저버에 등록
        GameEventSystem.AddListener(this);

        // 카메라 위치 조정 및 활성화
        var lobbyManager = LobbyManager.Instance;
        lobbyManager.MainCam.transform.SetPositionAndRotation
            (lobbyManager.LobbySystem.CharacterUICameraPosition.transform.position,
            lobbyManager.LobbySystem.CharacterUICameraPosition.transform.rotation);

        lobbyManager.MainCam.gameObject.SetActive(true);

    }

    public void SetData(ObjectCode selectedCharacter)
    {
        m_FirstSelectedCharacter = selectedCharacter;
        m_UseForParty = false;

        // 유저가 가지고 있는 캐릭터 목록 인스턴싱
        if (m_CharacterList.Count == 0)
        {
            ToggleGroup toggleGroup = CharacterListParent.GetComponent<ToggleGroup>();

            foreach (var cha in GameManager.PlayerData.CharacterDatas)
            {
                var _inst = Instantiate(CharacterListElementPrefab, CharacterListParent.transform);
                var inst = _inst.GetComponent<CharacterListElement>();

                inst.SetData(cha, toggleGroup);

                m_CharacterList.Add(inst);
            }
        }

        // 기본 정렬 순서는 캐릭터의 레벨 순서로 하자
        m_CharacterList.OrderBy(element => element.CharacterData.Level);
        for (int i = 0; i < m_CharacterList.Count; i++)
        {
            var child = m_CharacterList[i];
            child.transform.SetSiblingIndex(i);

            // selectedCharacter?
            if (child.CharacterData.Code == selectedCharacter)
                child.Toggle.isOn = true;
        }


        // 만약에 None으로 값이 들어올 경우는 유저가 가진 캐릭터들 중 맨 처음껄 보여주게 하자
        if (selectedCharacter == ObjectCode.NONE)
        {
            m_CharacterList[0].Toggle.isOn = true;
            selectedCharacter = m_CharacterList[0].CharacterData.Code;
        }

        Listen(GameEvent.LOBBY_ShowCharacter, selectedCharacter);
    }

    public void SetData(ObjectCode selectedCharacter, int worldIdx, int stageIdx, bool leaderSlot)
    {
        SetData(selectedCharacter);

        m_WorldIdx = worldIdx;
        m_StageIdx = stageIdx;
        m_UseForParty = true;
        m_IsLeaderSlot = leaderSlot;

        var stageRecord = GameManager.PlayerData.StageRecords.Find(stage => stage.WorldIdx == worldIdx && stage.StageIdx == stageIdx);

        // 파티가 껴있으면 파티 프리셋 순서대로 먼저 보여주기
        if (stageRecord != null)
        {
            for (int i = 0; i < m_CharacterList.Count; i++)
            {
                var view = m_CharacterList[i];
                if (view.CharacterData.Code == stageRecord.CharacterLeader)
                    view.transform.SetSiblingIndex(0);

                if (view.CharacterData.Code == stageRecord.CharacterSecond)
                    view.transform.SetSiblingIndex(1);

                if (view.CharacterData.Code == stageRecord.CharacterThird)
                    view.transform.SetSiblingIndex(2);
            }
        }

        // 파티탈퇴 버튼이 뜨는 경우
        // 리더자리가 아닌 슬롯에서 selectedCharacter를 고른 경우 && 파티에 selectedCharacter가 있는 경우
        if (!leaderSlot && selectedCharacter != ObjectCode.NONE)
        {
            bool find = false;
            if (stageRecord.CharacterSecond == selectedCharacter || stageRecord.CharacterThird == selectedCharacter)
                find = true;

            if (find)
                TriggerPartyButton(1);
            else
                TriggerPartyButton(2);
        }
        else
            TriggerPartyButton(2);

    }

    public void OnLevelUpButtonClick()
    {
        var ui = GameManager.UISystem.OpenWindow<NavigateLevelUpUI>(UIType.NavigateLevelUp, false);
        ui.SetData(SelectedCharacter);
    }

    public void OnPartyJoinButtonClick()
    {
        // 비어있는 곳 우선으로 파티 JOIN
        var stageRecord = GameManager.PlayerData.StageRecords.Find(stage => stage.WorldIdx == m_WorldIdx && stage.StageIdx == m_StageIdx);
        if (m_IsLeaderSlot)
        {
            stageRecord.CharacterLeader = SelectedCharacter;
        }
        else if (stageRecord.CharacterSecond == ObjectCode.NONE)
        {
            stageRecord.CharacterSecond = SelectedCharacter;
        }
        else if (stageRecord.CharacterThird == ObjectCode.NONE)
        {
            stageRecord.CharacterThird = SelectedCharacter;
        }


        GameEventSystem.SendEvent(GameEvent.LOBBY_ChangePartyPreset);
        GameManager.UISystem.CloseWindow();
    }

    public void OnPartyOutButtonClick()
    {
        var stageRecord = GameManager.PlayerData.StageRecords.Find(stage => stage.WorldIdx == m_WorldIdx && stage.StageIdx == m_StageIdx);
        if (stageRecord.CharacterSecond == SelectedCharacter)
        {
            stageRecord.CharacterSecond = ObjectCode.NONE;

            // 세번째가 있으면 두번째로 밀어줘야한다.
            if (stageRecord.CharacterThird != ObjectCode.NONE)
                stageRecord.CharacterSecond = stageRecord.CharacterThird;
        }
        else if (stageRecord.CharacterThird == SelectedCharacter)
        {
            stageRecord.CharacterThird = ObjectCode.NONE;
        }

        GameEventSystem.SendEvent(GameEvent.LOBBY_ChangePartyPreset);
        GameManager.UISystem.CloseWindow();
    }

    /// <summary>
    /// 파티관련 버튼을 활성화 시킵니다.
    /// </summary>
    /// <param name="index">0: Join, 1: Out, 2: Already, 3: 모두 비활성화</param>
    void TriggerPartyButton(int index)
    {
        PartyJoinButton.gameObject.SetActive(index == 0 ? true : false);
        PartyOutButton.gameObject.SetActive(index == 1 ? true : false);
        PartyAlreadyJoinObject.gameObject.SetActive(index == 2 ? true : false);

        if (index == 3)
        {
            PartyJoinButton.gameObject.SetActive(false);
            PartyOutButton.gameObject.SetActive(false);
            PartyAlreadyJoinObject.gameObject.SetActive(false);
        }
    }

    void ShowCharacterInfo(ObjectCode characterCode)
    {
        ObjectCode selectedCharacter = characterCode;

        // 레벨
        var characterRecord = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == selectedCharacter);
        SelectedCharacterLevel.text = $"Lv.{characterRecord.Level}";

        // 캐릭터 경험치
        var characterExpData = TableManager.Instance.CharacterLevelExperienceTable.Find(cha => cha.Level == characterRecord.Level);
        SelectedCharacterExpSlider.SetData(0, characterExpData.MaxExperience, (exp) =>
        {
            var playerCharacter = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == selectedCharacter);

            SelectedCharacterExp.text = $"{playerCharacter.Experience} / {characterExpData.MaxExperience}";
        });
        SelectedCharacterExpSlider.Value = characterRecord.Experience;

        // 캐릭터 스텟
        var characterData = Character.GetCharacterData(selectedCharacter, characterRecord.Level, characterRecord.EquipWeaponIndex);
        Hp.text = characterData.Hp.ToString();
        Sp.text = characterData.Sp.ToString();
        Damage.text = characterData.Damage.ToString();
        Defense.text = characterData.Defense.ToString();
        Critical.text = characterData.Critical.ToString();

        // 이름
        var characterTableData = TableManager.Instance.CharacterTable.Find(cha => cha.Code == selectedCharacter);
        SelectedCharacterName.text = characterTableData.Name;

        // 타입
        string characterType = TypeToString(characterTableData.Type);
        SelectedCharacterType.text = characterType;
        SelectedCharacterTypeIcons.ForEach(gameObj => gameObj.SetActive(false));
        SelectedCharacterTypeIcons[(int)characterTableData.Type].SetActive(true);

        // 무기
        var weaponData = TableManager.Instance.WeaponTable.Find(item => item.Index == characterRecord.EquipWeaponIndex);
        SelectedCharacterWeapon.text = weaponData.Name;

        // 파티용도로 쓰이는지에 따른 설정
        TriggerPartyButton(3);
        if (m_UseForParty)
        {
            var stageRecord = GameManager.PlayerData.StageRecords.Find(stage => stage.WorldIdx == m_WorldIdx && stage.StageIdx == m_StageIdx);

            // 파티가입 버튼이 뜨는 경우
            // m_FirstSelectedCharacter와 다른 캐릭터를 골랐을 경우
            if (selectedCharacter != m_FirstSelectedCharacter)
                TriggerPartyButton(0);

            // 편성중 버튼이 뜨는 경우
            // m_FirstSelectedCharacter와 다른 캐릭터를 골랐을 경우
            // 파티에 이미 있는 경우
            if (selectedCharacter != m_FirstSelectedCharacter)
            {
                bool already = false;
                if (stageRecord.CharacterLeader == selectedCharacter)
                    already = true;
                if (stageRecord.CharacterSecond == selectedCharacter)
                    already = true;
                if (stageRecord.CharacterThird == selectedCharacter)
                    already = true;

                if (already == true)
                    TriggerPartyButton(2);
            }

            // 파티탈퇴 버튼이 뜨는 경우
            // 처음에 골랐던 캐릭터를 다시 고르는 경우 (리더제외)
            if (selectedCharacter == m_FirstSelectedCharacter && stageRecord.CharacterLeader != m_FirstSelectedCharacter)
                TriggerPartyButton(1);

        }
    }
}
