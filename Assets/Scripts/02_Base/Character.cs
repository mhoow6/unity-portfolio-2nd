using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.AI;

public class Character : BaseObject
{
    #region 애니메이션
    public readonly int ANITYPE_HASHCODE = Animator.StringToHash("AniType");
    public readonly int ANISPEED_HASHCODE = Animator.StringToHash("AniSpeed");
    public Animator Animator { get; private set; }
    public float AniSpeed
    {
        get
        {
            return Animator.GetFloat(ANISPEED_HASHCODE);
        }
        set
        {
            Animator.SetFloat(ANISPEED_HASHCODE, value);
        }
    }
    [ReadOnly] public AniType CurrentAniType;
    #endregion

    #region AI
    public NavMeshAgent Agent { get; private set; }
    #endregion

    #region 물리
    public Rigidbody Rigidbody { get; private set; }
    public Collider Collider { get; private set; }
    public bool Physic
    {
        get
        {
            return !Rigidbody.isKinematic && Collider.enabled;
        }
        set
        {
            Rigidbody.isKinematic = !value;
            Collider.enabled = value;
        }
    }
    #endregion

    #region 캐릭터 데이터
    /// <summary> 기록용 데이터. Data를 통하여 값을 변경하는 행위는 가급적 하지 말 것</summary> ///
    public CharacterData Data;
    public string Name { get; private set; }
    public CharacterType Type { get; private set; }
    public int Hp
    {
        get => Data.Hp;
        set
        {
            int hpDelta = value - Data.Hp;
            Data.Hp = value;
        }
    }

    public float Speed
    {
        get => Data.Speed;
        set
        {
            Data.Speed = value;
        }
    }

    public int Damage
    {
        get
        {
            return Data.Damage;
        }
        set
        {
            Data.Damage = value;
        }

    }

    public Dictionary<SkillType, int> SkillIndices = new Dictionary<SkillType, int>();
    #endregion

    public Transform Head;

    #region 캐릭터의 기본
    /// <summary> 캐릭터 스폰시 호출 </summary> ///
    protected virtual void OnSpawn() { }

    /// <summary> 캐릭터 사망시 호출 </summary> ///
    protected virtual void OnDead() { }

    /// <summary> 캐릭터 살아있을 때 호출 </summary> ///
    protected virtual void OnLive() { }

    protected void Start()
    {
        // 컴포넌트 붙이기
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();

        SetMainMenuAnimations();
        SetPropertiesFromTable();

        TryAttachToFloor();

        OnSpawn();
    }
    protected void Update()
    {
        CurrentAniType = (AniType)Animator.GetInteger(ANITYPE_HASHCODE);

        OnLive();
    }
    #endregion

    #region 공격
    /// <summary> 애니메이션 이벤트 함수 </summary> ///
    public virtual void Attack(int skillIndex) { }

    public void Damaged(Character attacker, int damage, DamageType damageType)
    {
        Hp -= damage;

        // 캐릭터 사망
        if (Hp <= 0)
        {
            Hp = 0;

            // 물리 제거
            Collider.enabled = false;
            if (Rigidbody)
                Rigidbody.isKinematic = true;

            OnDead();
            return;
        }

        // TODO: 데미지 타입에 따른 애니메이션
        switch (damageType)
        {
            case DamageType.Normal:
                break;
            case DamageType.Stiffness:
                break;
        }

        OnDamaged(attacker, damage);
    }

    /// <summary> Damaged 호출 시 해야할 행동 </summary> ///
    protected virtual void OnDamaged(Character attacker, float updateHp) { }

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

    #region 데미지 계산
    /// <summary> 상대방에게 입힐 데미지를 계산합니다. </summary> ///
    /// <returns>int: 데미지 bool: 크리티컬 여부</returns>
    public (int, bool) CalcuateDamage(Character rhs)
    {
        (int, bool) result;
        result.Item1 = CalculateTypeDamage(this, rhs);
        result = CalculateCriticalDamage(result.Item1, Data.Critical);
        return result;
    }

    /// <summary>
    /// 속성별로 적용되는 데미지를 계산합니다.
    /// </summary>
    /// <param name="lhs">자신</param>
    /// <param name="rhs">상대방</param>
    /// <returns>계산된 데미지</returns>
    int CalculateTypeDamage(Character lhs, Character rhs)
    {
        // 내 속성이 상대방에게 유리한 속성일 경우 30% 추뎀,
        // 내 속성이 상대방에게 불리한 속성일 경우 30%의 데미지만 입힐 수 있음
        // 내 속성과 상대방에게 어떠한 이점이 없을 경우 데미지는 그대로

        int result = 0;
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

        result = (int)(lhs.Damage * bonusRatio);

        return result;
    }

    /// <summary>
    /// 치명타 확률을 포함한 최종 데미지를 계산합니다.
    /// </summary>
    /// <returns>float: 데미지 bool: 크리티컬</returns>
    (int, bool) CalculateCriticalDamage(float damage, float criticalRate)
    {
        float convertCriticalRate = criticalRate * 0.01f;
        float calcuatedDamage = damage;
        bool critical = false;

        float random = Random.Range(0.0f, 1.0f);
        if (convertCriticalRate > random)
            critical = true;
        else
            critical = false;

        if (critical)
            calcuatedDamage *= 2;

        return ((int)calcuatedDamage, critical);
    }
    #endregion

    /// <summary> 땅바닥에 캐릭터 위치 정확하게 놓기 </summary> ///
    public bool TryAttachToFloor()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hitInfo;

        int layermask = 1 << LayerMask.NameToLayer("Terrain");

        if (UnityEngine.Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layermask))
        {
            transform.position = hitInfo.point;
            return true;
        }
        return false;
    }

    /// <summary> objectCode에 맞는 캐릭터 인스턴싱 </summary> ///
    public static Character Get(ObjectCode objectCode, Transform parent, string resourcePath)
    {
        Character result = null;
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                var tryload = Resources.Load<Sparcher>($"{resourcePath}/sparcher");
                if (tryload)
                    result = Instantiate(tryload, parent);
                break;
            case ObjectCode.CHAR_GreenSpider:
                break;
            case ObjectCode.CHAR_PurpleSpider:
                break;
            case ObjectCode.CHAR_Dummy:
                break;
            case ObjectCode.PROJ_WoodenArrow:
                break;
            case ObjectCode.EFFECT_HexagonWall:
                break;
            case ObjectCode.CHAR_Pirate:
                break;
            case ObjectCode.NONE:
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary> 테이블로부터 데이터 세팅 </summary> ///
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
}

/// <summary> 캐릭터 스킬 타입 </summary> ///
public enum SkillType
{
    Attack,
    Skill,
    Dash,
}