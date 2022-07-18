using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterListElement : Display, IPointerDownHandler
{
    public Toggle Toggle;

    public Image Background;
    public Image Portrait;
    public Text Level;

    public List<Color> BackgroundColors = new List<Color>();

    [HideInInspector] public CharacterRecordData CharacterData;

    // Å¬¸¯½Ã
    public void OnPointerDown(PointerEventData eventData)
    {
        GameEventSystem.SendEvent(GameEvent.SwapCharacterInLobby, CharacterData.Code);
    }

    public void SetData(CharacterRecordData record, ToggleGroup group)
    {
        Toggle.group = group;
        CharacterData = record;

        Level.text = $"Lv.{record.Level}";
    }

    // OnValueChanged
    public void ChangeColor(bool selected)
    {
        Background.color = BackgroundColors[selected ? 1 : 0];
    }
}
