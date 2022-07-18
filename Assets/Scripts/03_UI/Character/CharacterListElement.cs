using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DatabaseSystem;

public class CharacterListElement : Display, IPointerClickHandler
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
    }

    private void OnDisable()
    {
        Toggle.onValueChanged.RemoveAllListeners();
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
        GameEventSystem.SendEvent(GameEvent.SwapCharacterInLobby, CharacterData.Code);
    }
}
