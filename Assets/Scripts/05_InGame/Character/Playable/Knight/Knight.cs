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

        // ����Ʈ
        var effect = sm.PoolSystem.Load<KnightSpeedBoostEffect>($"{GameManager.GameDevelopSettings.EffectResourcePath}/Knight_SpeedBoost");
        effect.transform.SetPositionAndRotation(transform.position, transform.rotation);

        // �̵��ӵ� ���� ���� (�ڱ� �ڽŸ�)
        sm.BuffSystem.DoBuff(new KnightSpeedBoost(this));
    }

    public override void OnBInput()
    {
        var sm = StageManager.Instance;

        // ����Ʈ
        var effect = sm.PoolSystem.Load<KnightShieldEffect>($"{GameManager.GameDevelopSettings.EffectResourcePath}/Knight_Shield");
        effect.transform.SetPositionAndRotation(transform.position, transform.rotation);

        // ���� ���� ���� (��Ƽ ��ü)
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
            // �ִ� ��Ʈ �� ����
            if (hitCount < skillData.MaximumHits)
            {
                // �Ÿ��ȿ� ���� �ִٸ�
                if (Vector3.SqrMagnitude(mob.transform.position - transform.position) < Mathf.Pow(skillData.HitRange, 2))
                {
                    Vector3 normalized = (mob.transform.position - transform.position).normalized;

                    //Debug.DrawRay(transform.position, normalized, Color.red, 2f);
                    //Debug.DrawRay(transform.position, transform.forward, Color.blue, 2f);

                    // �����ȿ� ���� �ִٸ�
                    if (Vector3.Dot(transform.forward, normalized) >= Mathf.Cos(skillData.HitAngle * 0.5f * Mathf.Deg2Rad))
                    {
                        // ������ ���
                        var result = CalcuateDamage(mob, skillData.DamageScale);

                        // �¾ƾ���
                        DamagedParam param = new DamagedParam()
                        {
                            Attacker = this,
                            Damage = result.Damage,
                            IsCrit = result.IsCrit,
                            GroggyPoint = 0,
                        };
                        mob.Damaged(param);

                        // Ÿ�� ����
                        if( setTarget == false)
                        {
                            setTarget = true;
                            Target = mob;
                        }
                            

                        // Sp ȸ��
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
