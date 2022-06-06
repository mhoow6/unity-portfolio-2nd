using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using UnityEngine.EventSystems;

public class CharacterButtonDisplay : Display
{
    [SerializeField] Image m_Portrait;
    [SerializeField] Slider m_HpSlider;
    [SerializeField] Slider m_SpSlider;
    [SerializeField] Image m_CoolTime;

    public Character ConnectCharacter { get; private set; }
    const float SWAP_COOLTIME = 8f;
    bool m_EventRegisterd;
    bool m_IsCooldown;

    public void OnButtonClick()
    {
        if (ConnectCharacter.Hp < 0)
            return;

        if (m_IsCooldown)
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

        var find = inGameUi.CharacterButtonDisplays.Find(button => button.ConnectCharacter.Equals(prevCharcter));
        find.gameObject.SetActive(true);
        find.SetCooldownState();

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

    public void SetData(Character character)
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

        if (!m_EventRegisterd)
            ConnectCharacter.OnHpUpdate += (hp) =>
            {
                if (hp == 0)
                    SetDeadState();
            };
    }

    void SetDeadState()
    {
        m_CoolTime.gameObject.SetActive(true);
        m_CoolTime.fillAmount = 1f;
    }

    void SetCooldownState()
    {
        StartCoroutine(CoolTimeCoroutine());
    }

    IEnumerator CoolTimeCoroutine()
    {
        float timer = 0f;

        m_IsCooldown = true;
        m_CoolTime.gameObject.SetActive(true);

        while (timer < SWAP_COOLTIME)
        {
            timer += Time.deltaTime;

            m_CoolTime.fillAmount = 1 - (timer / SWAP_COOLTIME);
            yield return null;
        }

        m_CoolTime.fillAmount = 0f;
        m_IsCooldown = false;
        m_CoolTime.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (ConnectCharacter != null)
            ConnectCharacter.DisposeEvents();
    }
}
