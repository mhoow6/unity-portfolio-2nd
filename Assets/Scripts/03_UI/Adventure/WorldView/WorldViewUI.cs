using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DatabaseSystem;

public class WorldViewUI : Display
{
    public WorldViewContentUI Content;
    ScrollRect m_ScrollRect;
    Vector2 SavedScrollRectPosition;

    protected override void OnAwake()
    {
        m_ScrollRect = GetComponent<ScrollRect>();

        // 초기화 값
        SavedScrollRectPosition = Vector2.down;
    }

    private void OnDisable()
    {
        SaveScrollRectPosition();
    }

    public void SetData(int worldIdx)
    {
        // 스테이지의 처음 부분을 보게끔 하자.
        if (SavedScrollRectPosition == Vector2.down)
            m_ScrollRect.normalizedPosition = Vector2.zero;
        else
            m_ScrollRect.normalizedPosition = SavedScrollRectPosition;

        // 모바일 해상도 대응: Content의 RectTransform.width는 2배, height는 자기 자신의 height랑 같게
        m_ScrollRect.content.sizeDelta = new Vector2(rectTransform.sizeDelta.x * 2, rectTransform.sizeDelta.y);

        // 월드에 알맞는 프리팹 로드하기.
        var row = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == worldIdx);
        var worldObj = Resources.Load<GameObject>($"{GameManager.GameDevelopSettings.UIResourcePath}/Adventure/World/{row.WorldPrefabName}");
        var instObj = Instantiate(worldObj, m_ScrollRect.content.transform);

        // 모바일 해상도 대응: 생성할 월드 프리팹 또한 Content의 사이즈와 일치해야한다.
        var content = instObj.GetComponent<WorldViewContentUI>();
        Content = content;

        Content.SetData(worldIdx, m_ScrollRect.content.sizeDelta);
    }

    public void SaveScrollRectPosition()
    {
        SavedScrollRectPosition = m_ScrollRect.normalizedPosition;
    }
}
