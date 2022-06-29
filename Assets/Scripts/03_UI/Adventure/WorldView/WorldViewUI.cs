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

        // �ʱ�ȭ ��
        m_ScrollRectNormalizedPosition = Vector2.down;
    }

    private void OnDisable()
    {
        m_ScrollRectNormalizedPosition = m_ScrollRect.normalizedPosition;
    }

    public void SetData(int worldIdx)
    {
        // ���������� ó�� �κ��� ���Բ� ����.
        if (m_ScrollRectNormalizedPosition == Vector2.down)
            m_ScrollRect.normalizedPosition = Vector2.zero;
        else
            m_ScrollRect.normalizedPosition = m_ScrollRectNormalizedPosition;

        // Content�� RectTransform.width�� 2��, height�� �ڱ� �ڽ��� height�� ����
        m_ScrollRect.content.sizeDelta = new Vector2(rectTransform.sizeDelta.x * 2, rectTransform.sizeDelta.y);

        // ���忡 �˸´� ������ �ε��ϱ�.
        var row = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == worldIdx);
        var worldObj = Resources.Load<GameObject>($"{GameManager.GameDevelopSettings.UIResourcePath}/Adventure/World/{row.WorldPrefabName}");
        var instObj = Instantiate(worldObj, m_ScrollRect.content.transform);

        // ������ ���� ������ ���� Content�� ������� ��ġ�ؾ��Ѵ�.
        var content = instObj.GetComponent<WorldViewContentUI>();
        Content = content;

        Content.SetData(worldIdx, m_ScrollRect.content.sizeDelta);
    }
}
