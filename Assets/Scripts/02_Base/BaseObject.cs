using UnityEngine;

public abstract class BaseObject : MonoBehaviour
{
    public virtual ObjectCode Code { get; } = ObjectCode.NONE;
}
