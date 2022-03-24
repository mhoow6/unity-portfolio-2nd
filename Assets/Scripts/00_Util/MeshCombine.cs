using UnityEngine;
using System.Collections;

// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombine : MonoBehaviour
{
    public MeshFilter selfMeshFilter;

    MeshFilter[] meshFilters;
    int childCount;
    const int MAX_LIMIT_VERTEX = 65535;

    void Start()
    {
        MeshCombineObjects();
    }

    private void GetChildrenMeshFilter()
    {
        for (int i = 0; i < childCount; i++)
        {
            MeshFilter childMeshFilter = transform.GetChild(i).GetComponent<MeshFilter>();
            meshFilters[i] = childMeshFilter;
        }
    }

    private void ChildrenMeshCombine()
    {
        GetChildrenMeshFilter();

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int totalVertex = 0;

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            totalVertex += meshFilters[i].sharedMesh.vertexCount;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        selfMeshFilter.sharedMesh = new Mesh();

        if (totalVertex > MAX_LIMIT_VERTEX)
        {
            Clean();
            throw new System.Exception(gameObject.name + " 의 컴바인한 매쉬의 버텍스가 " + MAX_LIMIT_VERTEX + "을 넘습니다. \"" + (totalVertex - MAX_LIMIT_VERTEX) + "\" 개의 버텍스를 줄여주세요.");
        }

        selfMeshFilter.sharedMesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }

    public void MeshCombineObjects()
    {
        childCount = transform.childCount;
        meshFilters = new MeshFilter[childCount];
        selfMeshFilter = GetComponent<MeshFilter>();

        ChildrenMeshCombine();
    }

    public void Clean()
    {
        meshFilters = null;
        childCount = 0;
        selfMeshFilter.sharedMesh = null;
        selfMeshFilter = null;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }

    }
}