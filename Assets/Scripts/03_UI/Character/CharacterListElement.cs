using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DatabaseSystem;

public class CharacterListElement : Display, IPointerClickHandler, IGameEventListener
{
    public Toggle Toggle;

    public Image Background;
    public Image Portrait;
    public Text Level;

    public List<Color> BackgroundColors = new List<Color>();

    [HideInInspector] public CharacterRecordData CharacterData;

    private void OnEnable()
    {
        Toggle.onValueChanged.AddListener(delegate {
            ChangeColor(Toggle);
        });
        GameEventSystem.AddListener(this);
    }

    private void OnDisable()
    {
        Toggle.onValueChanged.RemoveAllListeners();
        GameEventSystem.RemoveListener(this);
    }

    public void SetData(CharacterRecordData record, ToggleGroup group)
    {
        Toggle.group = group;
        CharacterData = record;

        Level.text = $"Lv.{record.Level}";

        var chaData = TableManager.Instance.CharacterTable.Find(cha => cha.Code == record.Code);
        Portrait.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{chaData.PortraitName}");
    }

    // OnValueChanged
    public void ChangeColor(Toggle change)
    {
        //Debug.Log($"{CharacterData.Code} 토글이 켜져있나요? {change.isOn}");

        Background.color = BackgroundColors[change.isOn ? 1 : 0];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameEventSystem.SendEvent(GameEvent.LOBBY_ShowCharacter, CharacterData.Code);
    }

    public void Listen(GameEvent gameEvent){}

    public void Listen(GameEvent gameEvent, params object[] args)
    {
        switch (gameEvent)
        {
            case GameEvent.LOBBY_ShowCharacter:
                {
                    ObjectCode character = (ObjectCode)args[0];
                    if (CharacterData.Code != character)
                        return;

                    Level.text = $"Lv.{CharacterData.Level}";
                    break;
                }
            default:
                break;
        }
    }
}
