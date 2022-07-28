using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;
using System.Linq;

public class Knight : Playable
{
    public override ObjectCode Code => ObjectCode.CHAR_Knight;

    public override void OnXInput()
    {
        // 이동속도 증가 버프 (자기 자신만)
    }

    public override void OnBInput()
    {
        // 쉴드 이펙트와 함께 방어력 증가 버프 (캐릭터 교체시에도 적용)
    }

    public void SlashSword()
    {
        var _skillData = GetSkillData(GetAInputDataIndex(ObjectCode.CHAR_Knight));
        var skillData = _skillData as KnightAInputData;

        int hitCount = 0;
        var _monsters = StageManager.Instance.Monsters;
        var monsters = _monsters.OrderBy(mob => Vector3.SqrMagnitude(mob.transform.position - transform.position));
        foreach (var mob in monsters)
        {
            // 최대 히트 수 제한
            if (hitCount < skillData.MaximumHits)
            {
                // 거리안에 몹이 있다면
                if (Vector3.SqrMagnitude(mob.transform.position - transform.position) < Mathf.Pow(skillData.HitRange, 2))
                {
                    Vector3 normalized = (mob.transform.position - transform.position).normalized;

                    Debug.DrawRay(transform.position, normalized, Color.red, 2f);
                    Debug.DrawRay(transform.position, transform.forward, Color.blue, 2f);

                    // 각도안에 몹이 있다면
                    if (Vector3.Dot(transform.forward, normalized) >= Mathf.Cos(skillData.HitAngle * 0.5f * Mathf.Deg2Rad))
                    {
                        // 데미지 계산
                        var result = CalcuateDamage(mob, skillData.DamageScale);

                        // 맞아야지
                        DamagedParam param = new DamagedParam()
                        {
                            Attacker = this,
                            Damage = result.Damage,
                            IsCrit = result.IsCrit,
                            GroggyPoint = 0,
                        };
                        mob.Damaged(param);

                        // Sp 회복
                        Sp += skillData.HitGainSp;

                        hitCount++;
                    }
                }
            }
        }
    }

    // -----------------------------------------------------------------------

    protected override void OnSpawn()
    {
        base.OnSpawn();

        // SkillDatas.json
        PassiveSkill = new KnightPassiveSkill(GetPassiveIndex(Code));
    }
}
