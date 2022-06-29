using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display : MonoBehaviour
{
    protected RectTransform rectTransform;

    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        OnAwake();
    }

    public virtual void SetData() { }
    protected virtual void OnAwake() { }
}
