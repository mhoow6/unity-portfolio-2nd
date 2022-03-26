using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
public static class Automation
{
    [MenuItem("Automation/Table/Generate Table Class")]
    public static void GenerateTableClass()
    {
        // ������ ���� (UpdateLoadingValues�� �������ֱ� ���� �뵵)
        int dataCount = 0;

        // ���̺� ���� ����ü ����
        string data = string.Empty;
        data += $"namespace TableSystem\n{{\n\t";

        string[] separatingStrings = { "\r\n" };
        var textassets = Resources.LoadAll<TextAsset>("99_Table");
        for (int i = 0; i < textassets.Length; i++)
        {
            var asset = textassets[i];

            // \r\n���� ���ڿ� ����
            string[] lines = asset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

            // 0��°: �ڷ���
            // 1��°: ������
            // 2��°: ����
            // 3��°: ����
            // 4��°���� ������

            if (lines.Length > 3)
                dataCount += lines.Length - 4;

            // ����ü ����
            data += $"public struct {asset.name}\n\t{{\n\t\t";

            // �ʵ� ����
            string[] fieldTypes = lines[0].Split(',');
            string[] fieldNames = lines[1].Split(',');
            string[] fieldSummarys = lines[2].Split(',');
            for (int j = 0; j < fieldTypes.Length; j++)
            {
                data += $"/// <summary> {fieldSummarys[j]} </summary> ///\n\t\t";
                data += j == fieldTypes.Length - 1 ? $"public {fieldTypes[j]} {fieldNames[j]};\n\t" : $"public {fieldTypes[j]} {fieldNames[j]};\n\t\t";
            }

            // ����ü �ݱ�
            data += i == textassets.Length - 1 ? $"}}\n\n" : $"}}\n\n\t";
        }

        // namespace �ݱ�
        data += $"}}";

        FileHelper.DirectoryCheck(
            $"{Application.dataPath}/Scripts/99_Table",
            $"{Application.dataPath}/Scripts/99_Table �� �ش��ϴ� ��ΰ� �����ϴ�.");

        WriteFile($"{Application.dataPath}/Scripts/99_Table/TableInfos.cs", data, () => data = string.Empty);

        // ������ �� ���� ����
        var scriptableObj = Resources.Load<Configuration>("Configuration");
        if (scriptableObj != null)
            scriptableObj.DownloadDataCount = dataCount;
        EditorUtility.SetDirty(scriptableObj);

        // --------------------------------------------------------------------------------------------------------------------------------------------

        // ���̺�Ŵ����� �б����� ����ü ����Ʈ �߰�
        if (!string.IsNullOrEmpty(data))
        {
            string managerData = string.Empty;
            managerData += $"using System.Collections.Generic;\n" +
                           $"using UnityEngine;\n" +
                           $"namespace TableSystem\n{{\n\t" +
                           $"public class TableManager\n\t{{\n\t\t" +
                           $"public static TableManager Instance {{ get; private set; }} = new TableManager();\n\t\t" +
                           $"public int LoadedData {{ get; private set; }} = 0;\n\t\t";

            for (int i = 0; i < textassets.Length; i++)
            {
                var asset = textassets[i];
                managerData += $"public List<{asset.name}> {asset.name}s = new List<{asset.name}>();\n\t\t";
            }

            managerData +=
                $"public void LoadTable()\n\t\t{{\n\t\t\t" +
                $"string[] separatingStrings = {{ \"\\r\\n\" }};";

            // LoadTable() ����
            string loadTable = string.Empty;
            loadTable =
         @"
            var {0}Textasset = Resources.Load<TextAsset>(""99_Table/{0}"");
            string[] {0}Lines = {0}Textasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < {0}Lines.Length; i++)
            {{
                string[] datas = {0}Lines[i].Split(',');
                {0} info;
                {1}
                {0}s.Add(info);
                LoadedData++;
            }}
        ";
            string loadTableInline = string.Empty;
            for (int i = 0; i < textassets.Length; i++)
            {
                var asset = textassets[i];
                string[] lines = asset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                string[] fieldTypes = lines[0].Split(',');
                string[] fieldNames = lines[1].Split(',');

                string inline = string.Empty;
                for (int j = 0; j < fieldTypes.Length; j++)
                {
                    if (fieldTypes[j].Equals("string"))
                        inline += $"info.{fieldNames[j]} = datas[{j}];\n\t\t\t\t";
                    else
                        inline += $"info.{fieldNames[j]} = {fieldTypes[j]}.Parse(datas[{j}]);\n\t\t\t\t";

                }

                loadTableInline += string.Format(loadTable, asset.name, inline);
                //Debug.Log(string.Format(loadTable, asset.name, inline));
            }
            managerData += loadTableInline;
            managerData += $"}}\n";
            managerData += $"\t}}\n";
            managerData += $"}}";

            //Debug.Log(managerData);

            WriteFile($"{Application.dataPath}/Scripts/99_Table/TableManager.cs", managerData);
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Automation/Prefab/Generate Collider")]
    public static void GenerateCollider()
    {
        string path = Application.dataPath + "/Resources/Result/";

        FileHelper.DirectoryCheck(
            path,
            "���丮�� ��� �ݶ��̴� ������ ������ �����߽��ϴ�. ���丮�� ��������� �ٽ� �õ����ּ���."
        );

        ColliderGenerator _instanceRoot = GameObject.FindObjectOfType<ColliderGenerator>();
        if (_instanceRoot == null)
        {
            throw new Exception("���� ������ ColliderGenerator�� ������ ������Ʈ�� ã�� �� �����ϴ�.");
        }

        GameObject instanceRoot; // Temp
        string instancePath;

        if (_instanceRoot.childrenMeshes.Count == 0)
            _instanceRoot.ColliderGenerate();

        for (int i = 0; i < _instanceRoot.childrenMeshes.Count; i++)
        {
            instanceRoot = _instanceRoot.childrenMeshes[i].gameObject;
            instancePath = path + instanceRoot.name + ".prefab";

            PrefabUtility.SaveAsPrefabAsset(instanceRoot, instancePath, out bool success);
            Debug.Log($"{instanceRoot.name} save {success == true}");
        }

        for (int i = 0; i < _instanceRoot.childrenSkinnedMeshes.Count; i++)
        {
            instanceRoot = _instanceRoot.childrenSkinnedMeshes[i].transform.parent.gameObject;
            instancePath = path + instanceRoot.name + ".prefab";

            PrefabUtility.SaveAsPrefabAsset(instanceRoot, instancePath, out bool success);
            Debug.Log($"{instanceRoot.name} save {success == true}");
        }

        Debug.Log("�ڽ� �ݶ��̴� ������ �Ϸ�Ǿ����ϴ�. �ݶ��̴��� ���ʹ� ������Ʈ���� �ٸ��� �ʿ�� �����ϼ���.");
    }

    [MenuItem("Automation/Prefab/Mesh Combined Objects")]
    public static void MeshCombinedObjects()
    {
        // MeshCombine ������Ʈ�� ���� ���ӿ�����Ʈ���� ã�� �迭�� �����Ѵ�. 
        MeshCombine[] _instanceRoots = GameObject.FindObjectsOfType<MeshCombine>();
        if (_instanceRoots.Length == 0)
        {
            throw new Exception("���� ������ MeshCombine�� ������ ������Ʈ�� ã�� �� �����ϴ�.");
        }

        GameObject instanceRoot; // Temp
        Mesh instanceRootMesh; // Temp
        string instancePath; // Temp

        FileHelper.DirectoryCheck(
            $"{Application.dataPath}/Resources/99_MeshCombineObjects",
            "���丮�� ��� �Ž� �Ĺ��ο� ������ �����߽��ϴ�. ���丮�� ��������� �ٽ� �õ����ּ���."
        );

        for (int i = 0; i < _instanceRoots.Length; i++)
        {
            string resultPath = $"{Application.dataPath}/Resources/99_MeshCombineObjects";
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", resultPath, _instanceRoots[i].name + "_Mesh", "asset");

            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path); // For AssetDatabase.CreateAsset

            // ������ �Ž� �Ĺ����� �����Ѵ�.
            _instanceRoots[i].MeshCombineObjects();

            // �Ž� �Ĺ����� �� ���ӿ�����Ʈ
            instanceRoot = _instanceRoots[i].gameObject;

            // �Ž� �Ĺ����� �� ���ӿ�����Ʈ�� ���������� ������ ���� �̸�
            instancePath = resultPath + _instanceRoots[i].name + ".prefab";

            // �Ž� �Ĺ����� �� ���ӿ�����Ʈ�� �Ž��� �ν��Ͻ�
            instanceRootMesh = UnityEngine.Object.Instantiate(_instanceRoots[i].selfMeshFilter.sharedMesh);

            // �ν��Ͻ̵� �Ž��� Asset���ν� �����Ѵ�. ���� �Ž��� ������ ���������� ������ �� �� �Ž��� ���� ä�� �����.
            AssetDatabase.CreateAsset(instanceRootMesh, path);
            AssetDatabase.SaveAssets();

            // �ν��Ͻ� �Ž��� �ٽ� ������ sharedMesh�� �ο��Ѵ�. (�����տ� �Ž� �ڵ� �ο��뵵)
            _instanceRoots[i].selfMeshFilter.sharedMesh = instanceRootMesh;

            // �Ž� �Ĺ��ε� ���ӿ�����Ʈ�� ���������ν� �����Ѵ�.
            PrefabUtility.SaveAsPrefabAsset(instanceRoot, instancePath, out bool success);

            if (success == true)
                Debug.Log(instanceRoot.name + " �ȿ� �����ִ� ������Ʈ���� �Ž� �Ĺ��ο� �����Ͽ����ϴ�!");
            else
                Debug.Log(instanceRoot.name + " �ȿ� �����ִ� ������Ʈ���� �Ž� �Ĺ��ο� �����Ͽ����ϴ�...");

            _instanceRoots[i].Clean();
        }
    }

