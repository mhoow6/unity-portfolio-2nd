using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingLockOnImage : MonoBehaviour, IPoolable
{
    Transform m_Target;
    RectTransform m_RectTransform;

    const float FLOATING_TIME = 2f;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    public void SetData(Transform target)
    {
        m_Target = target;
    }

    public void SetUpdate(bool value)
    {
        if (value)
            StartCoroutine(UpdateCoroutine());
        else
            StopAllCoroutines();
    }

    IEnumerator UpdateCoroutine()
    {
        float timer = 0f;
        var mainCam = StageManager.Instance.MainCam;

        while (timer < FLOATING_TIME)
        {
            timer += Time.deltaTime;
            m_RectTransform.position = mainCam.WorldToScreenPoint(m_Target.transform.position);

            yield return null;
        }
        GameManager.UISystem.Pool.Release(this);
    }

    #region 오브젝트 풀링
    bool m_Poolable;
    public bool Poolable { get => m_Poolable; set => m_Poolable = value; }

    public void OnLoad()
    {
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
