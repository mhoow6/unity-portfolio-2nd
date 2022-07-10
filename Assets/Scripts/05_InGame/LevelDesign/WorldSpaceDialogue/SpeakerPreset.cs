using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;
using DG.Tweening;

public class SpeakerPreset : MonoBehaviour
{
    [SerializeField] Image m_Portrait;
    [SerializeField] Image m_SpeakerLabel;
    [SerializeField] Text m_SpeakerName;

    public bool IsSpeaking { get; private set; } = false;
    public string SpeakerName { get; private set; } = string.Empty;
    public float SpeakingTime { get; private set; }

    readonly Color SPEAK_LABEL_COLOR = Color.yellow;
    readonly Color LISTEN_LABEL_COLOR = Color.gray;
    const float SPEAKING_TIME = 5f;

    string m_SpeakerImagePath = string.Empty;
    DG.Tweening.Core.TweenerCore<string, string, DG.Tweening.Plugins.Options.StringOptions> m_SpeakingTween;

    public void Speak(StageDialogueTable dialogue, Text speakText)
    {
        // �ʻ�ȭ
        if (dialogue.NpcName != string.Empty)
        {
            var speakerImagePath = $"{GameManager.GameDevelopSettings.TextureResourcePath}/{dialogue.NpcImage}";
            if (m_SpeakerImagePath != speakerImagePath)
                m_Portrait.sprite = Resources.Load<Sprite>(speakerImagePath);
        }
        // �����̸� �÷��̾ ����
        else
        {
            var row = TableManager.Instance.CharacterTable.Find(character => character.Code == StageManager.Instance.Player.CurrentCharacter.Code);
            var speakerImagePath = $"{GameManager.GameDevelopSettings.TextureResourcePath}/{row.PortraitName}";
            if (m_SpeakerImagePath != speakerImagePath)
                m_Portrait.sprite = Resources.Load<Sprite>(speakerImagePath);
        }
        

        // ��
        m_SpeakerLabel.color = SPEAK_LABEL_COLOR;

        // ��ȭ�� �̸�
        // ������ ��� �÷��̾ ���ϰ� �ִ� ����
        if (dialogue.NpcName != string.Empty)
            m_SpeakerName.text = $"�� {dialogue.NpcName}";
        else
            m_SpeakerName.text = $"�� {GameManager.PlayerData.NickName}";
        SpeakerName = dialogue.NpcName;

        // ��ȭâ
        speakText.text = string.Empty;
        m_SpeakingTween = 
        speakText
            .DOText(dialogue.Dialogue, SPEAKING_TIME)
            .OnUpdate(() =>
            {
                SpeakingTime += Time.deltaTime;
            })
            .OnPlay(() => 
            {
                IsSpeaking = true;
                SpeakingTime = 0f;
            })
            .OnComplete(() => 
            { 
                IsSpeaking = false;
            });
    }

    public void SpeakComplete()
    {
        m_SpeakingTween.Complete();
    }

    public void Listen()
    {
        m_SpeakerLabel.color = LISTEN_LABEL_COLOR;
    }
}
