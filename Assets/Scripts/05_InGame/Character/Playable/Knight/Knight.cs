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
        // �̵��ӵ� ���� ���� (�ڱ� �ڽŸ�)
    }

    public override void OnBInput()
    {
        // ���� ����Ʈ�� �Բ� ���� ���� ���� (ĳ���� ��ü�ÿ��� ����)
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
            // �ִ� ��Ʈ �� ����
            if (hitCount < skillData.MaximumHits)
            {
                // �Ÿ��ȿ� ���� �ִٸ�
                if (Vector3.SqrMagnitude(mob.transform.position - transform.position) < Mathf.Pow(skillData.HitRange, 2))
                {
                    Vector3 normalized = (mob.transform.position - transform.position).normalized;

                    Debug.DrawRay(transform.position, normalized, Color.red, 2f);
                    Debug.DrawRay(transform.position, transform.forward, Color.blue, 2f);

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
}
