using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using UnityEngine.EventSystems;

public class CharacterButtonUI : Display
{
    [SerializeField] Image m_Portrait;
    [SerializeField] Slider m_HpSlider;
    [SerializeField] Slider m_SpSlider;
    [SerializeField] Image m_CoolTime;

    public Playable ConnectCharacter { get; private set; }
    const float SWAP_COOLTIME = 8f;

    public void OnButtonClick()
    {
        if (ConnectCharacter.Hp < 0)
            return;

        // 스킬버튼을 누른 캐릭터에 맞게 세팅
        var inGameUi = GameManager.UISystem.CurrentWindow as InGameUI;
        inGameUi.SettingSkillButtons(ConnectCharacter);

        // 기존 캐릭터에 대한 처리
        var player = StageManager.Instance.Player;
        Vector3 spawnPosition = player.CurrentCharacter.transform.position;
        Quaternion spawnRotation = player.CurrentCharacter.transform.rotation;
        ConnectCharacter.transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        var prevCharcter = player.CurrentCharacter;
        prevCharcter.gameObject.SetActive(false);
        player.CurrentCharacter = ConnectCharacter;

        // 기존 캐릭터 버튼을 켜준다.
        var find = inGameUi.CharacterButtonDisplays.Find(button => button.ConnectCharacter.Equals(prevCharcter));
        find.gameObject.SetActive(true);

        // 누른 캐릭터에 대한 처리
        ConnectCharacter.gameObject.SetActive(true);
        ConnectCharacter.SetUpdate(true);
        StageManager.Instance.FreeLookCam.Follow = ConnectCharacter.transform;
        StageManager.Instance.FreeLookCam.LookAt = ConnectCharacter.transform;

        gameObject.SetActive(false);

        // 캐릭터 스왑 이펙트
        var effect = StageManager.Instance.PoolSystem.Load<CharacterSwapEffect>($"{GameManager.GameDevelopSettings.EffectResourcePath}/FX_LevelUp_01");
        effect.transform.position = ConnectCharacter.transform.position;
    }

    public void SetData(Playable character)
    {
        ConnectCharacter = character;

        var row = TableManager.Instance.CharacterTable.Find(cha => cha.Code == character.Code);

        m_Portrait.sprite = Resources.Load<Sprite>($"{GameManager.GameDevelopSettings.TextureResourcePath}/{row.PortraitName}");
        
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
