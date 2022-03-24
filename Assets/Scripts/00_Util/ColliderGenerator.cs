using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGenerator : MonoBehaviour
{
    public List<MeshFilter> childrenMeshes = new List<MeshFilter>();
    public List<SkinnedMeshRenderer> childrenSkinnedMeshes = new List<SkinnedMeshRenderer>();

    private Vector3 CORRECT_SCALE = new Vector3(1, 1, 1);

    public struct Line
    {
        public float min_x;
        public float max_x;
        public float min_y;
        public float max_y;
        public float min_z;
        public float max_z;

        public float Length_x => Mathf.Abs(max_x - min_x);
        public float Length_y => Mathf.Abs(max_y - min_y);
        public float Length_z => Mathf.Abs(max_z - min_z);
    }

    private void Start()
    {
        ColliderGenerate();
    }

    public void ColliderGenerate()
    {
        ColliderGenerateFromMeshFilter();
        ColliderGenerateFromSkinnedMesh();
    }

    private void ColliderGenerateFromMeshFilter()
    {
        MeshFilter childMesh; // Temp
        BoxCollider childBox; // // Temp
        Vector3[] childVertices; // Temp
        Line childLine; // Temp

        for (int i = 0; i < transform.childCount; i++)
        {
            childMesh = transform.GetChild(i).GetComponent<MeshFilter>();
            childBox = transform.GetChild(i).GetComponent<BoxCollider>();

            if (childMesh == null)
                continue;

            if (transform.GetChild(i).localScale != CORRECT_SCALE)
            {
                Debug.Log($"\"{transform.GetChild(i).name}\" 이 오브젝트의 스케일이 (1,1,1) 아닙니다. 스케일을 맞춰주세요.");
                continue;
            }

            if (childBox == null)
                childBox = transform.GetChild(i).gameObject.AddComponent<BoxCollider>();

            if (childMesh != null)
            {
                childrenMeshes.Add(childMesh);

                childVertices = childMesh.sharedMesh.vertices;
                childLine.min_x = childVertices[0].x;
                childLine.max_x = childVertices[0].x;
                childLine.min_y = childVertices[0].y;
                childLine.max_y = childVertices[0].y;
                childLine.min_z = childVertices[0].z;
                childLine.max_z = childVertices[0].z;

                foreach (Vector3 vertex in childVertices)
                    MinMaxUpdate(vertex, ref childLine);

                // new Vector3(0, 1.9f, 0);
                childBox.center = new Vector3(0, childLine.Length_y * 0.5f, 0);
                childBox.size = new Vector3(childLine.Length_x, childLine.Length_y, childLine.Length_z);
            }
        }
    }

    private void ColliderGenerateFromSkinnedMesh()
    {
        SkinnedMeshRenderer childSmr;
        BoxCollider childBox;

        for (int i = 0; i < transform.childCount; i++)
        {
            childSmr = transform.GetChild(i).GetComponentInChildren<SkinnedMeshRenderer>();
            childBox = transform.GetChild(i).GetComponent<BoxCollider>();

            if (childSmr == null)
                continue;

            if (transform.GetChild(i).localScale != CORRECT_SCALE)
            {
                Debug.Log($"\"{transform.GetChild(i).name}\" 이 오브젝트의 스케일이 (1,1,1) 아닙니다. 스케일을 맞춰주세요.");
                continue;
            }

            if (childBox == null)
                childBox = transform.GetChild(i).gameObject.AddComponent<BoxCollider>();

            if (childSmr != null)
            {
                childrenSkinnedMeshes.Add(childSmr);

                childBox.center = new Vector3(0, childSmr.bounds.size.y * 0.5f, 0);
                childBox.size = childSmr.bounds.size;
            }
        }
    }

    private void MinMaxUpdate(Vector3 vertex, ref Line container)
    {
        if (vertex.x <= container.min_x)
            container.min_x = vertex.x;

        if (vertex.x > container.max_x)
            container.max_x = vertex.x;

        if (vertex.y <= container.min_y)
            container.min_y = vertex.y;

        if (vertex.y > container.max_y)
            container.max_y = vertex.y;

        if (vertex.z <= container.min_z)
            container.min_z = vertex.z;

        if (vertex.z > container.max_z)
            container.max_z = vertex.z;
    }
}