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
        var sm = StageManager.Instance;

        // 이펙트
        var effect = sm.PoolSystem.Load<KnightSpeedBoostEffect>($"{GameManager.GameDevelopSettings.EffectResourcePath}/Knight_SpeedBoost");
        effect.transform.SetPositionAndRotation(transform.position, transform.rotation);

        // 이동속도 증가 버프 (자기 자신만)
        sm.BuffSystem.DoBuff(new KnightSpeedBoost(this));
    }

    public override void OnBInput()
    {
        var sm = StageManager.Instance;

        // 이펙트
        var effect = sm.PoolSystem.Load<KnightShieldEffect>($"{GameManager.GameDevelopSettings.EffectResourcePath}/Knight_Shield");
        effect.transform.SetPositionAndRotation(transform.position, transform.rotation);

        // 방어력 증가 버프 (파티 전체)
        sm.BuffSystem.DoBuff(new KnightShieldBoost(sm.Player.Characters));
    }

    public void SlashSword()
    {
        var _skillData = GetSkillData(GetAInputDataIndex(ObjectCode.CHAR_Knight));
        var skillData = _skillData as KnightAInputData;

        int hitCount = 0;
        bool setTarget = false;

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

                    //Debug.DrawRay(transform.position, normalized, Color.red, 2f);
                    //Debug.DrawRay(transform.position, transform.forward, Color.blue, 2f);

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

                        // 타겟 설정
                        if( setTarget == false)
                        {
                            setTarget = true;
                            Target = mob;
                        }
                            

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

    // -----------------------------------------------------------------------

}
