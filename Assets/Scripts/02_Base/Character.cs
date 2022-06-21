using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.AI;
using System;

public abstract class Character : BaseObject, IEventCallable
{
    #region �ִϸ��̼�
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

    #region ����
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

    #region ����Ƽ ������ ����Ŭ
    protected void Awake()
    {
        // ������Ʈ ���̱�
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();

        gameObject.layer = GameManager.GameDevelopSettings.BaseObjectLayermask;

        

        SetPropertiesFromTable();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }
    #endregion

    #region ĳ���� ������
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

    /// <summary> ��Ͽ� ������. Data�� ���Ͽ� ���� �����ϴ� ������ ������ ���� �� �� </summary>
    [SerializeField] CharacterData m_Data;
    #endregion

    #region ĳ���� �⺻
    #region ����
    IEnumerator m_UpdateCoroutine;
    public void SetUpdate(bool value)
    {
        if (m_UpdateCoroutine == null)
            m_UpdateCoroutine = UpdateCoroutine();

        if (value)
            StartCoroutine(m_UpdateCoroutine);
        else
            StopCoroutine(m_UpdateCoroutine);
    }
    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            OnLive();
            yield return null;
        }
    }
    /// <summary> ĳ���� ������� �� ȣ�� </summary>
    protected virtual void OnLive() { }
    #endregion

    #region ź��
    public void Spawn()
    {
        TryAttachToFloor();

        if (gameObject.activeSelf)
            SetUpdate(true);

        OnSpawn();
    }

    /// <summary> ĳ���� ������ ȣ�� </summary>
    protected virtual void OnSpawn() { }
    #endregion

    /// <summary> ĳ���� ����� ȣ�� </summary>
    protected virtual void OnDead(Character attacker, int damage) { }

    #endregion

    #region ĳ���� ��ų
    public PassiveSkill PassiveSkill;
    public bool Invincibility { get; set; }

    #region Ÿ����
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

            var image = GameManager.UISystem.Pool.Load<FloatingLockOnImage>($"{GameManager.GameDevelopSettings.UIResourcePath}/InGame/Effect/FloatingLockOn");
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
    protected FloatingLockOnImage m_TargetLockOnImage;
    #endregion

    #region �뽬
    protected int m_CurrentDashStack;
    bool m_ChargeDashStackCoroutine;

    /// <summary> �뽬��ư(X) ��� �̰� ȣ���Ͽ� �뽬�� �Ѵ�. </summary>
    public void Dash(SkillButtonUI skillButtonUI = null)
    {
        var skillData = GetSkillData(GetDashIndex(Code));
        if (skillData.Stack != 0)
        {
            // ������ �� ���� ��ų�� ����� �� ����.
            if (m_CurrentDashStack == 0)
                return;

            // �뽬�� ���� ���ϴ� ��� ���ϰ� �ؾ��Ѵ�.
            if (CanDash() == false)
                return;

            // �� ��ư�� ������ �뽬�� �������� �Ǿ�����
            GameManager.InputSystem.PressXButton = true;

            m_CurrentDashStack--;
            skillButtonUI.OnStackConsume();

            // ������ �� ��Ÿ�ӿ� �� ���� ��������
            if (!m_ChargeDashStackCoroutine)
                StartCoroutine(ChargeDashStackCoroutine(skillButtonUI));
        }
        else
            GameManager.InputSystem.PressXButton = true;
    }

    IEnumerator ChargeDashStackCoroutine(SkillButtonUI skillButtonUI = null)
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

    protected virtual bool CanDash() { return false; }
    #endregion

    #region ����
    /// <summary> �ִϸ��̼� �̺�Ʈ �Լ� </summary>
    public virtual void Attack(int skillIndex) { }

    /// <summary> �ǰ��� �޾ƾ� �Ǵ� ��Ȳ�� ȣ�� </summary>
    public void Damaged(Character attacker, int damage, bool isCrit)
    {
        if (Invincibility)
            return;

        // ������
        Hp -= damage;

        // ������ �ؽ�Ʈ
        var damageText = GameManager.UISystem.Pool.Load<FloatingDamageText>($"{GameManager.GameDevelopSettings.UIResourcePath}/InGame/Effect/FloatingDamage");
        var floatingStartPoint = StageManager.Instance.MainCam.WorldToScreenPoint(Head.position);
        damageText.SetData(damage, isCrit, floatingStartPoint, Head.position);
        damageText.StartFloating();

        // ĳ���� ���
        if (Hp <= 0)
        {
            Hp = 0;

            // ���� ����
            Collider.enabled = false;
            if (Rigidbody)
                Rigidbody.isKinematic = true;

            OnDead(attacker, damage);
            return;
        }

        OnDamaged(attacker, damage, isCrit);
    }

    /// <summary> Damaged ȣ�� �� �ؾ��� �ൿ </summary>
    protected virtual void OnDamaged(Character attacker, int damage, bool isCrit) { }
    #endregion

    #region �ñر�
    protected int m_CurrentUltiStack;
    bool m_ChargeUltiStackCoroutine;

    /// <summary> �ñر��ư(B) ��� �̰� ȣ���Ͽ� �ñر⸦ �Ѵ�. </summary>
    public void Ultimate(SkillButtonUI skillButtonUI = null)
    {

    }

    IEnumerator ChargeUltimateStackCoroutine(SkillButtonUI skillButtonUI = null)
    {
        yield return null;
    }

    protected virtual bool CanUltimate() { return false; }
    #endregion

    #endregion

    #region ������ ���
    /// <summary>���濡�� ���� �������� ����մϴ�. </summary>
    /// <returns>������, ũ��Ƽ�� ����</returns>
    public (int, bool) CalcuateDamage(Character rhs, float damageScale)
    {
        (int, bool) result;
        result.Item1 = CalculateTypeDamage(this, rhs);
        result.Item1 *= (int)damageScale;

        result = CalculateCriticalDamage(result.Item1, m_Data.Critical);
        return result;
    }

    /// <summary>
    /// �Ӽ����� ����Ǵ� �������� ����մϴ�.
    /// </summary>
    /// <param name="lhs">�ڽ�</param>
    /// <param name="rhs">����</param>
    /// <returns>���� ������</returns>
    int CalculateTypeDamage(Character lhs, Character rhs)
    {
        // �� �Ӽ��� ���濡�� ������ �Ӽ��� ��� 30% �ߵ�,
        // �� �Ӽ��� ���濡�� �Ҹ��� �Ӽ��� ��� 30%�� �������� ���� �� ����
        // �� �Ӽ��� ���濡�� ��� ������ ���� ��� �������� �״��

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
    /// ġ��Ÿ Ȯ���� ������ ���� �������� ����մϴ�.
    /// </summary>
    /// <returns>float: ������ bool: ũ��Ƽ��</returns>
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

    #region ���丮 �޼ҵ�
    /// <summary> objectCode�� �´� ĳ���� �ν��Ͻ� </summary>
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

    /// <summary> objectCode�� �´� �нú� ��ų �ε��� </summary>
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

    /// <summary> objectCode�� �´� �⺻���� �ε��� </summary>
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

    /// <summary> objectCode�� �´� �뽬 �ε��� </summary>
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

    /// <summary> objectCode�� �´� �ñر� �ε��� </summary>
    public static int GetUltimateIndex(ObjectCode objectCode)
    {
        return -1;
    }

    public static Skillable GetSkillData(int skillIndex)
    {
        if (JsonManager.Instance.JsonDatas.TryGetValue(skillIndex, out var origin))
        {
            var data = origin as Skillable;
            return data;
        }
        return null;
    }
    #endregion

    #region ��Ÿ
    public Transform Head;
    public Transform Body;

    /// <summary> ���ٴڿ� ĳ���� ��ġ ��Ȯ�ϰ� ���� </summary>
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

    /// <summary> ���̺�κ��� ������ ���� </summary>
    void SetPropertiesFromTable()
    {
        var table = TableManager.Instance.CharacterTable.Find(c => c.Code == Code);
        var record = GameManager.PlayerData.CharacterDatas.Find(c => c.Code == Code);

        Name = table.Name;
        Type = table.Type;

        if (record != null)
        {
            m_Data = new CharacterData()
            {
                Code = Code,
                Level = record.Level,
                Hp = (int)(table.BaseHp + (table.BaseHp * (record.Level * table.HpIncreaseRatioByLevelUp))),
                Sp = (int)(table.BaseSp + (table.BaseSp * (record.Level * table.SpIncreaseRatioByLevelUp))),
                Critical = (int)(table.BaseCritical + (table.BaseCritical * (record.Level * table.CriticalIncreaseRatioByLevelUp))),
                Damage = (int)(table.BaseDamage + (table.BaseDamage * (record.Level * table.DamageIncreaseRatioByLevelUp))),
                Defense = (int)(table.BaseDefense + (table.BaseDefense * (record.Level * table.DefenseIncreaseRatioByLevelUp))),
                Speed = table.BaseSpeed,
                EquipWeaponData = null
            };
        }
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