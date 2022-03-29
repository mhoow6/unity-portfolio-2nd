using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTitle_Road : MonoBehaviour
{
    float Radius;
    public Collider BoxCollider;

    private void Start()
    {
        Radius = BoxCollider.bounds.size.z;
    }

    public void Attach(GameObject obj)
    {
        obj.transform.position = transform.position + new Vector3(0, 0, Radius);
    }
}
