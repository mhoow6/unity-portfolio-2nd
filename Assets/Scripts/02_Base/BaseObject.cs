using UnityEngine;
using System.Collections;

public abstract class BaseObject : MonoBehaviour
{
    public virtual ObjectCode Code { get; } = ObjectCode.NONE;

    public void MoveInParabola(in MoveInParabolaParam param)
    {
        StartCoroutine(MoveInParabolaCoroutine(param));
    }

    IEnumerator MoveInParabolaCoroutine(MoveInParabolaParam param)
    {
        float timer = 0f;
        while (timer < param.SimulateTime)
        {
            timer += Time.deltaTime;

            transform.position = MathParabola.Parabola(param.StartPosition, param.EndPosition, param.Height, timer / param.SimulateTime);

            yield return null;
        }
    }

}
