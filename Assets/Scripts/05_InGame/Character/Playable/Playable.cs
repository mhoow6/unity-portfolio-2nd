using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using System;
using System.Linq;

public abstract class Playable : Character
{
    #region �κ� �ִϸ��̼�
    // �κ񿡼� ĳ���� Ŭ���ÿ� �ִϸ��̼��� �������� �ϴµ� �ʿ���
    public List<AniType> LobbyAnimations { get; protected set; } = new List<AniType>();
    public void SetLobbyAnimations(string runtimeAnimatorControllerPath)
    {
        var table = TableManager.Instance.AniTypeDialogueTable.FindAll(row => row.ObjectCode == Code);
        foreach (var row in table)
            LobbyAnimations.Add((AniType)row.AniType);

        var currentScene = GameManager.SceneCode;
        var config = GameManager.GameDevelopSettings;
        switch (currentScene)
        {
            case SceneCode.Lobby:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"{config.AnimationControllerResourcePath}/{runtimeAnimatorControllerPath}");
                break;
            default:
                Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>($"{config.AnimationControllerResourcePath}/Sparcher/InGame_Sparcher");
                break;
        }
    }
    #endregion

    public int XStack;
    public float XCoolTime;

    public int BStack;
    public float BCoolTime;
    
    public virtual bool CanAInput() { return true; }
    public virtual void OnAInput() { }

    public virtual bool CanXInput() { return true; }
    public virtual void OnXInput() { }
    public virtual bool CanBInput() { return true; }
    public virtual void OnBInput() { }
    public virtual bool CanYInput() { return true; }
    public virtual void OnYInput() { }

    // -----------------------------------------------------------------------

    protected override void OnTargetSet(Character target)
    {
        if (m_TargetLockOnImage != null)
        {
            if (!m_TargetLockOnImage.Poolable)
                GameManager.UISystem.Pool.Release(m_TargetLockOnImage);
        }

        var image = GameManager.UISystem.Pool.Load<FloatingLockOnImage>($"{GameManager.GameDevelopSettings.UIResourcePath}/InGame/Effect/FloatingLockOn");
        m_TargetLockOnImage = image;
        image.SetData(target);
    }

    protected override void OnSpawn()
    {
        gameObject.tag = "Player";

        var dashData = GetSkillData(GetXInputDataIndex(Code));
        XStack = dashData.Stack;

        var ultiData = GetSkillData(GetBInputDataIndex(Code));
        BStack = ultiData.Stack;
    }

    protected override void OnDamaged(DamagedParam param)
    {
        StageManager.Instance.MissionSystem.ReportAll(QuestType.GET_DAMAGED);
    }

    protected override void OnDead(Character attacker, int damage)
    {
        var sm = StageManager.Instance;
        if (sm == null)
            return;

        // �״� �ִϸ��̼�
        Animator.SetInteger(ANITYPE_HASHCODE, (int)AniType.DEAD_0);

        sm.MissionSystem.ReportAll(QuestType.INCAPCITATED);

        // ���͵��� Ÿ���� ����
        sm.Monsters.ForEach(mob => mob.Target = null);

        // ��� ĳ���Ͱ� �� �׾����� üũ
        bool allDead = sm.Player.Characters.All(cha => cha.Hp <= 0);
        if (allDead)
            sm.StageFail();
        else
        {
            // ����ִ� ĳ���� �߿��� ù ��°�� �ҷ��´�.
            var liveCharacters = sm.Player.Characters.Where(cha => cha.Hp > 0);
            var changeCharacter = liveCharacters.First();

            StartCoroutine(SwapDelayCorotuine(changeCharacter));
        }
    }

    // -----------------------------------------------------------------------

    const float SWAP_DELAY_TIME = 3f;

    IEnumerator SwapDelayCorotuine(Playable changeCharacter)
    {
        // ��� �ִϸ��̼��� ������ ����Ǿ��ٴ� ���� ����?
        yield return new WaitForSeconds(SWAP_DELAY_TIME);
        StageManager.Instance.Player.SwapCharacter(changeCharacter);
    }
}
