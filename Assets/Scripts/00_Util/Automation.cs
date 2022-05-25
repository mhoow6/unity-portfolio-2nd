using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
public static class Automation
{
    [MenuItem("Custom Tools/Automation/Database/Generate Game Enum")]
    public static void GenerateGameEnum()
    {
        string result = string.Empty;

        string datas = string.Empty;

        string dataStruct =
@"
{0}
";
        string enums = string.Empty;

        string enumStruct =
@"
    public enum {0}
    {{
        {1}
    }}
";
        string enumValues = string.Empty;

        string enumValueStruct =
@"
        {2}
        {0} = {1},
";

        string[] separatingStrings = { "\r\n" };
        var textasset = Resources.Load<TextAsset>("99_Database/Enum/GameEnum");
        string[] lines = textasset.text.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);

        bool enumStart = false;
        bool dataParsing = false;
        string enumType = string.Empty;
        string enumName = string.Empty;
        string enumValue = string.Empty;
        string enumDesc = string.Empty;
        

        foreach (var line in lines)
        {
            // Enum::****
            // @
            // IDLE,0,����
            // @
            // ,,,
            // Enum::****
            string[] words = line.Split(',');

            // ������ Ÿ�� ����
            if (!enumStart)
            {
                foreach (var word in words)
                {
                    enumStart = word.StartsWith("Enum::");
                    if (enumStart)
                    {
                        enumType = word.Replace("Enum::", "");
                        break;
                    }
                }
            }
            else
            {
                // ������ �Ľ� ����/�� ����
                if (line.StartsWith("@"))
                {
                    dataParsing = !dataParsing;
                }
                else
                {
                    if (dataParsing)
                    {
                        // 0��°: ������ �̸�
                        // 1��°: ������ ����
                        // 2��°: ������ ����
                        enumName = words[0];
                        enumValue = words[1];
                        enumDesc = $"/// <summary> {words[2]} </summary> ///";

                        string s = string.Format(enumValueStruct, enumName, enumValue, enumDesc);
                        enumValues += s;
                    }

                    bool allEmpty = true;
                    foreach (var word in words)
                    {
                        //Debug.Log($"{word} �� ����? {string.IsNullOrEmpty(word)}");
                        allEmpty &= string.IsNullOrEmpty(word);
                    }

                    if (allEmpty)
                    {
                        enumStart = false;
                        enums += string.Format(enumStruct, enumType, enumValues);

                        // ���������� ������� Enum�� datas�� ����ִ´�.
                        datas += enums;

                        // ���� Enum�� ���� Ŭ�������� ����
                        enumValues = string.Empty;
                        enums = string.Empty;
                    }
                }
            }
        }

