using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollideHelper
{
    public static Bounds GetBoundsFromMeshFilter(MeshFilter mf)
    {
        Vector3[] meshVertices = mf.mesh.vertices;
        Bounds result = new Bounds();

        // C# 7.0
        var container =
            (min_x: meshVertices[0].x,
             max_x: meshVertices[0].x,
             min_y: meshVertices[0].y,
             max_y: meshVertices[0].y,
             min_z: meshVertices[0].z,
             max_z: meshVertices[0].z);

        foreach (Vector3 vertex in meshVertices)
        {
            if (vertex.x < container.min_x)
                container.min_x = vertex.x;

            if (vertex.x > container.max_x)
                container.max_x = vertex.x;

            if (vertex.y < container.min_y)
                container.min_y = vertex.y;

            if (vertex.y > container.max_y)
                container.max_y = vertex.y;

            if (vertex.z < container.min_z)
                container.min_z = vertex.z;

            if (vertex.z > container.max_z)
                container.max_z = vertex.z;
        }

        float Length_x = container.max_x - container.min_x;
        float Length_y = container.max_y - container.min_y;
        float Length_z = container.max_z - container.min_z;

        result.center = mf.gameObject.transform.position;
        result.size = new Vector3(Length_x * mf.transform.localScale.x, Length_y * mf.transform.localScale.y, Length_z * mf.transform.localScale.z);

        return result;
    }

    /// <summary>
    /// 해당 이름의 노드를 탐색합니다.
    /// </summary>
    public static void GetNodeObject(Transform root, string nodeName, ref Transform node)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            // 이미 nodeName에 맞는 것을 찾아서 null이 아닐 경우 의미없는 호출을 방지하기 위해 함수 종료
            if (node != null)
                return;

            Transform child = root.GetChild(i);

            if (child.name != nodeName)
            {
                // 자식이 또다른 자식을 가질 경우 자식의 자식들을 탐색
                if (child.childCount != 0)
                    GetNodeObject(child, nodeName, ref node);
            }

            if (child.name == nodeName)
                node = child;
        }
    }

    /// <summary>
    /// 구 충돌을 시도합니다.
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns>충돌 여부</returns>
    public static bool TrySphereCollide(this GameObject lhs, GameObject rhs, float lhsRadius)
    {
        // 화살과 타겟의 충돌처리
        Vector3 offset = lhs.transform.position - rhs.transform.position;
        float sqrLength = offset.sqrMagnitude;

        // 충돌했다면?
        if (sqrLength < lhsRadius * lhsRadius)
            return true;
        return false;
    }

    /// <summary>
    /// AABB 충돌을 시도합니다.
    /// </summary>
    /// <param name="lhsBounds"></param>
    /// <param name="rhsBounds"></param>
    /// <returns></returns>
    public static bool TryAABB(this ref Bounds lhsBounds, ref Bounds rhsBounds)
    {
        if (lhsBounds.Intersects(rhsBounds))
            return true;

        return false;
    }

    /// <summary>
    /// 3D 공간에서 오브젝트가 카메라 안에 있는지 확인합니다.
    /// </summary>
    /// <param name="obj">확인할 오브젝트</param>
    /// <param name="cam">오브젝트를 감시할 카메라</param>
    /// <returns></returns>
    public static bool IsInCamera(this GameObject obj, Camera cam)
    {
        Vector3 screenPoint = cam.WorldToViewportPoint(obj.transform.position);

        // 뷰포트 안에 있는지 확인
        return
            screenPoint.z > 0 &&
            screenPoint.x > 0 &&
            screenPoint.x < 1 &&
            screenPoint.y > 0 &&
            screenPoint.y < 1 &&
            // FarPlane보다 가까이 있는지 확인
            obj.TrySphereCollide(cam.gameObject, cam.farClipPlane);
    }
}