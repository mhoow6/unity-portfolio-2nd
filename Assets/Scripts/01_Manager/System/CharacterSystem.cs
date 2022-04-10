using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TableSystem;

public class CharacterSystem : GameSystem
{
    public void Init()
    {
        
    }
    public void Tick()
    {
        
    }

    /// <summary>
    /// 플레이어의 캐릭터 데이터를 업데이트 합니다.
    /// </summary>
    public void UpdatePlayerCharacterDatas()
    {
        var characters = GameObject.FindObjectsOfType<Character>();
        foreach (var cha in characters)
        {
            var playerData = GameManager.Instance.PlayerData;
            var exist = playerData.CharacterDatas.Find(c => c.Code == cha.Code);
            if (exist == null)
                playerData.CharacterDatas.Add(cha.Data);
            else
                exist = cha.Data;
        }
    }

    public CharacterData GetData(CharacterCode code)
    {
        CharacterData result = null;
        var row = TableManager.Instance.CharacterTable.Find(c => c.Code == code);

        result = new CharacterData()
        {
            Code = code,
            Level = 1,
            Hp = row.BaseHp,
            Sp = row.BaseSp,
            Critical = row.BaseCritical,
            Damage = row.BaseDamage,
            Defense = row.BaseDefense,
            EquipWeaponData = null
        };

        return result;
    }

    /// <summary>
    /// 상대방에게 입힐 데미지를 계산합니다.
    /// </summary>
    /// <param name="lhs">자신</param>
    /// <param name="rhs">상대방</param>
    /// <returns></returns>
    public float CalcuateDamage(Character lhs, Character rhs)
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
            result *= criticalRate;

        return result;
    }
}
