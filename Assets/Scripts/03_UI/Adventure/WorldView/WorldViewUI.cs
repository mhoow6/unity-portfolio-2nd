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

        // �ʱ�ȭ ��
        SavedScrollRectPosition = Vector2.down;
    }

    private void OnDisable()
    {
        SaveScrollRectPosition();
    }

    public void SetData(int worldIdx)
    {
        // ���������� ó�� �κ��� ���Բ� ����.
        if (SavedScrollRectPosition == Vector2.down)
            m_ScrollRect.normalizedPosition = Vector2.zero;
        else
            m_ScrollRect.normalizedPosition = SavedScrollRectPosition;

        // ����� �ػ� ����: Content�� RectTransform.width�� 2��, height�� �ڱ� �ڽ��� height�� ����
        m_ScrollRect.content.sizeDelta = new Vector2(rectTransform.sizeDelta.x * 2, rectTransform.sizeDelta.y);

        // ���忡 �˸´� ������ �ε��ϱ�.
        var row = TableManager.Instance.StageTable.Find(stage => stage.WorldIdx == worldIdx);
        var worldObj = Resources.Load<GameObject>($"{GameManager.GameDevelopSettings.UIResourcePath}/Adventure/World/{row.WorldPrefabName}");
        var instObj = Instantiate(worldObj, m_ScrollRect.content.transform);

        // ����� �ػ� ����: ������ ���� ������ ���� Content�� ������� ��ġ�ؾ��Ѵ�.
        var content = instObj.GetComponent<WorldViewContentUI>();
        Content = content;

        Content.SetData(worldIdx, m_ScrollRect.content.sizeDelta);
    }

    public void SaveScrollRectPosition()
    {
        SavedScrollRectPosition = m_ScrollRect.normalizedPosition;
    }
}
