using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseSystem;

public class KnightShieldEffect : MonoBehaviour, IPoolable
{
    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }
    bool m_Poolable;

    float m_LifeDuration = 0f;
    public float m_LifeTimer = 0f;

    public void OnLoad()
    {
        var skillData = Character.GetSkillData(Character.GetBInputDataIndex(ObjectCode.CHAR_Knight)) as KnightBInputData;
        m_LifeDuration = skillData.BuffDuration;
        m_LifeTimer = 0f;

        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_LifeTimer <= m_LifeDuration)
        {
            m_LifeTimer += Time.deltaTime;

            transform.Rotate(Vector3.up, 1f);

            // 계속 따라다닐 캐릭터
            var followCharacter = StageManager.Instance.Player.CurrentCharacter;
            transform.position = followCharacter.transform.position;
        }
        else
        {
            m_LifeTimer = 0f;
            StageManager.Instance.PoolSystem.Release(this);
        }    
    }
}
