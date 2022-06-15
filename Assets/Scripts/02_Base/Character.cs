using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.AI;
using System;

public class Character : BaseObject, IEventCallable
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
    public AniType AniType
    {
        get
        {
            return (AniType)Animator.GetInteger(ANITYPE_HASHCODE);
        }
    }
    #endregion

    #region AI
    public NavMeshAgent Agent { get; private set; }
    #endregion

    #region 물리
    public Rigidbody Rigidbody { get; private set; }
    Collider Collider { get; set; }
    public bool Physics
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
    public string Name { get; private set; }
    public CharacterType Type { get; private set; }
    public int Hp
    {
        get => m_Data.Hp;
        set
        {
            int hpDelta = value - m_Data.Hp;

            if (value <= MaxHp)
                m_Data.Hp = value;
            else if (value > MaxHp)
                m_Data.Hp = MaxHp;

            OnHpUpdate?.Invoke(value);
        }
    }
    public int MaxHp
    {
        get
        {
            var row = TableManager.Instance.CharacterTable.Find(cha => cha.Code == Code);
            return row.BaseHp + ((int)(row.BaseHp * row.HpIncreaseRatioByLevelUp) * (m_Data.Level - 1));
        }
    }
    public Action<int> OnHpUpdate;

    public int Sp
    {
        get => m_Data.Sp;
        set
        {
            int spDelta = value - m_Data.Sp;

            if (value <= MaxSp)
                m_Data.Sp = value;
            else if (value > MaxSp)
                m_Data.Sp = MaxSp;

            OnSpUpdate?.Invoke(value);
        }
    }
    public int MaxSp
    {
        get
        {
            var row = TableManager.Instance.CharacterTable.Find(cha => cha.Code == Code);
            return row.BaseSp + ((int)(row.BaseSp * row.SpIncreaseRatioByLevelUp) * (m_Data.Level - 1));
        }
    }
    public Action<int> OnSpUpdate;

    public float MoveSpeed
    {
        get => m_Data.Speed;
        set
        {
            m_Data.Speed = value;
        }
    }

    public int Damage
    {
        get
        {
            return m_Data.Damage;
        }
        set
        {
            m_Data.Damage = value;
        }

    }

    /// <summary> 기록용 데이터. Data를 통하여 값을 변경하는 행위는 가급적 하지 말 것 </summary>
    [SerializeField] CharacterData m_Data;
    #endregion

    #region 캐릭터 기본
    public void SetUpdate(bool value)
    {
        if (value)
            StartCoroutine(m_UpdateCoroutine);
        else
            StopCoroutine(m_UpdateCoroutine);
    }
    IEnumerator m_UpdateCoroutine;

    public void Spawn()
    {
        TryAttachToFloor();

        if (gameObject.activeSelf)
            SetUpdate(true);

        OnSpawn();
    }

    void Awake()
    {
        // 컴포넌트 붙이기
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
        gameObject.layer = 6;
        m_UpdateCoroutine = UpdateCoroutine();

        SetPropertiesFromTable();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            OnLive();
            yield return null;
        }
    }

    /// <summary> 캐릭터 스폰시 호출 </summary>
    protected virtual void OnSpawn() { }

    /// <summary> 캐릭터 사망시 호출 </summary>
    protected virtual void OnDead(Character attacker, int damage, DamageType damageType) { }

    /// <summary> 캐릭터 살아있을 때 호출 </summary>
    protected virtual void OnLive() { }
    #endregion

    #region 캐릭터 공격/스킬
    public PassiveSkill PassiveSkill;
    public Character Target
    {
        get
        {
            return m_Target;
        }
        set
        {
            Character prevTarget = m_Target;

            if (m_TargetLockOnImage != null)
            {
                if (!m_TargetLockOnImage.Poolable)
                    GameManager.UISystem.Pool.Release(m_TargetLockOnImage);
            }

            var image = GameManager.UISystem.Pool.Load<FloatingLockOnImage>($"{GameManager.GameDevelopSettings.UIResourcePath}/InGame/FloatingLockOn");
            m_TargetLockOnImage = image;
            image.SetData(value);
            image.SetUpdate(true);
            m_Target = value;

            if (prevTarget != null)
            {
                if (!prevTarget.Equals(value))
                    OnTargetUpdate?.Invoke(value);
            }
            else
                OnTargetUpdate?.Invoke(value);
        }
    }
    Character m_Target;
    public Action<Character> OnTargetUpdate;

    public FloatingLockOnImage AttachedLockOnImage;
    FloatingLockOnImage m_TargetLockOnImage;

    public bool Invincibility { get; set; }

    /// <summary> 피격을 받아야 되는 상황에 호출 </summary>
    public void Damaged(Character attacker, int damage, DamageType damageType)
    {
        if (Invincibility)
            return;

        Hp -= damage;

        // 캐릭터 사망
        if (Hp <= 0)
        {
            Hp = 0;

            // 물리 제거
            Collider.enabled = false;
            if (Rigidbody)
                Rigidbody.isKinematic = true;

            OnDead(attacker, damage, damageType);
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

        OnDamaged(attacker, damage, damageType);
    }

    /// <summary> 애니메이션 이벤트 함수 </summary>
    public virtual void Attack(int skillIndex) { }

    /// <summary> Damaged 호출 시 해야할 행동 </summary>
    protected virtual void OnDamaged(Character attacker, int damage, DamageType damageType) { }

    public float CurrentDashCoolTime
    {
        get
        {
            return m_DashCoolTimeData.Value;
        }
    }
    public float DashCoolTime
    {
        get
        {
            var skillData = GetSkillData(GetDashIndex(Code));
            return skillData.CoolTime;
        }
    }
    CoolTimeData m_DashCoolTimeData = new CoolTimeData();
    /// <summary> 대쉬 쿨타임 적용하기 </summary>
    public void ActiveDashCoolDown(Action<float> onCoolTimeRunning)
    {
        if (!m_DashCoolTimeData.Cooldown)
        {
            var skillData = GetSkillData(GetDashIndex(Code));

            m_DashCoolTimeData.Value = DashCoolTime;

            StartCoroutine(DOCoolTimeCoroutine(m_DashCoolTimeData, skillData.CoolTime, onCoolTimeRunning));
        }
    }

    /// <summary> 캐릭터 쿨타임 적용시 자동 호출 </summary>
    IEnumerator DOCoolTimeCoroutine(CoolTimeData data, float duration, Action<float> onCoolTimeRunning)
    {
        float timer = 0f;
        data.Cooldown = true;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            data.Value -= Time.deltaTime;
            onCoolTimeRunning.Invoke(data.Value);

            yield return null;
        }

        data.Value = 0;
        data.Cooldown = false;
    }
    #endregion

    #region 데미지 계산
    /// <summary>상대방에게 입힐 데미지를 계산합니다. </summary>
    /// <returns>데미지, 크리티컬 여부</returns>
    public (int, bool) CalcuateDamage(Character rhs, float damageScale)
    {
        (int, bool) result;
        result.Item1 = CalculateTypeDamage(this, rhs);
        result.Item1 *= (int)damageScale;

        result = CalculateCriticalDamage(result.Item1, m_Data.Critical);
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

        float random = UnityEngine.Random.Range(0.0f, 1.0f);
        if (convertCriticalRate > random)
            critical = true;
        else
            critical = false;

        if (critical)
            calcuatedDamage *= 2;

        return ((int)calcuatedDamage, critical);
    }
    #endregion

    #region 팩토리 메소드
    /// <summary> objectCode에 맞는 캐릭터 인스턴싱 </summary>
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
            case ObjectCode.CHAR_MonsterPirate:
                break;
        }
        return result;
    }

    /// <summary> objectCode에 맞는 패시브 스킬 인덱스 </summary>
    public static int GetPassiveIndex(ObjectCode objectCode)
    {
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                return 2001;
            default:
                return -1;
        }
    }

    /// <summary> objectCode에 맞는 기본공격 인덱스 </summary>
    public static int GetAttackIndex(ObjectCode objectCode)
    {
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                return 2000;
            default:
                return -1;
        }
    }

    /// <summary> objectCode에 맞는 대쉬 인덱스 </summary>
    public static int GetDashIndex(ObjectCode objectCode)
    {
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                return 2002;
            default:
                return -1;
        }
    }

    /// <summary> objectCode에 맞는 궁극기 인덱스 </summary>
    public static int GetUltimateIndex(ObjectCode objectCode)
    {
        return -1;
    }


    public static Skillable GetSkillData(int skillIndex)
    {
        var origin = JsonManager.Instance.JsonDatas[skillIndex];
        var data = origin as Skillable;
        return data;
    }
    #endregion

    #region 기타
    public Transform Head;
    public Transform Body;

    /// <summary> 땅바닥에 캐릭터 위치 정확하게 놓기 </summary>
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

    /// <summary> 테이블로부터 데이터 세팅 </summary>
    void SetPropertiesFromTable()
    {
        var table = TableManager.Instance.CharacterTable.Find(c => c.Code == Code);
        var record = GameManager.PlayerData.CharacterDatas.Find(c => c.Code == Code);

        Name = table.Name;
        Type = table.Type;

        if (record != null)
            m_Data = record;
        else
            m_Data = new CharacterData()
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

    public void DisposeEvents()
    {
        OnHpUpdate = null;
        OnSpUpdate = null;
        OnTargetUpdate = null;
    }
    #endregion
}