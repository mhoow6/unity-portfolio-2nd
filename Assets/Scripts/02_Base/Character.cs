using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class Character : BaseObject
{
    public readonly int ANITYPE_HASHCODE = Animator.StringToHash("AniType");
    public Animator Animator { get; private set; }

    #region �ΰ��ӿ� ������
    public CharacterType Type { get; private set; }
    public string Name { get; private set; }
    public Dictionary<SkillType, int> SkillIndices = new Dictionary<SkillType, int>();
    [ReadOnly] public AniType CurrentAniType;
    #endregion

    /// <summary> ��Ͽ� ������ </summary> ///
    public CharacterData Data;

    protected void Start()
    {
        // ������Ʈ ���̱�
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

    #region ����
    /// <summary> �ִϸ��̼� �̺�Ʈ �Լ� </summary> ///
    public virtual void Attack(int skillIndex) { }
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

    /// <summary>
    /// �÷��̾��� ĳ���� �����͸� ������Ʈ �մϴ�.
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

    #region ������ ���
    /// <summary>
    /// ���濡�� ���� �������� ����մϴ�.
    /// </summary>
    /// <param name="lhs">�ڽ�</param>
    /// <param name="rhs">����</param>
    /// <returns></returns>
    protected float CalcuateDamage(Character lhs, Character rhs)
    {
        float result = 0;
        result = CalculateTypeDamage(lhs, rhs);
        result = CalculateCriticalDamage(result, lhs.Data.Critical);
        return result;
    }

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
    /// ġ��Ÿ Ȯ���� ������ ���� �������� ����մϴ�.
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

/// <summary> ĳ���� ��ų Ÿ�� </summary> ///
public enum SkillType
{
    Attack,
    Skill,
    Dash,
}