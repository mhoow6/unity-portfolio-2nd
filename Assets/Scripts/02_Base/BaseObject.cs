using UnityEngine;

public class BaseObject : MonoBehaviour
{
    public virtual ObjectCode Code { get; } = ObjectCode.NONE;
}
