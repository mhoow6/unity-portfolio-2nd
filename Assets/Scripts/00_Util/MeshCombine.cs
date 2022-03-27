using UnityEngine;
using System.Collections;

// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombine : MonoBehaviour
{
    public MeshFilter MeshFilter;
    public Material Material;
    public MeshFilter[] ChildrenMeshFilter;
    MeshRenderer meshRenderer;

    int childCount;
    const int MAX_LIMIT_VERTEX = 65535;

    void Start()
    {
        MeshCombineObjects();
    }


    public void MeshCombineObjects()
    {
        // 준비 과정
        childCount = transform.childCount;
        ChildrenMeshFilter = new MeshFilter[childCount];
        MeshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        // ---

        ChildrenMeshCombine();
    }

    public void Clean()
    {
        ChildrenMeshFilter = null;
        childCount = 0;
        MeshFilter.sharedMesh = null;
        MeshFilter = null;
        Material = null;
        meshRenderer.sharedMaterial = null;

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);
    }

    private void ChildrenMeshCombine()
    {
        // 자식 오브젝트들의 매쉬필터 얻기
        for (int j = 0; j < childCount; j++)
        {
            MeshFilter childMeshFilter = transform.GetChild(j).GetComponent<MeshFilter>();
            ChildrenMeshFilter[j] = childMeshFilter;
        }
        // 자식의 material을 부모에게 적용
        if (ChildrenMeshFilter.Length > 0)
        {
            meshRenderer.sharedMaterial = ChildrenMeshFilter[0].gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            Material = meshRenderer.sharedMaterial;
        }
            
        // CombineInstance에 자식들의 매쉬 적용
        CombineInstance[] combine = new CombineInstance[ChildrenMeshFilter.Length];
        int totalVertex = 0;
        int i = 0;
        while (i < ChildrenMeshFilter.Length)
        {
            combine[i].mesh = ChildrenMeshFilter[i].sharedMesh;
            combine[i].transform = ChildrenMeshFilter[i].transform.localToWorldMatrix;
            totalVertex += ChildrenMeshFilter[i].sharedMesh.vertexCount;
            ChildrenMeshFilter[i].gameObject.SetActive(false);

            i++;
        }

        // 버텍스가 65535개가 넘어가는 경우 CombineMeshes() 정상 작동 X
        if (totalVertex > MAX_LIMIT_VERTEX)
        {
            Clean();
            throw new System.Exception(gameObject.name + " 의 컴바인한 매쉬의 버텍스가 " + MAX_LIMIT_VERTEX + "을 넘습니다. \"" + (totalVertex - MAX_LIMIT_VERTEX) + "\" 개의 버텍스를 줄여주세요.");
        }

        MeshFilter.sharedMesh = new Mesh();
        MeshFilter.sharedMesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
}