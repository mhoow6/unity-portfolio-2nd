using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class Character : BaseObject
{
    public readonly int ANITYPE_HASHCODE = Animator.StringToHash("AniType");
    public Animator Animator { get; private set; }

    #region 인게임용 데이터
    public CharacterType Type { get; private set; }
    public string Name { get; private set; }
    public Dictionary<SkillType, int> SkillIndices = new Dictionary<SkillType, int>();
    [ReadOnly] public AniType CurrentAniType;
    #endregion

    /// <summary> 기록용 데이터 </summary> ///
    public CharacterData Data;

    protected void Start()
    {
        // 컴포넌트 붙이기
        if (!Animator)
            Animator = GetComponent<Animator>();

        SetMainMenuAnimations();
        SetPropertiesFromTable();      

        OnSpawn();
    }

    protected void Update()
    {
        CurrentAniType = (AniType)Animator.GetInteger(ANITYPE_HASHCODE);

        OnLive();
    }

    protected void OnDestroy()
    {
        OnDead();
    }

    protected virtual void OnSpawn() { }
    protected virtual void OnDead() { }
    protected virtual void OnLive() { }

    #region 공격
    /// <summary> 애니메이션 이벤트 함수 </summary> ///
    public virtual void Attack(int skillIndex) { }
    public virtual AniType GetAniType(int skillIndex) { return AniType.NONE; }
    public virtual int GetSpCost(int skillIndex) { return -1; }
    #endregion

    #region 메인메뉴
    // 메인메뉴에서 캐릭터 클릭시에 애니메이션이 나오도록 하는데 필요함
    public List<AniType> AnimationsWhenUserClick { get; protected set; } = new List<AniType>();
    bool m_IsAnimationAndDialogSet;

    void SetMainMenuAnimations()
    {
        if (m_IsAnimationAndDialogSet)
            return;

        var table = TableManager.Instance.AniTypeDialogueTable.FindAll(row => row.ObjectCode == Code);
        foreach (var row in table)
            AnimationsWhenUserClick.Add((AniType)row.AniType);

        m_IsAnimationAndDialogSet = true;
    }
    #endregion

    #region 파일 데이터
    void SetPropertiesFromTable()
    {
        var table = TableManager.Instance.CharacterTable.Find(c => c.Code == Code);
        var record = GameManager.Instance.PlayerData.CharacterDatas.Find(c => c.Code == Code);

        Name = table.Name;
        Type = table.Type;

        if (record != null)
            Data = record;
        else
            Data = new CharacterData()
            {
                Code = Code,
                Level = 1,
                Hp = table.BaseHp,
                Sp = table.BaseSp,
                Critical = table.BaseCritical,
                Damage = table.BaseDamage,
                Defense = table.BaseDefense,
                Speed = table.BaseSpeed,
                EquipWeaponData = null
            };
    }

    /// <summary>
    /// 플레이어의 캐릭터 데이터를 업데이트 합니다.
    /// </summary>
    public void UpdatePlayerData()
    {
        var player = GameManager.Instance.Player;
        foreach (var cha in player.Characters)
        {
            var playerData = GameManager.Instance.PlayerData;
            var exist = playerData.CharacterDatas.Find(c => c.Code == cha.Code);
            if (exist == null)
                playerData.CharacterDatas.Add(cha.Data);
            else
                exist = cha.Data;
        }
    }
    #endregion

    #region 데미지 계산
    /// <summary>
    /// 상대방에게 입힐 데미지를 계산합니다.
    /// </summary>
    /// <param name="lhs">자신</param>
    /// <param name="rhs">상대방</param>
    /// <returns></returns>
    protected float CalcuateDamage(Character lhs, Character rhs)
    {
        float result = 0;
        result = CalculateTypeDamage(lhs, rhs);
        result = CalculateCriticalDamage(result, lhs.Data.Critical);
        return result;
    }

    /// <summary>
    /// 속성별로 적용되는 데미지를 계산합니다.
    /// </summary>
    /// <param name="lhs">자신</param>
    /// <param name="rhs">상대방</param>
    /// <returns>계산된 데미지</returns>
    float CalculateTypeDamage(Character lhs, Character rhs)
    {
        // 내 속성이 상대방에게 유리한 속성일 경우 30% 추뎀,
        // 내 속성이 상대방에게 불리한 속성일 경우 30%의 데미지만 입힐 수 있음
        // 내 속성과 상대방에게 어떠한 이점이 없을 경우 데미지는 그대로

        float result = 0;
        float bonusRatio = 1;

        switch (lhs.Type)
        {
            case CharacterType.Biology:
                switch (rhs.Type)
                {
                    case CharacterType.Biology:
                        bonusRatio = 1f;
                        break;
                    case CharacterType.Machine:
                        bonusRatio = 0.3f;
                        break;
                    case CharacterType.Supernatural:
                        bonusRatio = 1.3f;
                        break;
                    default:
                        break;
                }
                break;
            case CharacterType.Machine:
                switch (rhs.Type)
                {
                    case CharacterType.Biology:
                        bonusRatio = 1.3f;
                        break;
                    case CharacterType.Machine:
                        bonusRatio = 1f;
                        break;
                    case CharacterType.Supernatural:
                        bonusRatio = 0.3f;
                        break;
                }
                break;
            case CharacterType.Supernatural:
                switch (rhs.Type)
                {
                    case CharacterType.Biology:
                        bonusRatio = 0.3f;
                        break;
                    case CharacterType.Machine:
                        bonusRatio = 1.3f;
                        break;
                    case CharacterType.Supernatural:
                        bonusRatio = 1f;
                        break;
                }
                break;
        }

        result = lhs.Data.Damage * bonusRatio;

        return result;
    }

    /// <summary>
    /// 치명타 확률을 포함한 최종 데미지를 계산합니다.
    /// </summary>
    float CalculateCriticalDamage(float damage, float criticalRate)
    {
        float result = damage;
        bool critical = false;

        float random = Random.Range(0.0f, 1.0f);
        if (criticalRate > random)
            critical = true;
        else
            critical = false;

        if (critical)
            result *= 2;

        return result;
    }
    #endregion
}

/// <summary> 캐릭터 스킬 타입 </summary> ///
public enum SkillType
{
    Attack,
    Skill,
    Dash,
}