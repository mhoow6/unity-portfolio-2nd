using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 중심이 사각형 정중앙인 Rect. 스크린좌표계를 사용한다.
/// </summary>
public struct CustomRect
{
    public float Width;
    public float Height;
    public Vector2 Center;

    public readonly float xMin;
    public readonly float xMax;
    public readonly float yMin;
    public readonly float yMax;

    public CustomRect(Vector2 centerPosition, float width, float height)
    {
        Center = centerPosition;
        Width = width;
        Height = height;

        xMin = Center.x - (Width * 0.5f);
        xMax = Center.x + (Width * 0.5f);
        yMin = Center.y - (Height * 0.5f);
        yMax = Center.y + (Height * 0.5f);
    }

    /// <summary> 스크린좌표계의 screenPos가 rect안에 있는지 검사합니다. </summary>
    public bool IsScreenPointInRect(Vector2 screenPos)
    {
        if (screenPos.x >= xMin && screenPos.x <= xMax &&
            screenPos.y >= yMin && screenPos.y <= yMax)
            return true;


        return false;
    }
}
