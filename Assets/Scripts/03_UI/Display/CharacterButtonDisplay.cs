using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class CharacterButtonDisplay : Display
{
    [SerializeField] Image m_Portrait;
    [SerializeField] Slider m_HpSlider;
    [SerializeField] Slider m_SpSlider;

    public void SetData(Character character)
    {
        var row = TableManager.Instance.CharacterTable.Find(cha => cha.Code == character.Code);

        m_Portrait.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{row.PortraitName}");
        
        m_HpSlider.minValue = 0;
        m_HpSlider.maxValue = character.MaxHp;
        m_HpSlider.value = character.Hp;

        m_SpSlider.minValue = 0;
        m_SpSlider.maxValue = character.MaxSp;
        m_SpSlider.value = character.Sp;
    }
}
