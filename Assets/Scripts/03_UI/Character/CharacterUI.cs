using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class CharacterUI : UI, IGameEventListener
{
    public override UIType Type => UIType.Character;

    public Text SelectedCharacterLevel;
    public Text SelectedCharacterName;
    public Text SelectedCharacterType;
    public List<GameObject> SelectedCharacterTypeIcons = new List<GameObject>(3);

    public void Listen(GameEvent gameEvent){}

    public void Listen(GameEvent gameEvent, params object[] args)
    {
        if (gameEvent != GameEvent.SwapCharacter)
            return;

        ObjectCode selectedCharacter = (ObjectCode)args[0];

        // 레벨
        var characterRecord = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == selectedCharacter);
        SelectedCharacterLevel.text = $"Lv.{characterRecord.Level}";

        // 이름
        var characterData = TableManager.Instance.CharacterTable.Find(cha => cha.Code == selectedCharacter);
        SelectedCharacterName.text = characterData.Name;

        // 타입
        string characterType = TypeToString(characterData.Type);
        SelectedCharacterType.text = characterType;
        SelectedCharacterTypeIcons.ForEach(gameObj => gameObj.SetActive(false));
        SelectedCharacterTypeIcons[(int)characterData.Type].SetActive(true);

    }

    public override void OnClosed()
    { 
        GameEventSystem.RemoveListener(this);

        // 메인 카메라 끄기
        LobbyManager.Instance.MainCam.gameObject.SetActive(false);
    }

    public override void OnOpened()
    { 
        GameEventSystem.AddListener(this);

        var lobbyManager = LobbyManager.Instance;
        lobbyManager.MainCam.transform.SetPositionAndRotation
            (lobbyManager.MainLobbySystem.CharacterUICameraPosition.transform.position,
            lobbyManager.MainLobbySystem.CharacterUICameraPosition.transform.rotation);

        lobbyManager.MainCam.gameObject.SetActive(true);
    }

    public void SetData(ObjectCode selectedCharacter)
    {
        Listen(GameEvent.SwapCharacter, selectedCharacter);
    }

}
