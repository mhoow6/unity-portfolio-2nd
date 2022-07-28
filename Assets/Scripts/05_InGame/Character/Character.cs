using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.AI;
using System;

public abstract class Character : BaseObject, ISubscribable, IGameEventListener
{
    #region �ִϸ��̼�
    public readonly int ANITYPE_HASHCODE = Animator.StringToHash("AniType");
    public readonly int ANISPEED_HASHCODE = Animator.StringToHash("AniSpeed");
    public int AnimatorBaseLayerIndex { get; private set; }

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

    public int GroggyExhaustion
    {
        get
        {
            return m_Data.GroggyExhaustion;
        }
        set
        {
            // �׷αⰪ�� ������ �ɼ��� ����.
            int preCalculated = m_Data.GroggyExhaustion + value;
            if (preCalculated < 0)
                return;

            var data = TableManager.Instance.CharacterTable.Find(cha => cha.Code == Code);

            // �׷α� �������̶��
            if (GroggyRecoverySpeed == data.GroggyRecoverySpeed)
                return;

            m_Data.GroggyExhaustion = value;

            // �׷α� ���� �޼���
            int maxGroggy = data.MaxGroggyExhaustion;
            if (maxGroggy > -1)
            {
                if (m_Data.GroggyExhaustion >= maxGroggy)
                {
                    m_Data.GroggyExhaustion = maxGroggy;
                    GroggyRecoverySpeed = data.GroggyRecoverySpeed;
                    OnGroggied();
                }
            }
        }
    }

    public int GroggyRecoverySpeed
    {
        get
        {
            return m_Data.GroggyRecoverySpeed;
        }
        set
        {
            m_Data.GroggyRecoverySpeed = value;
        }
    }

    public int Defense
    {
        get
        {
            return m_Data.Defense;
        }
        set
        {
            m_Data.Defense = value;
        }
    }

    public int Critical
    {
        get
        {
            return m_Data.Critical;
        }
        set
        {
            m_Data.Critical = value;
        }
    }

