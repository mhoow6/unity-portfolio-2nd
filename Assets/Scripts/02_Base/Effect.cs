using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : BaseObject, IPoolable
{
    public float Duration => m_ParticleSystem.main.duration;

    public Color EffectColor
    {
        get
        {
            var mainModule = m_ParticleSystem.main;
            return new Color(mainModule.startColor.color.r, mainModule.startColor.color.g, mainModule.startColor.color.b, mainModule.startColor.color.a);
        }
        set
        {
            var mainModule = m_ParticleSystem.main;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(value);
        }
    }

    [SerializeField] protected ParticleSystem m_ParticleSystem;

    /// <summary> Pool���� ��ƼŬ�� �������� �� �ؾ��� �ൿ </summary>
    protected virtual void OnPoolLoaded() { }

    /// <summary> Pool�� ��ƼŬ�� �Ѱ��ٶ� �ؾ��� �ൿ </summary>
    protected virtual void OnPoolReleased() { }

    protected IEnumerator AutoRelease()
    {
        yield return new WaitUntil(() => !m_ParticleSystem.isPlaying);
        StageManager.Instance.PoolSystem.Release(this);
        gameObject.SetActive(false);
    }

    #region ������Ʈ Ǯ��
    bool m_Poolable;
    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }

    public void OnLoad()
    {
        gameObject.SetActive(true);
        m_ParticleSystem.Play(true);
        OnPoolLoaded();
    }

    public void OnRelease()
    {
        OnPoolReleased();
    }
    #endregion
}
