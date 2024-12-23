using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using UnityEngine.AI;
using System;

public abstract class Character : BaseObject, ISubscribable, IGameEventListener
{
    #region 애니메이션
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

    #region 물리
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
            // 그로기값은 음수가 될수는 없다.
            int preCalculated = m_Data.GroggyExhaustion + value;
            if (preCalculated < 0)
                return;

            var data = TableManager.Instance.CharacterTable.Find(cha => cha.Code == Code);

            // 그로기 진행중이라면
            if (GroggyRecoverySpeed == data.GroggyRecoverySpeed)
                return;

            m_Data.GroggyExhaustion = value;

            // 그로기 조건 달성시
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

    public int EquipWeaponIndex
    {
        get
        {
            return m_Data.EquipWeaponData.Index;
        }
        set
        {
            m_Data.EquipWeaponData.Index = value;
        }
    }

    /// <summary> objectCode에 맞는 패시브 스킬 인덱스 </summary>
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

    /// <summary> A버튼을 누를 때 나올 인덱스 추출 </summary>
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

    /// <summary> X버튼을 누를 때 나올 인덱스 추출 </summary>
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

    /// <summary> B버튼을 누를 때 나올 인덱스 추출 </summary>
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

    #region 타겟팅
    public Character Target
    {
        get
        {
            return m_Target;
        }
        set
        {
            // 이전타겟과 다른 타겟이 세팅되었다?
            Character prevTarget = m_Target;
            if (prevTarget != null)
            {
                if (!prevTarget.Equals(value))
                    OnTargetUpdate?.Invoke(value);
            }
            else
                OnTargetUpdate?.Invoke(value);

            // 타겟이 세팅되었다
            OnTargetSet(value);
            m_Target = value;
        }
    }
    public Action<Character> OnTargetUpdate;
    /// <summary> 인게임UI 타겟 슬라이더가 Hp를 후킹하고 있는지에 대한 여부 </summary>
    [HideInInspector] public bool TargetSliderHooked;
    #endregion

    #region 캐릭터 피격
    public bool Invulnerable { get; set; }

    /// <summary> 피격을 받아야 되는 상황에 호출 </summary>
    public void Damaged(DamagedParam param)
    {
        if (Invulnerable)
            return;

        // 플레이어 치트
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

        // 데이터
        Hp -= param.Damage;

        // 데미지 텍스트
        var damageText = GameManager.UISystem.Pool.Load<FloatingDamageText>($"{GameManager.GameDevelopSettings.UIResourcePath}/InGame/Effect/FloatingDamage");
        var floatingStartPoint = StageManager.Instance.MainCam.WorldToScreenPoint(Head.position);
        damageText.SetData(param.Damage, param.IsCrit, floatingStartPoint, Head.position);
        damageText.StartFloating();

        // 캐릭터 사망
        if (Hp <= 0)
        {
            Hp = 0;

            // 물리 제거
            Collider.enabled = false;
            if (Rigidbody)
                Rigidbody.isKinematic = true;

            GameEventSystem.RemoveListener(this);

            OnDead(param.Attacker, param.Damage);
        }

        OnDamaged(param);

        // 그로기 공격이라면?
        if (param.GroggyPoint > 0)
            GroggyExhaustion += param.GroggyPoint;
    }
    #endregion

    #region 데미지 계산
    /// <summary>상대방에게 입힐 데미지를 계산합니다. </summary>
    public DamageResult CalcuateDamage(Character opponent, float damageScale)
    {
        DamageResult result;

        // 데미지 계산 순서
        // 무기 -> 상성 -> 크리
        float damage = 0f;
        var weaponData = TableManager.Instance.ItemTable.Find(item => item.Index == m_Data.EquipWeaponData.Index);
        damage += weaponData.Point;

        damage = CalculateTypeDamage(this, opponent);

        var critResult = CalculateCriticalDamage(damage, m_Data.Critical);
        damage = critResult.Item1;

        // 계산된 데미지에 스케일 적용
        damage *= damageScale;

        // 상대방의 방어력에 의한 감소
        damage -= opponent.Defense;
        if (damage < 0)
            damage = 0;

        // 결과
        result.Damage = (int)damage;
        result.IsCrit = critResult.Item2;

        return result;
    }
    #endregion

    public PassiveSkill PassiveSkill;
    public PreloadSettings Preloads;

    public Transform Head;
    public Transform Body;

    /// <summary> 땅바닥에 캐릭터 위치 정확하게 놓기 </summary>
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
        // 테이블에서 데이터를 가져와 세팅하기
        var characterRecord = GameManager.PlayerData.CharacterDatas.Find(cha => cha.Code == Code);
        var characterTableData = TableManager.Instance.CharacterTable.Find(cha => cha.Code == Code);
        if (characterRecord != null)
            m_Data = GetCharacterData(Code, characterRecord.Level, characterRecord.EquipWeaponIndex);
        else
            m_Data = GetCharacterData(Code, 1, -1);

        Name = characterTableData.Name;
        Type = characterTableData.Type;

        // 그로기 세팅
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

    /// <summary> objectCode에 맞는 캐릭터 인스턴싱 </summary>
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

    #region 캐릭터 공격/피격
    /// <summary> Damaged 호출 시 해야할 행동 </summary>
    protected virtual void OnDamaged(DamagedParam param) { }

    protected virtual void OnGroggied() { }

    protected virtual void OnGroggyOut() { }
    #endregion

    #region 타겟팅
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

        // 공격범위 안에 있는 경우
        if (Mathf.Cos(halfAngle) < Vector3.Dot(forward, fromTargetNormalized))
        {
            // 사정거리 안에 있는 경우 
            if (Mathf.Round(Vector3.SqrMagnitude(fromTarget)) <= Mathf.Pow(attackRange, 2))
                return true;
        }
        return false;
    }
    #endregion

    #region 캐릭터 사망시
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
        // 컴포넌트 붙이기
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

    /// <summary> 캐릭터 살아있을 때 호출 </summary>
    protected virtual void OnLive() { }

    /// <summary> 캐릭터 스폰시 호출 </summary>
    protected virtual void OnSpawn() { }

    /// <summary> 캐릭터 사망시 호출 </summary>
    protected virtual void OnDead(Character attacker, int damage) { }

    // -----------------------------------------------------------------------

    #region 물리
    Collider Collider { get; set; }
    #endregion

    #region 캐릭터 데이터
    /// <summary> 기록용 데이터. Data를 통하여 값을 변경하는 행위는 가급적 하지 말 것 </summary>
    [SerializeField] CharacterData m_Data;
    #endregion

    #region 타겟팅
    Character m_Target;
    #endregion

    #region 캐릭터 공격/피격
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

            // 그로기에 걸려 그로기 회복속도가 올라갔을 경우
            if (GroggyRecoverySpeed == groggyRecoverySpeed)
            {
                groggyTimer += 1;

                // 정신을 차릴때가 왔다.
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

    #region 데미지 계산
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
    /// 치명타 확률을 포함한 최종 데미지를 계산합니다.
    /// </summary>
    /// <returns>float: 데미지 bool: 크리티컬</returns>
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