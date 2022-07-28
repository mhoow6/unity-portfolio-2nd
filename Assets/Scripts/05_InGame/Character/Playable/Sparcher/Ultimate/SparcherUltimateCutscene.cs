using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#region ��� ����
/*
 * 1. ������
 * 1�ܰ� �ڽĵ��� ���� ��Ȱ��ȭ ���·� ���д�.
 */
#endregion

public class SparcherUltimateCutscene : Cutscene
{
    protected override CinemachineBrain cinemachineBrain => StageManager.Instance.BrainCam;
    protected override Dictionary<string, BindingData> bindingKeyValuePairs => _bindingKeyValuePairs;
    protected override bool reUsable => true;

    Dictionary<string, BindingData> _bindingKeyValuePairs = new Dictionary<string, BindingData>();

    [SerializeField] BoxCollider _hitBox;
    GameObject _ultimateEffect;

    public void Signal_EffectOn()
    {
        // Control Track�� ������ ��� �̸� �ε�� ������Ʈ�� ����� ����Ʈ�� �����ش�.
        // �� Control Track�� Mute ���·� �־�� ��.
        var sparcher = StageManager.Instance.Player.CurrentCharacter as Sparcher;
        if (sparcher.Preloads == null)
            return;

        var preloads = sparcher.Preloads as SparcherPreloadSettings;
        if (preloads.UltimateEffect == null)
            return;

        var preloadEffect = preloads.UltimateEffect;
        var trackPrefab = controlTrackPrefabs["Effect Track"][0];
        if (preloadEffect.name.EraseBracketInName().Equals(trackPrefab.name.EraseBracketInName()))
        {
            _ultimateEffect = preloadEffect;
            preloadEffect.gameObject.SetActive(true);
            preloadEffect.transform.position = _hitBox.transform.position;
        }
    }

    public void Signal_EffectOff()
    {
        // Control Track�� ������ ��� �̸� �ε�� ������Ʈ�� ���
        if (_ultimateEffect == null)
            return;

        _ultimateEffect.gameObject.SetActive(false);
    }

    public void Signal_HitboxOn()
    {
        _hitBox.gameObject.SetActive(true);
        _hitBox.enabled = true;
    }

    public void Signal_HitboxOff()
    {
        _hitBox.enabled = false;
    }

    protected override bool CutSceneInput()
    {
        if (GameManager.InputSystem.PressBButton && StageManager.Instance.Player.CurrentCharacter.Code == ObjectCode.CHAR_Sparcher)
            return true;
        return false;
    }

    protected override void OnAwake()
    {
        // ���ε��� Ʈ���� ������Ʈ �����ϱ�
        var sm = StageManager.Instance;
        var character = transform.parent.GetComponent<Sparcher>();

        _bindingKeyValuePairs.Add("Sparcher Animation Track", new BindingData()
        {
            BindingObject = character.gameObject,
            BindingTrackCallback = (track) =>
            {
                var animeTrack = track.sourceObject as AnimationTrack;
                var secondTrackClips = animeTrack.GetClips();
                foreach (var clip in secondTrackClips)
                {
                    var animationPlayableAsset = clip.asset as AnimationPlayableAsset;
                    animationPlayableAsset.position = character.transform.position;
                    animationPlayableAsset.rotation = character.transform.rotation;
                }
            }
        });
        _bindingKeyValuePairs.Add("Signal Track", new BindingData()
        {
            BindingObject = GetComponent<SignalReceiver>(),
        });

        // ��Ʈ�ڽ� ����
        var skillData = Character.GetSkillData(Character.GetBInputDataIndex(ObjectCode.CHAR_Sparcher));
        if (skillData)
        {
            var sparcherUltiData = skillData as DatabaseSystem.SparcherUltiData;
            float range = sparcherUltiData.HitBoxRange;
            _hitBox.size = new Vector3(range, 1, range);
        }
    }

    protected override void OnCutSceneStart()
    {
        // �ƽ� ������ҵ� Ȱ��ȭ �����༭ �۵��ϰ� �ϱ�
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            // ����
            if (child.Equals(_hitBox.transform))
                continue;

            child.gameObject.SetActive(true);
        }

        // ĳ���� �� �����̰� �ϱ�
        GameManager.InputSystem.CameraRotatable = false;
        StageManager.Instance.Player.Moveable = false;
        StageManager.Instance.Player.CurrentCharacter.Invulnerable = true;
        var inGameUi = GameManager.UISystem.CurrentWindow as InGameUI;
        inGameUi.Joystick.gameObject.SetActive(false);

    }

    protected override void OnCutSceneFinish()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            child.gameObject.SetActive(false);
        }

        var timelineAsset = director.playableAsset;
        foreach (var output in timelineAsset.outputs)
        {
            // ��ġ�� �����ߴ��� �ʱ�ȭ ��Ű��
            if (output.sourceObject is AnimationTrack)
            {
                var animeTrack = output.sourceObject as AnimationTrack;
                var secondTrackClips = animeTrack.GetClips();
                foreach (var clip in secondTrackClips)
                {
                    var animationPlayableAsset = clip.asset as AnimationPlayableAsset;
                    animationPlayableAsset.position = Vector3.zero;
                    animationPlayableAsset.rotation = Quaternion.identity;
                }
            }
        }

        // ĳ���� �� �����̰� �ϱ�
        GameManager.InputSystem.CameraRotatable = true;
        StageManager.Instance.Player.Moveable = true;
        StageManager.Instance.Player.CurrentCharacter.Invulnerable = false;

        var inGameUi = GameManager.UISystem.CurrentWindow as InGameUI;
        if (inGameUi)
        {
            if (!inGameUi.Joystick.IsNullOrDestroyed())
                inGameUi.Joystick.gameObject.SetActive(true);
        }
    }
}
