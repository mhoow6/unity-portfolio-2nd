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

    readonly Color SPEAK_LABEL_COLOR = Color.yellow;
    readonly Color LISTEN_LABEL_COLOR = Color.gray;
    const float SPEAKING_TIME = 5f;

    string m_SpeakerImagePath = string.Empty;

    public void Speak(StageDialogueTable dialogueTable, Text speakTo)
    {
        var gm = GameManager.Instance;

        var speakerImagePath = $"{gm.Config.TextureResourcePath}/{dialogueTable.NpcImage}";
        if (m_SpeakerImagePath != speakerImagePath)
            m_Portrait.sprite = Resources.Load<Sprite>(speakerImagePath);

        m_SpeakerLabel.color = SPEAK_LABEL_COLOR;
        m_SpeakerName.text = dialogueTable.NpcName;

        speakTo.text = string.Empty;
        speakTo.DOText(dialogueTable.Dialogue, SPEAKING_TIME);
    }

    public void Listen()
    {
        m_SpeakerLabel.color = LISTEN_LABEL_COLOR;
    }
}
