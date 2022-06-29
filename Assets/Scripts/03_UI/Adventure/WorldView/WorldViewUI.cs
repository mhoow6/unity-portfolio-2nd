using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class WorldViewUI : Display
{
    public WorldViewContentUI Content;
    ScrollRect m_ScrollRect;
    Vector2 m_ScrollRectNormalizedPosition;

    protected override void OnAwake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();

        // 초기화 값
        m_ScrollRectNormalizedPosition = Vector2.down;
    }

    private void OnDisable()
    {
        m_ScrollRectNormalizedPosition = m_ScrollRect.normalizedPosition;
    }

    public void SetData(int worldIdx)
    {
        // 스테이지의 처음 부분을 보게끔 하자.
        if (m_ScrollRectNormalizedPosition == Vector2.down)
            m_ScrollRect.normalizedPosition = Vector2.zero;
        else
            m_ScrollRect.normalizedPosition = m_ScrollRectNormalizedPosition;

        // Content의 RectTransform.width는 2배, height는 자기 자신의 height랑 같게
        m_ScrollRect.content.sizeDelta = new Vector2(rectTransform.sizeDelta.x * 2, rectTransform.sizeDelta.y);

        // 월드에 알맞는 프리팹 로드하기.
        var row = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == worldIdx);
        var worldObj = Resources.Load<GameObject>($"{GameManager.GameDevelopSettings.UIResourcePath}/Adventure/World/{row.WorldPrefabName}");
        var instObj = Instantiate(worldObj, m_ScrollRect.content.transform);

        // 생성할 월드 프리팹 또한 Content의 사이즈와 일치해야한다.
        var content = instObj.GetComponent<WorldViewContentUI>();
        Content = content;

        Content.SetData(worldIdx, m_ScrollRect.content.sizeDelta);
    }
}
