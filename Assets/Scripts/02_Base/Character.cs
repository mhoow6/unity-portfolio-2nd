using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.AI;

public class Character : BaseObject
{
    public readonly int ANITYPE_HASHCODE = Animator.StringToHash("AniType");
    public Animator Animator { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public Transform Head;

    public Dictionary<SkillType, int> SkillIndices = new Dictionary<SkillType, int>();
    [ReadOnly] public AniType CurrentAniType;
    
    #region ĳ���� ������
    /// <summary> ��Ͽ� ������. Data�� ���Ͽ� ���� �����ϴ� ������ ������ ���� �� ��</summary> ///
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
    #endregion

    protected void Start()
    {
        // ������Ʈ ���̱�
        if (!Animator)
            Animator = GetComponent<Animator>();

        if (!Agent)
            Agent = GetComponent<NavMeshAgent>();

        SetMainMenuAnimations();
        SetPropertiesFromTable();

        OnSpawn();
    }

    protected void Update()
    {
        OnLive();
    }

    /// <summary> ĳ���� ������ ȣ�� </summary> ///
    protected virtual void OnSpawn() { }

    /// <summary> ĳ���� ����� ȣ�� </summary> ///
    protected virtual void OnDead() { }

    /// <summary> ĳ���� ������� �� ȣ�� </summary> ///
    protected virtual void OnLive() { }

    /// <summary> Damaged ȣ�� �� �ؾ��� �ൿ </summary> ///
    protected virtual void OnDamaged(Character attacker, float updateHp) { }

    #region ����
    /// <summary> �ִϸ��̼� �̺�Ʈ �Լ� </summary> ///
    public virtual void Attack(int skillIndex) { }

    public void Damaged(Character attacker, int damage, DamageType damageType)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Hp = 0;
            OnDead();
            return;
        }

        // TODO: ������ Ÿ�Կ� ���� �ִϸ��̼�
        switch (damageType)
        {
            case DamageType.Normal:
                break;
            case DamageType.Stiffness:
                break;
        }

        OnDamaged(attacker, damage);
    }

    public virtual AniType GetAniType(int skillIndex) { return AniType.NONE; }
    public virtual int GetSpCost(int skillIndex) { return -1; }
    #endregion

    #region ���θ޴�
    // ���θ޴����� ĳ���� Ŭ���ÿ� �ִϸ��̼��� �������� �ϴµ� �ʿ���
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

    #region ���� ������
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
    #endregion

    #region ������ ���
    /// <summary> ���濡�� ���� �������� ����մϴ�. </summary> ///
    /// <returns>int: ������ bool: ũ��Ƽ�� ����</returns>
    public (int, bool) CalcuateDamage(Character rhs)
    {
        (int, bool) result;
        result.Item1 = CalculateTypeDamage(this, rhs);
        result = CalculateCriticalDamage(result.Item1, Data.Critical);
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
}

/// <summary> ĳ���� ��ų Ÿ�� </summary> ///
public enum SkillType
{
    Attack,
    Skill,
    Dash,
}