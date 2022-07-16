using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using System;
using System.Linq;

public abstract class Playable : Character
{
    #region 로비 애니메이션
    // 로비에서 캐릭터 클릭시에 애니메이션이 나오도록 하는데 필요함
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

    #region 캐릭터 공격
    /// <summary> 공격버튼(A) 대신 이걸 호출하여 대쉬를 한다. </summary>
    public void Attack(SkillButtonUI skillButtonUI = null)
    {
        if (CanAttack() == false)
            return;

        GameManager.InputSystem.PressAButton = true;
    }

    public virtual bool CanAttack() { return true; }
    #endregion

    #region 캐릭터 대쉬
    /// <summary> 대쉬버튼(X) 대신 이걸 호출하여 대쉬를 한다. </summary>
    public void Dash(SkillButtonUI skillButtonUI = null)
    {
        // 대쉬를 하지 못하는 경우 못하게 해야한다.
        if (CanDash() == false)
            return;

        var skillData = GetSkillData(GetDashIndex(Code));
        if (skillData.Stack != 0)
        {
            // 스택을 다 쓰면 스킬을 사용할 수 없다.
            if (m_CurrentDashStack == 0)
                return;

            // 이 버튼을 눌러야 대쉬가 나가도록 되어있음
            GameManager.InputSystem.PressXButton = true;

            // Sp 소비
            Sp -= skillData.SpCost;

            m_CurrentDashStack--;

            if (skillButtonUI)
            {
                skillButtonUI.OnStackConsume();

                // 스택은 한 쿨타임에 한 번만 충전가능
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

    #region 캐릭터 궁극기
    /// <summary> 궁극기버튼(B) 대신 이걸 호출하여 궁극기를 한다. </summary>
    public void Ultimate(SkillButtonUI skillButtonUI = null)
    {
        // 치트
        if (GameManager.CheatSettings.FreeSkill)
        {
            GameManager.InputSystem.PressBButton = true;
            return;
        }

        // --------------------------------------------------

        // 대쉬를 하지 못하는 경우 못하게 해야한다.
        if (CanUltimate() == false)
            return;

        var skillData = GetSkillData(GetUltimateIndex(Code));
        if (skillData.Stack != 0)
        {
            // 스택을 다 쓰면 스킬을 사용할 수 없다.
            if (m_CurrentUltiStack == 0)
                return;

            // 이 버튼을 눌러야 스킬이 나가도록 되어있음
            GameManager.InputSystem.PressBButton = true;

            // Sp 소비
            Sp -= skillData.SpCost;

            m_CurrentUltiStack--;

            if (skillButtonUI)
            {
                skillButtonUI.OnStackConsume();

                // 스택은 한 쿨타임에 한 번만 충전가능
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

    #region 캐릭터 점프
    /// <summary> 점프버튼(Y) 대신 이걸 호출하여 궁극기를 한다. </summary>
    public void Jump(SkillButtonUI skillButtonUI = null)
    {
        if (CanJump() == false)
            return;

        // 이 버튼을 눌러야 점프가 나가도록 되어있음
        GameManager.InputSystem.PressYButton = true;
    }

    public virtual bool CanJump() { return true; }
    #endregion

    // -----------------------------------------------------------------------

    #region 캐릭터 대쉬
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

    #region 캐릭터 궁극기
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

            // UI 표기
            progress = timer / duration;
            if (skillButtonUI != null)
                skillButtonUI.CoolTimeBackground.fillAmount = 1 - progress;

            // 쿨타임 데이터
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

        // 죽는 애니메이션
        sm.Player.AnimationJobs.Enqueue(AniType.DEAD_0);

        sm.MissionSystem.ReportAll(QuestType.INCAPCITATED);

        // 모든 캐릭터가 다 죽었는지 체크
        bool allDead = sm.Player.Characters.All(cha => cha.Hp <= 0);
        if (allDead)
            sm.StageFail();
        else
        {
            // 살아있는 캐릭터 중에서 첫 번째를 불러온다.
            var liveCharacters = sm.Player.Characters.Where(cha => cha.Hp > 0);
            var changeCharacter = liveCharacters.First();

            sm.Player.SwapCharacter(changeCharacter);
        }
    }

    // -----------------------------------------------------------------------

}
