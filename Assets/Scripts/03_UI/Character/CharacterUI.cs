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
    ObjectCode m_FirstSelectedCharacter;
    int m_WorldIdx;
    int m_StageIdx;


    public void Listen(GameEvent gameEvent){}

    public void Listen(GameEvent gameEvent, params object[] args)
    {
        switch (gameEvent)
        {
            case GameEvent.LOBBY_SwapCharacter:
                if (args.Length != 1)
                    return;

                ObjectCode selectedCharacter = (ObjectCode)args[0];
                SelectedCharacter = selectedCharacter;

                // ����
                var characterRecord = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == selectedCharacter);
                SelectedCharacterLevel.text = $"Lv.{characterRecord.Level}";

                // ĳ���� ����ġ
                var characterExpData = TableManager.Instance.CharacterLevelExperienceTable.Find(cha => cha.Level == characterRecord.Level);
                SelectedCharacterExpSlider.SetData(0, characterExpData.MaxExperience, (exp) =>
                {
                    SelectedCharacterExp.text = $"{characterRecord.Experience} / {characterExpData.MaxExperience}";
                });
                SelectedCharacterExpSlider.Value = characterRecord.Experience;

                // ĳ���� ����
                var characterData = Character.GetCharacterData(selectedCharacter, characterRecord.Level, characterRecord.EquipWeaponIndex);
                Hp.text = characterData.Hp.ToString();
                Sp.text = characterData.Sp.ToString();
                Damage.text = characterData.Damage.ToString();
                Defense.text = characterData.Defense.ToString();
                Critical.text = characterData.Critical.ToString();

                // �̸�
                var characterTableData = TableManager.Instance.CharacterTable.Find(cha => cha.Code == selectedCharacter);
                SelectedCharacterName.text = characterTableData.Name;

                // Ÿ��
                string characterType = TypeToString(characterTableData.Type);
                SelectedCharacterType.text = characterType;
                SelectedCharacterTypeIcons.ForEach(gameObj => gameObj.SetActive(false));
                SelectedCharacterTypeIcons[(int)characterTableData.Type].SetActive(true);

                // ����
                var weaponData = TableManager.Instance.ItemTable.Find(item => item.Index == characterRecord.EquipWeaponIndex);
                SelectedCharacterWeapon.text = weaponData.Name;

                if (m_UseForParty)
                {
                    var stageRecord = GameManager.PlayerData.StageRecords.Find(stage => stage.WorldIdx == m_WorldIdx && stage.StageIdx == m_StageIdx);

                    // ��Ƽ���� ��ư�� �ߴ� ���
                    // m_FirstSelectedCharacter�� �ٸ� ĳ���͸� ����� ���
                    if (selectedCharacter != m_FirstSelectedCharacter)
                        TriggerPartyButton(0);

                    // ���� ��ư�� �ߴ� ���
                    // m_FirstSelectedCharacter�� �ٸ� ĳ���͸� ����� ���
                    // ��Ƽ�� �̹� �ִ� ���
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

                    // ��ƼŻ�� ��ư�� �ߴ� ���
                    // ó���� ����� ĳ���͸� �ٽ� ���� ��� (��������)
                    if (selectedCharacter == m_FirstSelectedCharacter && stageRecord.CharacterLeader != m_FirstSelectedCharacter)
                        TriggerPartyButton(1);

                }

                break;

            default:
                break;
        }

    }

    public override void OnClosed()
    { 
        GameEventSystem.RemoveListener(this);

        // ���� ī�޶� ����
        LobbyManager.Instance.MainCam.gameObject.SetActive(false);
    }

    public override void OnOpened()
    {
        // �������� ���
        GameEventSystem.AddListener(this);

        // ī�޶� ��ġ ���� �� Ȱ��ȭ
        var lobbyManager = LobbyManager.Instance;
        lobbyManager.MainCam.transform.SetPositionAndRotation
            (lobbyManager.MainLobbySystem.CharacterUICameraPosition.transform.position,
            lobbyManager.MainLobbySystem.CharacterUICameraPosition.transform.rotation);

        lobbyManager.MainCam.gameObject.SetActive(true);

    }

    public void SetData(ObjectCode selectedCharacter)
    {
        m_FirstSelectedCharacter = selectedCharacter;
        m_UseForParty = false;

        // ������ ������ �ִ� ĳ���� ��� �ν��Ͻ�
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

        // �⺻ ���� ������ ĳ������ ���� ������ ����
        m_CharacterList.OrderBy(element => element.CharacterData.Level);
        for (int i = 0; i < m_CharacterList.Count; i++)
        {
            var child = m_CharacterList[i];
            child.transform.SetSiblingIndex(i);

            // selectedCharacter?
            if (child.CharacterData.Code == selectedCharacter)
                child.Toggle.isOn = true;
        }


        // ���࿡ None���� ���� ���� ���� ������ ���� ĳ���͵� �� �� ó���� �����ְ� ����
        if (selectedCharacter == ObjectCode.NONE)
        {
            m_CharacterList[0].Toggle.isOn = true;
            selectedCharacter = m_CharacterList[0].CharacterData.Code;
        }

        Listen(GameEvent.LOBBY_SwapCharacter, selectedCharacter);
    }

    public void SetData(ObjectCode selectedCharacter, int worldIdx, int stageIdx, bool leaderSlot)
    {
        SetData(selectedCharacter);

        m_WorldIdx = worldIdx;
        m_StageIdx = stageIdx;
        m_UseForParty = true;

        var stageRecord = GameManager.PlayerData.StageRecords.Find(stage => stage.WorldIdx == worldIdx && stage.StageIdx == stageIdx);

        // ��Ƽ�� �������� ��Ƽ ������ ������� ���� �����ֱ�
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

        // ��ƼŻ�� ��ư�� �ߴ� ���
        // �����ڸ��� �ƴ� ���Կ��� selectedCharacter�� �� ��� && ��Ƽ�� selectedCharacter�� �ִ� ���
        if (!leaderSlot)
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

    }

    public void OnPartyJoinButtonClick()
    {

    }

    public void OnPartyOutButtonClick()
    {

    }

    /// <summary>
    /// ��Ƽ���� ��ư�� Ȱ��ȭ ��ŵ�ϴ�.
    /// </summary>
    /// <param name="index">0: Join, 1: Out, 2: Already, 3: ��� ��Ȱ��ȭ</param>
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

}