    [MenuItem("Automation/AssetBundle/Monster Asset Bundles")]
    public static void ExportMonsterBundle()
    {
        AssetBundleBuild[] buildBundles = new AssetBundleBuild[1];
        buildBundles[0].assetBundleName = "monster.pak";
        buildBundles[0].assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("monsters.assetbundle");

        FileHelper.DirectoryCheck(
            $"{Application.dataPath}/AssetBundles/Monster",
            "���丮�� ��� ���¹��� ������ �����߽��ϴ�. ���丮�� ��������� �ٽ� �õ����ּ���."
        );

        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/Monster", buildBundles, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Automation/AssetBundle/All Asset Bundles")]
    public static void ExportAllBundle()
    {
        
    }

    [MenuItem("Automation/Table/Save Scene")]
    public static void SaveScene()
    {
        string filePath = string.Empty;
        filePath = $"{Application.dataPath}/Resources/99_Table/{SceneManager.GetActiveScene().name}.csv";

        using (StreamWriter sw = new StreamWriter(filePath))
        {
            sw.WriteLine("name,xpos,ypos,zpos,xrot,yrot,zrot,xscale,yscale,zscale");

            GameObject parent = GameObject.FindGameObjectWithTag("Terrain");
            WriteSceneData(parent, sw);

            parent = GameObject.FindGameObjectWithTag("Character");
            WriteSceneData(parent, sw);

            parent = GameObject.FindGameObjectWithTag("Interactable");
            WriteSceneData(parent, sw);

            sw.Close();
        }

        AssetDatabase.Refresh();
    }

    static void WriteFile(string filePath, string content, Action onExceptionCB = null)
    {
        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(content);
                sw.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            onExceptionCB?.Invoke();
        }
    }

    static void WriteSceneData(GameObject parent, StreamWriter streamWriter)
    {
        streamWriter.WriteLine(parent.name);
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            streamWriter.WriteLine(
                EraseBracketInName(parent.transform.GetChild(i).name) + "," +
                parent.transform.GetChild(i).position.x + "," +
                parent.transform.GetChild(i).position.y + "," +
                parent.transform.GetChild(i).position.z + "," +
                parent.transform.GetChild(i).rotation.eulerAngles.x + "," +
                parent.transform.GetChild(i).rotation.eulerAngles.y + "," +
                parent.transform.GetChild(i).rotation.eulerAngles.z + "," +
                parent.transform.GetChild(i).localScale.x + "," +
                parent.transform.GetChild(i).localScale.y + "," +
                parent.transform.GetChild(i).localScale.z
                );
        }
    }

    static string EraseBracketInName(string mobName)
    {
        string mobNameWithNoSpace = mobName.Replace(" ", "");
        int index = mobNameWithNoSpace.IndexOf('(');

        if (index == -1)
            return mobName;

        return mobNameWithNoSpace.Remove(index);
    }
}
#endif