using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#region 설명
// 점 P0과 점 P1를 잇는 선분에서 t지점의 좌표는 M0 = Lerp(P1, P2, t)이다.
// 여기서 새로운 점 P2을 추가하여 선분P2P3에서의 t지점의 좌표는 M1 = Lerp(P2, P3, t)이다.
// 이때, M0와 M1을 선분으로 잇게되면 t에따라 선분은 움직이며, 선분위의 임의의 점 B가 그리는 궤적을 2차 베지어 곡선이라 한다.
// 여기서 점 P3을 추가하여, 같은 과정을 거쳐서 나온 궤적을 3차 베지어 곡선이라 한다.
// Class BeizerCurve는 3차 베지어 곡선의 원리를 이용하여 궤적을 그리는 클래스이다.
#endregion
public class BeizerCurve : MonoBehaviour
{
    public GameObject TargetObject;
    [Range(0, 1)]
    public float Value;

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
    /// 삼차 베지어 곡선을 리턴합니다.
    /// </summary>
    /// <param name="value">0 ~ 1</param>
    public Vector3 Beizered(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float value)
    {
        // 1차 보간
        Vector3 A = Vector3.Lerp(p1, p2, value);
        Vector3 B = Vector3.Lerp(p2, p3, value);
        Vector3 C = Vector3.Lerp(p3, p4, value);

        // 2차 보간
        Vector3 D = Vector3.Lerp(A, B, value);
        Vector3 E = Vector3.Lerp(B, C, value);

        Vector3 F = Vector3.Lerp(D, E, value);

        return F;
    }
    #endregion
}

// 에디터와 스크립트를 같이 있게 하는 어트리뷰트
[CanEditMultipleObjects]
[CustomEditor(typeof(BeizerCurve))]
public class BeizerCurveEditor : Editor
{
    private void OnSceneGUI()
    {
        BeizerCurve Generator = (BeizerCurve)target;

        // Set the colour of the next handle to be drawn:
        Handles.color = Color.green;

        // Scene에 Generator.P1 Gizmo를 추가
        // Generator.P1에 다시 값을 넣음으로써 핸들링 가능
        Generator.P1 = Handles.PositionHandle(Generator.P1, Quaternion.identity);
        Generator.P2 = Handles.PositionHandle(Generator.P2, Quaternion.identity);
        Generator.P3 = Handles.PositionHandle(Generator.P3, Quaternion.identity);
        Generator.P4 = Handles.PositionHandle(Generator.P4, Quaternion.identity);

        // 선 추가
        Handles.DrawLine(Generator.P1, Generator.P2);
        Handles.DrawLine(Generator.P3, Generator.P4);

        // 50개의 베지어 선 추가
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