    /// <summary> objectCode�� �´� �нú� ��ų �ε��� </summary>
    public static int GetPassiveIndex(ObjectCode objectCode)
    {
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                return 2001;
            case ObjectCode.CHAR_Knight:
                return 2008;
            default:
                return -1;
        }
    }

    /// <summary> A��ư�� ���� �� ���� �ε��� ���� </summary>
    public static int GetAInputDataIndex(ObjectCode objectCode)
    {
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                return 2000;
            case ObjectCode.CHAR_MonsterPirate:
                return 2004;
            case ObjectCode.CHAR_MonsterMushroom:
                return 2005;
            case ObjectCode.CHAR_Knight:
                return 2009;
            default:
                return -1;
        }
    }

    /// <summary> X��ư�� ���� �� ���� �ε��� ���� </summary>
    public static int GetXInputDataIndex(ObjectCode objectCode)
    {
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                return 2002;
            case ObjectCode.CHAR_Knight:
                return 2010;
            default:
                return -1;
        }
    }

    /// <summary> B��ư�� ���� �� ���� �ε��� ���� </summary>
    public static int GetBInputDataIndex(ObjectCode objectCode)
    {
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                return 2003;
            case ObjectCode.CHAR_Knight:
                return 2011;
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

    public static string GetName(ObjectCode objectCode)
    {
        var data = TableManager.Instance.CharacterTable.Find(cha => cha.Code == objectCode);
        if (data.Name != string.Empty)
            return data.Name;
        return string.Empty;
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
    [HideInInspector] public bool TargetSliderHooked;
    #endregion

    #region ĳ���� �ǰ�
    public bool Invulnerable { get; set; }

    /// <summary> �ǰ��� �޾ƾ� �Ǵ� ��Ȳ�� ȣ�� </summary>
    public void Damaged(DamagedParam param)
    {
        if (Invulnerable)
            return;

        // �÷��̾� ġƮ
        var player = StageManager.Instance.Player.CurrentCharacter;
        var victim = this;
        if (param.Attacker.Equals(player))
        {
            if (GameManager.CheatSettings.OneShotKill)
                param.Damage = 999999999;
        }
        if (victim.Equals(player))
        {
            if (GameManager.CheatSettings.GodMode)
                param.Damage = 0;
        }

        // ----------------------------------------------------------

        // ������
        Hp -= param.Damage;

        // ������ �ؽ�Ʈ
        var damageText = GameManager.UISystem.Pool.Load<FloatingDamageText>($"{GameManager.GameDevelopSettings.UIResourcePath}/InGame/Effect/FloatingDamage");
        var floatingStartPoint = StageManager.Instance.MainCam.WorldToScreenPoint(Head.position);
        damageText.SetData(param.Damage, param.IsCrit, floatingStartPoint, Head.position);
        damageText.StartFloating();

        // ĳ���� ���
        if (Hp <= 0)
        {
            Hp = 0;

            // ���� ����
            Collider.enabled = false;
            if (Rigidbody)
                Rigidbody.isKinematic = true;

            GameEventSystem.RemoveListener(this);

            OnDead(param.Attacker, param.Damage);
        }

        OnDamaged(param);

        // �׷α� �����̶��?
        if (param.GroggyPoint > 0)
            GroggyExhaustion += param.GroggyPoint;
    }
    #endregion

    #region ������ ���
    /// <summary>���濡�� ���� �������� ����մϴ�. </summary>
    public DamageResult CalcuateDamage(Character opponent, float damageScale)
    {
        DamageResult result;

        // ������ ��� ����
        // ���� -> �� -> ũ��
        float damage = 0f;
        var weaponData = TableManager.Instance.ItemTable.Find(item => item.Index == m_Data.EquipWeaponData.Index);
        damage += weaponData.Point;

        damage = CalculateTypeDamage(this, opponent);

        var critResult = CalculateCriticalDamage(damage, m_Data.Critical);
        damage = critResult.Item1;

        // ���� �������� ������ ����
        damage *= damageScale;

        // ������ ���¿� ���� ����
        damage -= opponent.Defense;
        if (damage < 0)
            damage = 0;

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

        int layermask = 1 << GameManager.GameDevelopSettings.TerrainLayermask;

        if (UnityEngine.Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layermask))
        {
            transform.position = hitInfo.point;
            return true;
        }
        return false;
    }

    public void Spawn()
    {
        // ���̺��� �����͸� ������ �����ϱ�
        var characterRecord = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == Code);
        var characterTableData = TableManager.Instance.CharacterTable.Find(cha => cha.Code == Code);
        if (characterRecord != null)
            m_Data = GetCharacterData(Code, characterRecord.Level, characterRecord.EquipWeaponIndex);
        else
            m_Data = GetCharacterData(Code, 1, -1);

        Name = characterTableData.Name;
        Type = characterTableData.Type;

        // �׷α� ����
        var chaData = TableManager.Instance.CharacterTable.Find(cha => cha.Code == Code);
        if (chaData.MaxGroggyExhaustion != -1)
            StartCoroutine(NaturalGroggyRecoveryCoroutine());

        // -------------------------------------------------------------------------

        TryAttachToFloor();

        if (gameObject.activeSelf)
            SetUpdate(true);

        GameEventSystem.AddListener(this);

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
    public static Character Get(ObjectCode objectCode, Transform parent)
    {
        Character result = null;

        string prefabName = string.Empty;
        switch (objectCode)
        {
            case ObjectCode.CHAR_Sparcher:
                prefabName = "sparcher";
                break;
            case ObjectCode.CHAR_Knight:
                prefabName = "knight";
                break;

        }

        var tryload = Resources.Load<Character>($"{GameManager.GameDevelopSettings.CharacterResourcePath}/{prefabName}");
        if (tryload)
            result = Instantiate(tryload, parent);

        return result;
    }

    public static CharacterData GetCharacterData(ObjectCode objectCode, int level, int equipWeaponIndex)
    {
        var table = TableManager.Instance.CharacterTable.Find(c => c.Code == objectCode);
        var result = new CharacterData()
        {
            Code = objectCode,
            Level = level,
            Hp = (int)(table.BaseHp + (table.BaseHp * (level * table.HpIncreaseRatioByLevelUp))),
            Sp = (int)(table.BaseSp + (table.BaseSp * (level * table.SpIncreaseRatioByLevelUp))),
            Critical = (int)(table.BaseCritical + (table.BaseCritical * (level * table.CriticalIncreaseRatioByLevelUp))),
            Damage = (int)(table.BaseDamage + (table.BaseDamage * (level * table.DamageIncreaseRatioByLevelUp))),
            Defense = (int)(table.BaseDefense + (table.BaseDefense * (level * table.DefenseIncreaseRatioByLevelUp))),
            Speed = table.BaseSpeed,
            EquipWeaponData = new WeaponData(equipWeaponIndex),
            GroggyExhaustion = 0
        };

        result.Critical += (int)result.EquipWeaponData.Critical;
        result.Damage += (int)result.EquipWeaponData.Damage;

        return result;
    }

    // -----------------------------------------------------------------------

    #region ĳ���� ����/�ǰ�
    /// <summary> Damaged ȣ�� �� �ؾ��� �ൿ </summary>
    protected virtual void OnDamaged(DamagedParam param) { }

    protected virtual void OnGroggied() { }

    protected virtual void OnGroggyOut() { }
    #endregion

    #region Ÿ����
    protected FloatingLockOnImage m_TargetLockOnImage;

    protected virtual void OnTargetSet(Character target) { }

    protected bool IsTargetIn(IsTargetInParam param)
    {
        if (param.Target == null)
            return false;

        Vector3 forward = transform.forward;
        Vector3 fromTarget = (Target.transform.position - transform.position);
        Vector3 fromTargetNormalized = fromTarget.normalized;

        float halfAngle = param.DetectAngle * 0.5f * Mathf.Deg2Rad;
        float attackRange = param.DetectRange;

        // ���ݹ��� �ȿ� �ִ� ���
        if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, fromTargetNormalized))
        {
            // �����Ÿ� �ȿ� �ִ� ��� 
            if (Mathf.Round(Vector3.SqrMagnitude(fromTarget)) <= Mathf.Pow(attackRange, 2))
                return true;
        }
        return false;
    }
    #endregion

    #region ĳ���� �����
    float m_DeathTimer;
    const float INVISIBLE_TIME = 3f;

    protected IEnumerator OnDeadCoroutine()
    {
        while (true)
        {
            m_DeathTimer += Time.deltaTime;
            if (m_DeathTimer > INVISIBLE_TIME)
            {
                Destroy(gameObject);
                m_DeathTimer = 0f;
                yield break;
            }
            yield return null;
        }
    }
    #endregion

    protected void Awake()
    {
        // ������Ʈ ���̱�
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();

        AnimatorBaseLayerIndex = Animator.GetLayerIndex("Base Layer");
        gameObject.layer = GameManager.GameDevelopSettings.BaseObjectLayermask;
    }

    protected void OnDestroy()
    {
        StopAllCoroutines();
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

    #region ĳ���� ����/�ǰ�
    IEnumerator NaturalGroggyRecoveryCoroutine()
    {
        var data = TableManager.Instance.CharacterTable.Find(cha => cha.Code == Code);
        int groggyRecoverySpeed = data.GroggyRecoverySpeed;
        int groggyNaturalRecoverySpeed = data.GroggyNaturalRecoverySpeed;
        int groggyTimer = 0;
        int groggyRecoveryMaxTime = data.MaxGroggyExhaustion / groggyRecoverySpeed;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            // �׷α⿡ �ɷ� �׷α� ȸ���ӵ��� �ö��� ���
            if (GroggyRecoverySpeed == groggyRecoverySpeed)
            {
                groggyTimer += 1;

                // ������ �������� �Դ�.
                if (groggyTimer == groggyRecoveryMaxTime)
                {
                    GroggyExhaustion = 0;
                    GroggyRecoverySpeed = groggyNaturalRecoverySpeed;
                    groggyTimer = 0;

                    OnGroggyOut();
                }
            }

            GroggyExhaustion -= GroggyRecoverySpeed;
        }
    }
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
        convertCriticalRate += m_Data.EquipWeaponData.Critical * 0.01f;

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

    // -----------------------------------------------------------------------

    public void DisposeEvents()
    {
        OnHpUpdate = null;
        OnSpUpdate = null;
        OnTargetUpdate = null;
        TargetSliderHooked = false;
        OnMoveSpeedUpdate = null;
    }

    public void Listen(GameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case GameEvent.STAGE_Clear:
            case GameEvent.STAGE_Fail:
                DisposeEvents();
                GameEventSystem.LateRemoveListener(this);
                break;
        }
    }

    public void Listen(GameEvent gameEvent, params object[] args)
    {
        
    }
}