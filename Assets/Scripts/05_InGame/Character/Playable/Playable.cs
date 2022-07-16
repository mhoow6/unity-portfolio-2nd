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

    #region ĳ���� ����
    /// <summary> ���ݹ�ư(A) ��� �̰� ȣ���Ͽ� �뽬�� �Ѵ�. </summary>
    public void Attack(SkillButtonUI skillButtonUI = null)
    {
        if (CanAttack() == false)
            return;

        GameManager.InputSystem.PressAButton = true;
    }

    public virtual bool CanAttack() { return true; }
    #endregion

    #region ĳ���� �뽬
    /// <summary> �뽬��ư(X) ��� �̰� ȣ���Ͽ� �뽬�� �Ѵ�. </summary>
    public void Dash(SkillButtonUI skillButtonUI = null)
    {
        // �뽬�� ���� ���ϴ� ��� ���ϰ� �ؾ��Ѵ�.
        if (CanDash() == false)
            return;

        var skillData = GetSkillData(GetDashIndex(Code));
        if (skillData.Stack != 0)
        {
            // ������ �� ���� ��ų�� ����� �� ����.
            if (m_CurrentDashStack == 0)
                return;

            // �� ��ư�� ������ �뽬�� �������� �Ǿ�����
            GameManager.InputSystem.PressXButton = true;

            // Sp �Һ�
            Sp -= skillData.SpCost;

            m_CurrentDashStack--;

            if (skillButtonUI)
            {
                skillButtonUI.OnStackConsume();

                // ������ �� ��Ÿ�ӿ� �� ���� ��������
                if (!m_ChargeDashStackCoroutine)
                    StartCoroutine(ChargeDashStackCoroutine(skillButtonUI));
            }
        }
        else
        {
            GameManager.InputSystem.PressXButton = true;

            Sp -= skillData.SpCost;
        }

    }

    public virtual bool CanDash() { return true; }
    #endregion

    #region ĳ���� �ñر�
    /// <summary> �ñر��ư(B) ��� �̰� ȣ���Ͽ� �ñر⸦ �Ѵ�. </summary>
    public void Ultimate(SkillButtonUI skillButtonUI = null)
    {
        // ġƮ
        if (GameManager.CheatSettings.FreeSkill)
        {
            GameManager.InputSystem.PressBButton = true;
            return;
        }

        // --------------------------------------------------

        // �뽬�� ���� ���ϴ� ��� ���ϰ� �ؾ��Ѵ�.
        if (CanUltimate() == false)
            return;

        var skillData = GetSkillData(GetUltimateIndex(Code));
        if (skillData.Stack != 0)
        {
            // ������ �� ���� ��ų�� ����� �� ����.
            if (m_CurrentUltiStack == 0)
                return;

            // �� ��ư�� ������ ��ų�� �������� �Ǿ�����
            GameManager.InputSystem.PressBButton = true;

            // Sp �Һ�
            Sp -= skillData.SpCost;

            m_CurrentUltiStack--;

            if (skillButtonUI)
            {
                skillButtonUI.OnStackConsume();

                // ������ �� ��Ÿ�ӿ� �� ���� ��������
                if (!m_ChargeUltiStackCoroutine)
                    StartCoroutine(ChargeUltimateStackCoroutine(skillButtonUI));
            }
        }
        else
        {
            GameManager.InputSystem.PressBButton = true;

            Sp -= skillData.SpCost;

            StartCoroutine(ChargeUltimateStackCoroutine(false, skillButtonUI));
        }
    }

    public virtual bool CanUltimate() { return true; }
    #endregion

    #region ĳ���� ����
    /// <summary> ������ư(Y) ��� �̰� ȣ���Ͽ� �ñر⸦ �Ѵ�. </summary>
    public void Jump(SkillButtonUI skillButtonUI = null)
    {
        if (CanJump() == false)
            return;

        // �� ��ư�� ������ ������ �������� �Ǿ�����
        GameManager.InputSystem.PressYButton = true;
    }

    public virtual bool CanJump() { return true; }
    #endregion

    // -----------------------------------------------------------------------

    #region ĳ���� �뽬
    [SerializeField, ReadOnly] protected int m_CurrentDashStack;
    protected bool m_ChargeDashStackCoroutine { get; private set; }
    protected IEnumerator ChargeDashStackCoroutine(SkillButtonUI skillButtonUI = null)
    {
        m_ChargeDashStackCoroutine = true;

        float timer = 0f;
        float progress = 0f;
        var skillData = GetSkillData(GetDashIndex(Code));
        float duration = skillData.CoolTime;
        float maxStack = skillData.Stack;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            progress = timer / duration;

            if (skillButtonUI != null)
                skillButtonUI.CoolTimeBackground.fillAmount = 1 - progress;

            yield return null;
        }

        m_CurrentDashStack++;
        skillButtonUI.OnStackCharge(m_CurrentDashStack);

        m_ChargeDashStackCoroutine = false;

        if (m_CurrentDashStack < maxStack)
            StartCoroutine(ChargeDashStackCoroutine(skillButtonUI));
    }
    #endregion

    #region ĳ���� �ñر�
    [SerializeField, ReadOnly] protected int m_CurrentUltiStack;
    protected bool m_ChargeUltiStackCoroutine { get; private set; }
    [SerializeField, ReadOnly] protected float m_CurrentUltiCoolTime;
    protected IEnumerator ChargeUltimateStackCoroutine(bool hasStack = true, SkillButtonUI skillButtonUI = null)
    {
        if (hasStack)
            m_ChargeUltiStackCoroutine = true;

        float timer = 0f;
        float progress = 0f;
        var skillData = GetSkillData(GetUltimateIndex(Code));
        float duration = skillData.CoolTime;
        float maxStack = skillData.Stack;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // UI ǥ��
            progress = timer / duration;
            if (skillButtonUI != null)
                skillButtonUI.CoolTimeBackground.fillAmount = 1 - progress;

            // ��Ÿ�� ������
            m_CurrentUltiCoolTime = duration - timer;

            yield return null;
        }
        m_CurrentUltiCoolTime = 0f;

        if (hasStack)
        {
            m_CurrentUltiStack++;
            skillButtonUI.OnStackCharge(m_CurrentUltiStack);

            m_ChargeUltiStackCoroutine = false;

            if (m_CurrentUltiStack < maxStack)
                StartCoroutine(ChargeUltimateStackCoroutine(skillButtonUI));
        }
    }
    #endregion

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

        var dashData = GetSkillData(GetDashIndex(Code));
        m_CurrentDashStack = dashData.Stack;
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
        sm.Player.AnimationJobs.Enqueue(AniType.DEAD_0);

        sm.MissionSystem.ReportAll(QuestType.INCAPCITATED);

        // ��� ĳ���Ͱ� �� �׾����� üũ
        bool allDead = sm.Player.Characters.All(cha => cha.Hp <= 0);
        if (allDead)
            sm.StageFail();
        else
        {
            // ����ִ� ĳ���� �߿��� ù ��°�� �ҷ��´�.
            var liveCharacters = sm.Player.Characters.Where(cha => cha.Hp > 0);
            var changeCharacter = liveCharacters.First();

            sm.Player.SwapCharacter(changeCharacter);
        }
    }

    // -----------------------------------------------------------------------

}
