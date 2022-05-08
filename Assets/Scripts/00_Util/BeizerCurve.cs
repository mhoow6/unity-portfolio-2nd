using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#region ����
// P1�� P2�� �մ� ���п���, t������ ��ǥ�� Lerp(P1,P2, t)�̴�.
// 4���� ������ �����Ͽ�, �� ������ t ������ ��ǥ�� ���ϴ� ������ 1�� �����̶�� ��.
// 3�� �������� �Ͽ� ���� �� x�� �����µ�, �̶� t�� ���������� ������Ű�� 3�������� ����ϴ� ������ ������ ������ ��� ����� �ִ�.
// �̰��� ������ ��̶� �Ѵ�.
#endregion
public class BeizerCurve : MonoBehaviour
{
    public GameObject TargetObject;
    [Range(0, 1)]
    public float Value;

    Action m_SimulateCB;

    public void Simulate(float endTime, Action endCallback = null)
    {
        StartCoroutine(SimulateCoroutine(endTime, endCallback));
    }

    IEnumerator SimulateCoroutine(float endTime, Action endCallback)
    {
        float timer = 0f;
        while (timer < endTime)
        {
            timer += Time.deltaTime;
            Value = timer / endTime;

            TargetObject.transform.position = Beizered(P1, P2, P3, P4, Value);
            yield return null;
        }
        endCallback?.Invoke();
    }

    #region Core
    public Vector3 P1;
    public Vector3 P2;
    public Vector3 P3;
    public Vector3 P4;

    /// <summary>
    /// ���� ������ ��� �����մϴ�.
    /// </summary>
    /// <param name="value">0 ~ 1</param>
    public Vector3 Beizered(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float value)
    {
        // 1�� ����
        Vector3 A = Vector3.Lerp(p1, p2, value);
        Vector3 B = Vector3.Lerp(p2, p3, value);
        Vector3 C = Vector3.Lerp(p3, p4, value);

        // 2�� ����
        Vector3 D = Vector3.Lerp(A, B, value);
        Vector3 E = Vector3.Lerp(B, C, value);

        Vector3 F = Vector3.Lerp(D, E, value);

        return F;
    }
    #endregion
}

// �����Ϳ� ��ũ��Ʈ�� ���� �ְ� �ϴ� ��Ʈ����Ʈ
[CanEditMultipleObjects]
[CustomEditor(typeof(BeizerCurve))]
public class BeizerCurveEditor : Editor
{
    private void OnSceneGUI()
    {
        BeizerCurve Generator = (BeizerCurve)target;

        // Set the colour of the next handle to be drawn:
        Handles.color = Color.green;

        // Scene�� Generator.P1 Gizmo�� �߰�
        // Generator.P1�� �ٽ� ���� �������ν� �ڵ鸵 ����
        Generator.P1 = Handles.PositionHandle(Generator.P1, Quaternion.identity);
        Generator.P2 = Handles.PositionHandle(Generator.P2, Quaternion.identity);
        Generator.P3 = Handles.PositionHandle(Generator.P3, Quaternion.identity);
        Generator.P4 = Handles.PositionHandle(Generator.P4, Quaternion.identity);

        // �� �߰�
        Handles.DrawLine(Generator.P1, Generator.P2);
        Handles.DrawLine(Generator.P3, Generator.P4);

        // 50���� ������ �� �߰�
        int detail = 50;
        for (float i = 0; i < detail; i++)
        {
            float beforeValue = i / detail;
            float afterValue = (i+1) / detail;
            Vector3 before = Generator.Beizered(Generator.P1, Generator.P2, Generator.P3, Generator.P4, beforeValue);
            Vector3 after = Generator.Beizered(Generator.P1, Generator.P2, Generator.P3, Generator.P4, afterValue);

            Handles.DrawLine(before, after);
        }
    }
}
