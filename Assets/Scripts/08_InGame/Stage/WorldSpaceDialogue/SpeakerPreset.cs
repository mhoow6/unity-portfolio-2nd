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

    readonly Color SPEAK_LABEL_COLOR = Color.yellow;
    readonly Color LISTEN_LABEL_COLOR = Color.gray;
    const float SPEAKING_TIME = 5f;

    string m_SpeakerImagePath = string.Empty;
    DG.Tweening.Core.TweenerCore<string, string, DG.Tweening.Plugins.Options.StringOptions> m_SpeakingTween;

    public void Speak(StageDialogueTable dialogue, Text speakText)
    {
        // �ʻ�ȭ
        var speakerImagePath = $"{GameManager.Config.TextureResourcePath}/{dialogue.NpcImage}";
        if (m_SpeakerImagePath != speakerImagePath)
            m_Portrait.sprite = Resources.Load<Sprite>(speakerImagePath);

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
            .OnPlay(() => { IsSpeaking = true; })
            .OnComplete(() => { IsSpeaking = false; });
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
