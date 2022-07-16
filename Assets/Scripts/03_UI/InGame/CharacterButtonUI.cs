using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using UnityEngine.EventSystems;

public class CharacterButtonUI : Display
{
    public Image Portrait;
    [SerializeField] Slider m_HpSlider;
    [SerializeField] Slider m_SpSlider;
    public Image CoolTime;

    public Playable ConnectCharacter { get; private set; }
    const float SWAP_COOLTIME = 8f;

    public void OnButtonClick()
    {
        if (ConnectCharacter.Hp < 0)
            return;

        StageManager.Instance.Player.SwapCharacter(ConnectCharacter);
    }

    public void SetData(Playable character)
    {
        ConnectCharacter = character;

        var row = TableManager.Instance.CharacterTable.Find(cha => cha.Code == character.Code);

        Portrait.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{row.PortraitName}");
        
        m_HpSlider.minValue = 0;
        m_HpSlider.maxValue = character.MaxHp;
        m_HpSlider.value = character.Hp;

        m_SpSlider.minValue = 0;
        m_SpSlider.maxValue = character.MaxSp;
        m_SpSlider.value = character.Sp;
    }

    private void OnDisable()
    {
        if (ConnectCharacter != null)
            ConnectCharacter.DisposeEvents();
    }
}
