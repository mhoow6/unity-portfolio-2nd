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
    /// �ش� �̸��� ��带 Ž���մϴ�.
    /// </summary>
    public static void GetNodeObject(Transform root, string nodeName, ref Transform node)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            // �̹� nodeName�� �´� ���� ã�Ƽ� null�� �ƴ� ��� �ǹ̾��� ȣ���� �����ϱ� ���� �Լ� ����
            if (node != null)
                return;

            Transform child = root.GetChild(i);

            if (child.name != nodeName)
            {
                // �ڽ��� �Ǵٸ� �ڽ��� ���� ��� �ڽ��� �ڽĵ��� Ž��
                if (child.childCount != 0)
                    GetNodeObject(child, nodeName, ref node);
            }

            if (child.name == nodeName)
                node = child;
        }
    }

    /// <summary>
    /// �� �浹�� �õ��մϴ�.
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns>�浹 ����</returns>
    public static bool TrySphereCollide(this GameObject lhs, GameObject rhs, float lhsRadius)
    {
        // ȭ��� Ÿ���� �浹ó��
        Vector3 offset = lhs.transform.position - rhs.transform.position;
        float sqrLength = offset.sqrMagnitude;

        // �浹�ߴٸ�?
        if (sqrLength < lhsRadius * lhsRadius)
            return true;
        return false;
    }

    /// <summary>
    /// AABB �浹�� �õ��մϴ�.
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
    /// 3D �������� ������Ʈ�� ī�޶� �ȿ� �ִ��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="obj">Ȯ���� ������Ʈ</param>
    /// <param name="cam">������Ʈ�� ������ ī�޶�</param>
    /// <returns></returns>
    public static bool IsInCamera(this GameObject obj, Camera cam)
    {
        Vector3 screenPoint = cam.WorldToViewportPoint(obj.transform.position);

        // ����Ʈ �ȿ� �ִ��� Ȯ��
        return
            screenPoint.z > 0 &&
            screenPoint.x > 0 &&
            screenPoint.x < 1 &&
            screenPoint.y > 0 &&
            screenPoint.y < 1 &&
            // FarPlane���� ������ �ִ��� Ȯ��
            obj.TrySphereCollide(cam.gameObject, cam.farClipPlane);
    }
}