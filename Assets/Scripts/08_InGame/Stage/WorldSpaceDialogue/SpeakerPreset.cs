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
        // 초상화
        var speakerImagePath = $"{GameManager.Config.TextureResourcePath}/{dialogue.NpcImage}";
        if (m_SpeakerImagePath != speakerImagePath)
            m_Portrait.sprite = Resources.Load<Sprite>(speakerImagePath);

        // 라벨
        m_SpeakerLabel.color = SPEAK_LABEL_COLOR;

        // 대화자 이름
        // 공백인 경우 플레이어가 말하고 있는 거임
        if (dialogue.NpcName != string.Empty)
            m_SpeakerName.text = $"▶ {dialogue.NpcName}";
        else
            m_SpeakerName.text = $"▶ {GameManager.PlayerData.NickName}";
        SpeakerName = dialogue.NpcName;

        // 대화창
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
