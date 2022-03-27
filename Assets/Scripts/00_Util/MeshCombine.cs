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
        // �غ� ����
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
        // �ڽ� ������Ʈ���� �Ž����� ���
        for (int j = 0; j < childCount; j++)
        {
            MeshFilter childMeshFilter = transform.GetChild(j).GetComponent<MeshFilter>();
            ChildrenMeshFilter[j] = childMeshFilter;
        }
        // �ڽ��� material�� �θ𿡰� ����
        if (ChildrenMeshFilter.Length > 0)
        {
            meshRenderer.sharedMaterial = ChildrenMeshFilter[0].gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            Material = meshRenderer.sharedMaterial;
        }
            
        // CombineInstance�� �ڽĵ��� �Ž� ����
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

        // ���ؽ��� 65535���� �Ѿ�� ��� CombineMeshes() ���� �۵� X
        if (totalVertex > MAX_LIMIT_VERTEX)
        {
            Clean();
            throw new System.Exception(gameObject.name + " �� �Ĺ����� �Ž��� ���ؽ��� " + MAX_LIMIT_VERTEX + "�� �ѽ��ϴ�. \"" + (totalVertex - MAX_LIMIT_VERTEX) + "\" ���� ���ؽ��� �ٿ��ּ���.");
        }

        MeshFilter.sharedMesh = new Mesh();
        MeshFilter.sharedMesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
}