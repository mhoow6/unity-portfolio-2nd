using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class FloatingDamageText : MonoBehaviour, IPoolable
{
    [SerializeField] TMP_Text m_Text;
    [SerializeField] Animator m_Animator;
    RectTransform m_RectTransform;
    DummyObject m_FollowObject;

    int m_Damage;
    bool m_IsCrit;
    Vector2 m_FloatingStartPoint;

    readonly Color m_COLOR = new Color(255, 255, 255);
    readonly Color m_CRITICAL_COLOR = new Color(255, 0, 0);
    const float m_FLOATING_TIME = 1f;
    const float m_MOVE_SPEED = 0.5f;
    

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    public void SetData(int damage, bool isCrit, Vector2 textStartPoint, Vector3 textFollowObjectStartPoint)
    {
        m_Damage = damage;
        m_IsCrit = isCrit;
        m_FloatingStartPoint = textStartPoint;

        m_Text.text = $"{damage}";
        if (m_IsCrit)
            m_Text.color = m_CRITICAL_COLOR;
        else
            m_Text.color = m_COLOR;

        m_RectTransform.position = new Vector3(textStartPoint.x, textStartPoint.y, 0);

        // �ؽ�Ʈ�� ����ٴ� ������Ʈ
        m_FollowObject = StageManager.Instance.Pool.LoadDummyObject();
        m_FollowObject.transform.position = textFollowObjectStartPoint;
    }

    public void StartFloating()
    {
        StartCoroutine(FloatingCoroutine());
    }

    IEnumerator FloatingCoroutine()
    {
        float timer = 0f;
        var mainCam = GameManager.Instance.MainCam;
        
        // �ؽ�Ʈ ������ �ִϸ��̼�
        m_Animator.SetTrigger("Active");

        // ������ �������� �ؽ�Ʈ ���ϱ�
        float angle = Random.Range(-5, 5);
        Vector3 textDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(Mathf.Abs(angle)));

        while (timer < m_FLOATING_TIME)
        {
            timer += Time.deltaTime;

            // 3D ������Ʈ�� ��ġ�� 2D ������Ʈ�� �ؽ�Ʈ�� ����
            m_FollowObject.transform.position += textDirection * Time.deltaTime * m_MOVE_SPEED;
            m_RectTransform.position = mainCam.WorldToScreenPoint(m_FollowObject.transform.position);

            yield return null;
        }
        GameManager.Instance.UISystem.Pool.Release(this);
    }

    #region ������Ʈ Ǯ��
    bool m_Poolable;
    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }
    public void OnLoad()
    {
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);

        m_Damage = 0;
        m_IsCrit = false;
        StageManager.Instance.Pool.Release(m_FollowObject);
    }
    #endregion
}
