using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class CharacterUI : UI, IGameEventListener
{
    public override UIType Type => UIType.Character;

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
    List<GameObject> m_CharacterList = new List<GameObject>();

    [Header("# Right")]
    public Text SelectedCharacterWeapon;

    public void Listen(GameEvent gameEvent){}

    public void Listen(GameEvent gameEvent, params object[] args)
    {
        switch (gameEvent)
        {
            case GameEvent.SwapCharacterInLobby:
                if (args.Length != 1)
                    return;

                ObjectCode selectedCharacter = (ObjectCode)args[0];

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

                // �̸�
                var characterData = TableManager.Instance.CharacterTable.Find(cha => cha.Code == selectedCharacter);
                SelectedCharacterName.text = characterData.Name;

                
                // Ÿ��
                string characterType = TypeToString(characterData.Type);
                SelectedCharacterType.text = characterType;
                SelectedCharacterTypeIcons.ForEach(gameObj => gameObj.SetActive(false));
                SelectedCharacterTypeIcons[(int)characterData.Type].SetActive(true);

                // ����
                var weaponData = TableManager.Instance.ItemTable.Find(item => item.Index == characterRecord.EquipWeaponIndex);
                SelectedCharacterWeapon.text = weaponData.Name;
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
        Listen(GameEvent.SwapCharacterInLobby, selectedCharacter);

        // ������ ������ �ִ� ĳ���� ��� �����ֱ�
        if (m_CharacterList.Count == 0)
        {
            ToggleGroup toggleGroup = CharacterListParent.GetComponent<ToggleGroup>();

            foreach (var cha in GameManager.PlayerData.CharacterDatas)
            {
                var _inst = Instantiate(CharacterListElementPrefab, CharacterListParent.transform);
                var inst = _inst.GetComponent<CharacterListElement>();

                inst.SetData(cha, toggleGroup);

                if (inst.CharacterData.Code == selectedCharacter)
                    inst.Toggle.isOn = true;

                m_CharacterList.Add(inst.gameObject);
            }
        }

    }

}
