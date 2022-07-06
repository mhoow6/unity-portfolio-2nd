using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.AI;
using System;

public abstract class Character : BaseObject, ISubscribable
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
            return (int)(row.BaseHp + (row.BaseHp * (m_Data.Level * row.HpIncreaseRatioByLevelUp)));
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
            return (int)(row.BaseSp + (row.BaseSp * (m_Data.Level * row.SpIncreaseRatioByLevelUp)));
        }
    }
    public Action<int> OnSpUpdate;

    public float MoveSpeed
    {
        get => m_Data.Speed;
        set
        {
            m_Data.Speed = value;
            OnMoveSpeedUpdate?.Invoke(value);
        }
    }
    public Action<float> OnMoveSpeedUpdate;

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

    public int Level
    {
        get
        {
            return m_Data.Level;
        }
        set
        {
            m_Data.Level = value;
        }
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
            case ObjectCode.CHAR_MonsterPirate:
                return 2004;
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
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                return 2003;
            default:
                return -1;
        }
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

    #region Ÿ����
    public Character Target
    {
        get
        {
            return m_Target;
        }
        set
        {
            // ����Ÿ�ٰ� �ٸ� Ÿ���� ���õǾ���?
            Character prevTarget = m_Target;
            if (prevTarget != null)
            {
                if (!prevTarget.Equals(value))
                    OnTargetUpdate?.Invoke(value);
            }
            else
                OnTargetUpdate?.Invoke(value);

            // Ÿ���� ���õǾ���
            OnTargetSet(value);
            m_Target = value;
        }
    }
    public Action<Character> OnTargetUpdate;
    /// <summary> �ΰ���UI Ÿ�� �����̴��� Hp�� ��ŷ�ϰ� �ִ����� ���� ���� </summary>
    public bool TargetSliderHooked;
    #endregion

    #region ĳ���� ����/�ǰ�
    public bool Invulnerable { get; set; }
    public virtual void Attack() { }

    /// <summary> �ǰ��� �޾ƾ� �Ǵ� ��Ȳ�� ȣ�� </summary>
    public void Damaged(Character attacker, int damage, bool isCrit)
    {
        if (Invulnerable)
            return;

        // �÷��̾� ġƮ
        var player = StageManager.Instance.Player.CurrentCharacter;
        var victim = this;
        if (attacker.Equals(player))
        {
            if (GameManager.CheatSettings.OneShotKill)
                damage = 999999999;
        }
        if (victim.Equals(player))
        {

            if (GameManager.CheatSettings.GodMode)
                damage = 0;
        }

        // ----------------------------------------------------------

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
    #endregion

    #region ĳ���� �뽬
    public int CurrentDashStack;
    public virtual bool CanDash() { return false; }
    #endregion

    #region ĳ���� ����
    public virtual void Jump() { }
    #endregion

    #region ������ ���
    /// <summary>���濡�� ���� �������� ����մϴ�. </summary>
    public DamageResult CalcuateDamage(Character opponent, float damageScale)
    {
        DamageResult result;

        // ������ ��� ����
        float damage;
        damage = CalculateTypeDamage(this, opponent);

        var critResult = CalculateCriticalDamage(damage, m_Data.Critical);
        damage = critResult.Item1;

        // ���� �������� ������ ����
        damage *= damageScale;

        // ���
        result.Damage = (int)damage;
        result.IsCrit = critResult.Item2;

        return result;
    }
    #endregion

    public PassiveSkill PassiveSkill;
    public PreloadSettings Preloads;

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

    public void Spawn()
    {
        TryAttachToFloor();

        if (Preloads)
            Preloads.Instantitate();

        if (gameObject.activeSelf)
            SetUpdate(true);

        OnSpawn();
    }

    public void SetUpdate(bool value)
    {
        if (m_UpdateCoroutine == null)
            m_UpdateCoroutine = UpdateCoroutine();

        if (value)
            StartCoroutine(m_UpdateCoroutine);
        else
            StopCoroutine(m_UpdateCoroutine);
    }

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

    // -----------------------------------------------------------------------

    #region ĳ���� ����/�ǰ�
    /// <summary> Damaged ȣ�� �� �ؾ��� �ൿ </summary>
    protected virtual void OnDamaged(Character attacker, int damage, bool isCrit) { }
    #endregion

    #region Ÿ����
    protected FloatingLockOnImage m_TargetLockOnImage;

    protected virtual void OnTargetSet(Character target) { }
    #endregion

    protected void Awake()
    {
        // ������Ʈ ���̱�
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();

        gameObject.layer = GameManager.GameDevelopSettings.BaseObjectLayermask;

        SetPropertiesFromTable();
    }

    /// <summary> ĳ���� ������� �� ȣ�� </summary>
    protected virtual void OnLive() { }

    /// <summary> ĳ���� ������ ȣ�� </summary>
    protected virtual void OnSpawn() { }

    /// <summary> ĳ���� ����� ȣ�� </summary>
    protected virtual void OnDead(Character attacker, int damage) { }

    // -----------------------------------------------------------------------

    #region ����
    Collider Collider { get; set; }
    #endregion

    #region ĳ���� ������
    /// <summary> ��Ͽ� ������. Data�� ���Ͽ� ���� �����ϴ� ������ ������ ���� �� �� </summary>
    [SerializeField] CharacterData m_Data;
    #endregion

    #region Ÿ����
    Character m_Target;
    #endregion

    #region ������ ���
    /// <summary>
    /// �Ӽ����� ����Ǵ� �������� ����մϴ�.
    /// </summary>
    /// <param name="lhs">�ڽ�</param>
    /// <param name="rhs">����</param>
    /// <returns>���� ������</returns>
    float CalculateTypeDamage(Character lhs, Character rhs)
    {
        // �� �Ӽ��� ���濡�� ������ �Ӽ��� ��� 30% �ߵ�,
        // �� �Ӽ��� ���濡�� �Ҹ��� �Ӽ��� ��� 30%�� �������� ���� �� ����
        // �� �Ӽ��� ���濡�� ��� ������ ���� ��� �������� �״��

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
                        bonusRatio = 1f;
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
                        bonusRatio = 1f;
                        break;
                }
                break;
            case CharacterType.Supernatural:
                switch (rhs.Type)
                {
                    case CharacterType.Biology:
                        bonusRatio = 1f;
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

        result = lhs.Damage * bonusRatio;

        return result;
    }

    /// <summary>
    /// ġ��Ÿ Ȯ���� ������ ���� �������� ����մϴ�.
    /// </summary>
    /// <returns>float: ������ bool: ũ��Ƽ��</returns>
    (float, bool) CalculateCriticalDamage(float damage, float criticalRate)
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

        return (calcuatedDamage, critical);
    }
    #endregion

    IEnumerator m_UpdateCoroutine;

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
                Hp = (int)(table.BaseHp + (table.BaseHp * (1 * table.HpIncreaseRatioByLevelUp))),
                Sp = (int)(table.BaseSp + (table.BaseSp * (1 * table.SpIncreaseRatioByLevelUp))),
                Critical = (int)(table.BaseCritical + (table.BaseCritical * (1 * table.CriticalIncreaseRatioByLevelUp))),
                Damage = (int)(table.BaseDamage + (table.BaseDamage * (1 * table.DamageIncreaseRatioByLevelUp))),
                Defense = (int)(table.BaseDefense + (table.BaseDefense * (1 * table.DefenseIncreaseRatioByLevelUp))),
                Speed = table.BaseSpeed,
                EquipWeaponData = null
            };
    }

    // -----------------------------------------------------------------------

    public void DisposeEvents()
    {
        OnHpUpdate = null;
        OnSpUpdate = null;
        OnTargetUpdate = null;
        TargetSliderHooked = false;
        OnMoveSpeedUpdate = null;
    }
}