        result = string.Format(dataStruct, datas);
        //Debug.Log(result);
        FileHelper.WriteFile($"{Application.dataPath}/Scripts/99_Table/GameEnum.cs", result);
        AssetDatabase.Refresh();
        
    }

    [MenuItem("Custom Tools/Automation/Database/Generate Table Class")]
    public static void GenerateTableClass()
    {
        // ������ ����
        int dataCount = 0;

        // ���̺� ���� ����ü ����
        string data = string.Empty;
        data += $"namespace DatabaseSystem\n{{\n\t";

        string[] separatingStrings = { "\r\n" };
        var textassets = Resources.LoadAll<TextAsset>("99_Database/Table");
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

        FileHelper.WriteFile($"{Application.dataPath}/Scripts/99_Table/TableInfos.cs", data, () => data = string.Empty);

        // ������ �� ���� ����
        var scriptableObj = Resources.Load<Configuration>("Configuration");
        if (scriptableObj != null)
            scriptableObj.CSVDataCount = dataCount;
        EditorUtility.SetDirty(scriptableObj);

        // --------------------------------------------------------------------------------------------------------------------------------------------

        // ���̺�Ŵ����� �б����� ����ü ����Ʈ �߰�
        if (!string.IsNullOrEmpty(data))
        {
            string managerData = string.Empty;
            managerData += $"using System.Collections.Generic;\n" +
                           $"using UnityEngine;\n" +
                           $"using System;\n" +
                           $"namespace DatabaseSystem\n{{\n\t" +
                           $"public partial class TableManager\n\t{{\n\t\t" +
                           $"public static TableManager Instance {{ get; private set; }} = new TableManager();\n\t\t" +
                           $"public int LoadedData {{ get; private set; }} = 0;\n\t\t";

            for (int i = 0; i < textassets.Length; i++)
            {
                var asset = textassets[i];
                managerData += $"public List<{asset.name}> {asset.name} = new List<{asset.name}>();\n\t\t";
            }

            managerData +=
                $"public void LoadTable()\n\t\t{{\n\t\t\t" +
                $"string[] separatingStrings = {{ \"\\r\\n\" }};";

            // LoadTable() ����
            string loadTable = string.Empty;
            loadTable =
         @"
            var {0}Textasset = Resources.Load<TextAsset>(""99_Database/Table/{0}"");
            string[] {0}Lines = {0}Textasset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
            for (int i = 4; i < {0}Lines.Length; i++)
            {{
                string[] datas = {0}Lines[i].Split(',');
                {0} info;
                {1}
                {0}.Add(info);
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

                string[] types = new string[] { "char", "int", "long", "double", "string", "ushort", "bool", "float" };

                for (int j = 0; j < fieldTypes.Length; j++)
                {
                    if (fieldTypes[j].Equals("string"))
                        inline += $"info.{fieldNames[j]} = datas[{j}];\n\t\t\t\t";
                    else
                    {
                        // ������ Ÿ������ Ȯ��
                        bool isEnum = true;
                        for (int k = 0; k < types.Length; k++)
                        {
                            if (fieldTypes[j].Equals(types[k]))
                            {
                                isEnum = false;
                                break;
                            }    
                        }

                        if (!isEnum)
                            inline += $"info.{fieldNames[j]} = {fieldTypes[j]}.Parse(datas[{j}]);\n\t\t\t\t";
                        else
                            inline += $"info.{fieldNames[j]} = ({fieldTypes[j]})Enum.Parse(typeof({fieldTypes[j]}),datas[{j}]);\n\t\t\t\t";
                    }
                        

                }

                loadTableInline += string.Format(loadTable, asset.name, inline);
                //Debug.Log(string.Format(loadTable, asset.name, inline));
            }
            managerData += loadTableInline;
            managerData += $"}}\n";
            managerData += $"\t}}\n";
            managerData += $"}}";

            //Debug.Log(managerData);

            FileHelper.WriteFile($"{Application.dataPath}/Scripts/99_Table/TableManager.cs", managerData);
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Custom Tools/Automation/Database/Generate Json Class")]
    public static void GenerateJsonClass()
    {
        // �ش� ��ο� �ִ� json ���ϵ� �ؽ�Ʈ�� �б�
        var textassets = Resources.LoadAll<TextAsset>("99_Database/Json");
        string[] separatingStrings = { "\r\n" };

        // Configuration
        var scriptableObj = Resources.Load<Configuration>("Configuration");
        if (scriptableObj != null)
            scriptableObj.JsonDataCount = 0;

        #region JsonManager String Format
        // {0} jsonManagerLoadJsonFormat
        string jsonManagerFormat =
@"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Newtonsoft.Json;

namespace DatabaseSystem
{{
    public partial class JsonManager
    {{
        public static JsonManager Instance {{ get; private set; }} = new JsonManager();
        public Dictionary<int, JsonDatable> JsonDatas {{ get; private set; }} = new Dictionary<int, JsonDatable>();

        public void LoadJson()
        {{
            {0}
        }}
    }}
}}";
        StringBuilder jsonManagerBuilder = new StringBuilder();

        // {0} ���ϸ�
        // {1} ���ϸ� ���� �ҹ���
        // {2} jsonManagerLoadJsonIterableFormat
        string jsonManagerLoadJsonFormat =
@"var {1} = Resources.Load<TextAsset>(""99_Database/Json/{0}"");
            JSONNode {1}Root = JSONNode.Parse({1}.text);
            {2}";
        StringBuilder iterableFormatBuilder = new StringBuilder();


        // {0} ���ϸ� �ҹ���
        // {1} Ŭ������
        // {2} Ŭ������ ���� �ҹ���
        string jsonManagerLoadJsonIterableFormat =
@"JSONNode {2}Node = {0}Root[""{1}""];
            {3} {2} = JsonConvert.DeserializeObject<{3}>({2}Node.ToString());
            JsonDatas.Add({2}.Index, {2});";
        #endregion

        // �ʵ�� ���� ����Ʈ
        string[] exceptFields = DatabaseSystem.JsonDatable.JsonDataExceptFields;

        foreach (var asset in textassets)
        {
            string fileName = asset.name;
            int dataCount = 0;
            bool parseTrigger = false;

            // \r\n(ĳ��������, ������Ʈ)���� ���ڿ� ����
            string[] lines = asset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

            // 0��°�� ��������°�� �߰�ȣ�� �Ľ� ����/�� �뵵 -> ����
            // Ŭ������: {
            // ...
            // }
            StringBuilder classBuilder = new StringBuilder();
            string classFormat =
@"namespace DatabaseSystem
{{
    public class {0} : {2}
    {{
        {1}
    }}
}}
";
            StringBuilder classNameBuilder = new StringBuilder();
            StringBuilder fieldsBuilder = new StringBuilder();
            string parentName = string.Empty;
            for (int i = 1; i < lines.Length - 1; i++)
            {
                string word = lines[i];

                // �Ľ� ����
                if (word[word.Length - 1] == '{')
                {
                    parseTrigger = !parseTrigger;

                    // Ŭ������ ���
                    int startIdx = word.IndexOf('\"');
                    int endIdx = word.IndexOf('(');
                    for (int j = startIdx + 1; j < endIdx; j++)
                        classNameBuilder.Append(word[j]);

                    // �θ� ���
                    startIdx = endIdx;
                    endIdx = word.IndexOf(')');
                    for (int k = startIdx + 1; k < endIdx; k++)
                        parentName += word[k];

                    // LoadJson() ���ο��� �ݺ��Ǵ� �κ� ��Ʈ�� �ֱ�
                    string loadJsonInner = string.Empty;
                    loadJsonInner = string.Format(
                        jsonManagerLoadJsonIterableFormat,
                        fileName.ToLower(),
                        $"{classNameBuilder}({parentName})".ToString(),
                        classNameBuilder.ToString().ToLower(),
                        classNameBuilder.ToString());

                    iterableFormatBuilder.AppendLine(loadJsonInner);
                    // �鿩���� ���߱�
                    iterableFormatBuilder.AppendLine("");
                    iterableFormatBuilder.Append("            ");
                    continue;
                }
                // �Ľ� ��
                else if (word == "  }," || word == "  }")
                {
                    parseTrigger = !parseTrigger;

                    // Ŭ���� ���� �����
                    string copied = string.Copy(classFormat);
                    copied = string.Format(copied, classNameBuilder, fieldsBuilder, parentName);

                    // ���� Ŭ������ ���� �ʱ�ȭ
                    classNameBuilder.Clear();
                    fieldsBuilder.Clear();
                    parentName = string.Empty;

                    // classBuilder�� �߰�
                    classBuilder.AppendLine(copied);
                    continue;
                }

                if (parseTrigger)
                {
                    string field = string.Empty;

                    // �ʵ�� ���
                    StringBuilder fieldNameBuilder = new StringBuilder();
                    int startIdx = word.IndexOf('\"');
                    int endIdx = FileHelper.FindSpecificChar(word, '\"', 2);
                    for (int j = startIdx + 1; j < endIdx; j++)
                        fieldNameBuilder.Append(word[j]);

                    // �ʵ�� ���� ����Ʈ�� �ִ��� ����
                    bool find = false;
                    foreach (var f in exceptFields)
                    {
                        if (fieldNameBuilder.ToString().Equals(f))
                        {
                            // Configuration�� ������ ������ �ø���
                            if (scriptableObj != null)
                            {
                                scriptableObj.JsonDataCount += 1;
                                EditorUtility.SetDirty(scriptableObj);
                            }
                            // ���������� ��ŵ�ϱ� ���� true
                            find = true;
                            break;
                        }
                    }
                    if (find)
                        continue;
                        
                    // �ʵ� Ÿ�� ���
                    StringBuilder fieldDataBuilder = new StringBuilder();
                    startIdx = FileHelper.FindSpecificChar(word, '\"', 3);
                    endIdx = FileHelper.FindSpecificChar(word, '\"', 4);
                    for (int j = startIdx + 1; j < endIdx; j++)
                        fieldDataBuilder.Append(word[j]);

                    // �Ľ̽õ� ����
                    // int -> float -> bool -> DateTime -> string
                    if (int.TryParse(fieldDataBuilder.ToString(), out _))
                        field = $"public int {fieldNameBuilder};";
                    else if (float.TryParse(fieldDataBuilder.ToString(), out _))
                        field = $"public float {fieldNameBuilder};";
                    else if (bool.TryParse(fieldDataBuilder.ToString(), out _))
                        field = $"public bool {fieldNameBuilder};";
                    else if (DateTime.TryParse(fieldDataBuilder.ToString(), out _))
                        field = $"public DateTime {fieldNameBuilder};";
                    else
                        field = $"public string {fieldNameBuilder};";

                    // StringBuilder�� �߰�
                    dataCount++;
                    // �鿩���� ���߱�
                    fieldsBuilder.AppendLine(field);
                    fieldsBuilder.Append($"        ");
                }
            }

            // ---------------------------------------------------------------------------------------------------

            // JsonDatable Ŭ���� ���� ����
            string result = classBuilder.ToString();
            if (!FileHelper.DirectoryCheck($"{Application.dataPath}/Scripts/99_Json"))
                Debug.LogError($"{Application.dataPath}/Scripts/99_Json �� �ش��ϴ� ��ΰ� �����ϴ�.");
            else
            {
                // ���� ����
                FileHelper.WriteFile($"{Application.dataPath}/Scripts/99_Json/{fileName}.cs", result.Trim());

                // Configuration�� ������ �� ���� ����
                if (scriptableObj != null)
                {
                    scriptableObj.JsonDataCount += dataCount;
                    EditorUtility.SetDirty(scriptableObj);
                }

            }

            // ---------------------------------------------------------------------------------------------------

            // JsonManager.LoadJson() ����
            string loadJson = string.Empty;
            loadJson = string.Format(jsonManagerLoadJsonFormat, fileName, fileName.ToLower(), iterableFormatBuilder.ToString());
            jsonManagerBuilder.Append(loadJson);

            // ���� ������ ���� �ʱ�ȭ
            iterableFormatBuilder.Clear();
        }

        // JsonManager ����
        jsonManagerFormat = string.Format(jsonManagerFormat, jsonManagerBuilder.ToString().Trim());

        // JsonManager.cs ����
        if (!FileHelper.DirectoryCheck($"{Application.dataPath}/Scripts/99_Json"))
            Debug.LogError($"{Application.dataPath}/Scripts/99_Json �� �ش��ϴ� ��ΰ� �����ϴ�.");
        else
            FileHelper.WriteFile($"{Application.dataPath}/Scripts/99_Json/JsonManager.cs", jsonManagerFormat);

        AssetDatabase.Refresh();
    }

    [MenuItem("Custom Tools/Automation/Prefab/Generate Collider")]
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

    [MenuItem("Custom Tools/Automation/Prefab/Mesh Combined Objects")]
    public static void MeshCombinedObjects()
    {
        // MeshCombine ������Ʈ�� ���� ���ӿ�����Ʈ���� ã�� �迭�� �����Ѵ�. 
        MeshCombine[] _instanceRoots = GameObject.FindObjectsOfType<MeshCombine>();
        if (_instanceRoots.Length == 0)
        {
            throw new Exception("���� ������ MeshCombine�� ������ ������Ʈ�� ã�� �� �����ϴ�.");
        }

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
            GameObject instanceRoot; // Temp

            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path); // For AssetDatabase.CreateAsset

            // 1. �Ž� �Ĺ����� �����Ѵ�.
            _instanceRoots[i].MeshCombineObjects();

            // 2. �Ž� �Ĺ����� �� ���ӿ�����Ʈ
            instanceRoot = _instanceRoots[i].gameObject;

            // 3. �Ž� �Ĺ����� �� ���ӿ�����Ʈ�� ���������� ������ ���� �̸�
            instancePath = $"{resultPath}/{_instanceRoots[i].name}.prefab";

            // 4. �Ž� �Ĺ����� �� ���ӿ�����Ʈ�� �Ž��� �ν��Ͻ�
            instanceRootMesh = UnityEngine.Object.Instantiate(_instanceRoots[i].MeshFilter.sharedMesh);

            // �ν��Ͻ̵� �Ž��� Asset���ν� �����Ѵ�. ���� �Ž��� ������ ���������� ������ �� �� �Ž��� ���� ä�� �����.
            AssetDatabase.CreateAsset(instanceRootMesh, path);
            AssetDatabase.SaveAssets();

            // 6. �ν��Ͻ� �Ž��� �ٽ� ������ sharedMesh�� �ο��Ѵ�. (�����տ� �Ž� �ڵ� �ο��뵵)
            _instanceRoots[i].MeshFilter.sharedMesh = instanceRootMesh;

            // 7. �Ž� �Ĺ��ε� ���ӿ�����Ʈ�� ���������ν� �����Ѵ�.
            PrefabUtility.SaveAsPrefabAsset(instanceRoot, instancePath, out bool success);

            if (success == true)
                Debug.Log(instanceRoot.name + " �ȿ� �����ִ� ������Ʈ���� �Ž� �Ĺ��ο� �����Ͽ����ϴ�!");
            else
                Debug.Log(instanceRoot.name + " �ȿ� �����ִ� ������Ʈ���� �Ž� �Ĺ��ο� �����Ͽ����ϴ�...");

            _instanceRoots[i].Clean();

        }
    }

    [MenuItem("Custom Tools/Automation/Obsoleted/Build Monster Asset Bundles")]
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

    [MenuItem("Custom Tools/Automation/Obsoleted/Build All Asset Bundles")]
    public static void ExportAllBundle()
    {
        
    }

    [MenuItem("Custom Tools/Automation/Obsoleted/Save Scene")]
    public static void SaveScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string filePath = $"{Application.dataPath}/Resources/99_Table/{sceneName}.csv";

        using (StreamWriter sw = new StreamWriter(filePath))
        {
            sw.WriteLine("name,xpos,ypos,zpos,xrot,yrot,zrot,xscale,yscale,zscale");

            GameObject parent = null;

            parent = GameObject.Find("03_Terrain");
            if (parent != null)
                FileHelper.WriteSceneData(parent, sw);

            parent = GameObject.Find("99_MeshCombineObjects");
            if (parent != null)
                FileHelper.WriteSceneData(parent, sw);

            parent = GameObject.Find("04_Character");
            if (parent != null)
                FileHelper.WriteSceneData(parent, sw);
        
            parent = GameObject.Find("05_Interactable");
            if (parent != null)
                FileHelper.WriteSceneData(parent, sw);

            parent = GameObject.Find("09_Atmosphere");
            if (parent != null)
                FileHelper.WriteSceneData(parent, sw);

            sw.Close();
        }
        Debug.Log($"{sceneName}.csv Save Completed.");

        AssetDatabase.Refresh();
    }
}
#endif