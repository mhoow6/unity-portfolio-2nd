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
    [MenuItem("Automation/Table/Generate Game Enum")]
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
        var textasset = Resources.Load<TextAsset>("99_Table/Enum/GameEnum");
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
            // IDLE,0,설명
            // @
            // ,,,
            // Enum::****
            string[] words = line.Split(',');

            // 열거형 타입 얻어내기
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
                // 데이터 파싱 시작/끝 여부
                if (line.StartsWith("@"))
                {
                    dataParsing = !dataParsing;
                }
                else
                {
                    if (dataParsing)
                    {
                        // 0번째: 열거형 이름
                        // 1번째: 열거형 숫자
                        // 2번째: 열거형 설명
                        enumName = words[0];
                        enumValue = words[1];
                        enumDesc = $"/// <summary> {words[2]} </summary> ///";

                        string s = string.Format(enumValueStruct, enumName, enumValue, enumDesc);
                        enumValues += s;
                    }

                    bool allEmpty = true;
                    foreach (var word in words)
                    {
                        //Debug.Log($"{word} 는 빈문자? {string.IsNullOrEmpty(word)}");
                        allEmpty &= string.IsNullOrEmpty(word);
                    }

                    if (allEmpty)
                    {
                        enumStart = false;
                        enums += string.Format(enumStruct, enumType, enumValues);

                        // 최종적으로 만들어진 Enum을 datas에 집어넣는다.
                        datas += enums;

                        // 다음 Enum을 위해 클리어해줄 변수
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

    [MenuItem("Automation/Table/Generate Table Class")]
    public static void GenerateTableClass()
    {
        // 데이터 갯수
        int dataCount = 0;

        // 테이블에 따른 구조체 생성
        string data = string.Empty;
        data += $"namespace TableSystem\n{{\n\t";

        string[] separatingStrings = { "\r\n" };
        var textassets = Resources.LoadAll<TextAsset>("99_Table/Table");
        for (int i = 0; i < textassets.Length; i++)
        {
            var asset = textassets[i];

            // \r\n으로 문자열 구분
            string[] lines = asset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

            // 0번째: 자료형
            // 1번째: 변수명
            // 2번째: 설명
            // 3번째: 구분
            // 4번째부터 데이터

            if (lines.Length > 3)
                dataCount += lines.Length - 4;

            // 구조체 선언
            data += $"public struct {asset.name}\n\t{{\n\t\t";

            // 필드 선언
            string[] fieldTypes = lines[0].Split(',');
            string[] fieldNames = lines[1].Split(',');
            string[] fieldSummarys = lines[2].Split(',');
            for (int j = 0; j < fieldTypes.Length; j++)
            {
                data += $"/// <summary> {fieldSummarys[j]} </summary> ///\n\t\t";
                data += j == fieldTypes.Length - 1 ? $"public {fieldTypes[j]} {fieldNames[j]};\n\t" : $"public {fieldTypes[j]} {fieldNames[j]};\n\t\t";
            }

            // 구조체 닫기
            data += i == textassets.Length - 1 ? $"}}\n\n" : $"}}\n\n\t";
        }

        // namespace 닫기
        data += $"}}";

        FileHelper.DirectoryCheck(
            $"{Application.dataPath}/Scripts/99_Table",
            $"{Application.dataPath}/Scripts/99_Table 에 해당하는 경로가 없습니다.");

        FileHelper.WriteFile($"{Application.dataPath}/Scripts/99_Table/TableInfos.cs", data, () => data = string.Empty);

        // 데이터 총 갯수 저장
        var scriptableObj = Resources.Load<Configuration>("Configuration");
        if (scriptableObj != null)
            scriptableObj.DownloadDataCount = dataCount;
        EditorUtility.SetDirty(scriptableObj);

        // --------------------------------------------------------------------------------------------------------------------------------------------

        // 테이블매니저에 읽기전용 구조체 리스트 추가
        if (!string.IsNullOrEmpty(data))
        {
            string managerData = string.Empty;
            managerData += $"using System.Collections.Generic;\n" +
                           $"using UnityEngine;\n" +
                           $"using System;\n" +
                           $"namespace TableSystem\n{{\n\t" +
                           $"public class TableManager\n\t{{\n\t\t" +
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

            // LoadTable() 구현
            string loadTable = string.Empty;
            loadTable =
         @"
            var {0}Textasset = Resources.Load<TextAsset>(""99_Table/Table/{0}"");
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
                        // 열거형 타입인지 확인
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

    [MenuItem("Automation/Table/Generate Json Class")]
    public static void GenerateJsonClass()
    {
        // 해당 경로에 있는 json 파일들 텍스트로 읽기
        var textassets = Resources.LoadAll<TextAsset>("99_Table/Json");
        string[] separatingStrings = { "\r\n" };

        // Configuration
        var scriptableObj = Resources.Load<Configuration>("Configuration");
        if (scriptableObj != null)
            scriptableObj.JsonDataCount = 0;

        foreach (var asset in textassets)
        {
            string fileName = asset.name;
            int dataCount = 0;

            // \r\n(캐리지리턴, 라인피트)으로 문자열 구분
            string[] lines = asset.text.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

            // 0번째와 마지막번째의 중괄호는 파싱 시작/끝 용도 -> 생략
            // 클래스명: {
            // ...
            // }

            bool parseTrigger = false;
            StringBuilder classBuilder = new StringBuilder();
            string classFormat =
@"public class {0}
{{
    {1}
}}
";
            StringBuilder classNameBuilder = new StringBuilder();
            StringBuilder fieldsBuilder = new StringBuilder();
            for (int i = 1; i < lines.Length-1; i++)
            {
                string word = lines[i];

                // 파싱 시작
                if (word[word.Length - 1] == '{')
                {
                    parseTrigger = !parseTrigger;

                    // 클래스명 얻기
                    int startIdx = word.IndexOf('\"');
                    int endIdx = word.LastIndexOf('\"');
                    for (int j = startIdx + 1; j < endIdx; j++)
                        classNameBuilder.Append(word[j]);

                    continue;
                }
                // 파싱 끝
                else if (word == "  }," || word == "  }")
                {
                    parseTrigger = !parseTrigger;

                    // 클래스 포맷 만들기
                    string copied = string.Copy(classFormat);
                    copied = string.Format(copied, classNameBuilder, fieldsBuilder);

                    // 다음 클래스를 위해 초기화
                    classNameBuilder.Clear();
                    fieldsBuilder.Clear();

                    // classBuilder에 추가
                    classBuilder.AppendLine(copied);
                    continue;
                }
                
                if (parseTrigger)
                {
                    string field = string.Empty;

                    // 필드명 얻기
                    StringBuilder fieldNameBuilder = new StringBuilder();
                    int startIdx = word.IndexOf('\"');
                    int endIdx = FileHelper.FindSpecificChar(word, '\"', 2);
                    for (int j = startIdx + 1; j < endIdx; j++)
                        fieldNameBuilder.Append(word[j]);

                    // 필드 타입 얻기
                    StringBuilder fieldDataBuilder = new StringBuilder();
                    startIdx = FileHelper.FindSpecificChar(word, '\"', 3);
                    endIdx = FileHelper.FindSpecificChar(word, '\"', 4);
                    for (int j = startIdx + 1; j < endIdx; j++)
                        fieldDataBuilder.Append(word[j]);

                    // 파싱시도 순서
                    // int -> float -> bool -> string
                    if (int.TryParse(fieldDataBuilder.ToString(), out _))
                        field = $"public int {fieldNameBuilder};";
                    else
                    {
                        if (float.TryParse(fieldDataBuilder.ToString(), out _))
                            field = $"public float {fieldNameBuilder};";
                        else
                        {
                            if (bool.TryParse(fieldDataBuilder.ToString(), out _))
                                field = $"public bool {fieldNameBuilder};";
                            else
                                field = $"public string {fieldNameBuilder};";
                        }
                    }

                    // StringBuilder에 추가
                    dataCount++;
                    fieldsBuilder.AppendLine(field);
                    fieldsBuilder.Append($"    ");
                }
            }

            // ---------------------------------------------------------------------------------------------------

            string result = classBuilder.ToString();
            if (FileHelper.DirectoryCheck($"{Application.dataPath}/Scripts/99_Json"))
                Debug.LogError($"{Application.dataPath}/Scripts/99_Table 에 해당하는 경로가 없습니다.");
            else
            {
                // 파일 저장
                FileHelper.WriteFile($"{Application.dataPath}/Scripts/99_Json/{fileName}.cs", result);

                // 데이터 총 갯수 저장
                if (scriptableObj != null)
                {
                    scriptableObj.DownloadDataCount += dataCount;
                    EditorUtility.SetDirty(scriptableObj);
                }
                    
            }
        }

        AssetDatabase.Refresh();
    }

    

    [MenuItem("Automation/Prefab/Generate Collider")]
    public static void GenerateCollider()
    {
        string path = Application.dataPath + "/Resources/Result/";

        FileHelper.DirectoryCheck(
            path,
            "디렉토리가 없어서 콜라이더 생성에 생성에 실패했습니다. 디렉토리를 만들었으니 다시 시도해주세요."
        );

        ColliderGenerator _instanceRoot = GameObject.FindObjectOfType<ColliderGenerator>();
        if (_instanceRoot == null)
        {
            throw new Exception("현재 씬에서 ColliderGenerator가 부착된 오브젝트를 찾을 수 없습니다.");
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

        Debug.Log("박스 콜라이더 생성이 완료되었습니다. 콜라이더의 센터는 오브젝트마다 다르니 필요시 조정하세요.");
    }

    [MenuItem("Automation/Prefab/Mesh Combined Objects")]
    public static void MeshCombinedObjects()
    {
        // MeshCombine 컴포넌트가 붙은 게임오브젝트들을 찾아 배열에 저장한다. 
        MeshCombine[] _instanceRoots = GameObject.FindObjectsOfType<MeshCombine>();
        if (_instanceRoots.Length == 0)
        {
            throw new Exception("현재 씬에서 MeshCombine가 부착된 오브젝트를 찾을 수 없습니다.");
        }

        Mesh instanceRootMesh; // Temp
        string instancePath; // Temp

        FileHelper.DirectoryCheck(
            $"{Application.dataPath}/Resources/99_MeshCombineObjects",
            "디렉토리가 없어서 매쉬 컴바인에 생성에 실패했습니다. 디렉토리를 만들었으니 다시 시도해주세요."
        );

        for (int i = 0; i < _instanceRoots.Length; i++)
        {
            string resultPath = $"{Application.dataPath}/Resources/99_MeshCombineObjects";
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", resultPath, _instanceRoots[i].name + "_Mesh", "asset");
            GameObject instanceRoot; // Temp

            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path); // For AssetDatabase.CreateAsset

            // 1. 매쉬 컴바인을 진행한다.
            _instanceRoots[i].MeshCombineObjects();

            // 2. 매쉬 컴바인을 한 게임오브젝트
            instanceRoot = _instanceRoots[i].gameObject;

            // 3. 매쉬 컴바인을 한 게임오브젝트를 프리팹으로 저장할 때의 이름
            instancePath = $"{resultPath}/{_instanceRoots[i].name}.prefab";

            // 4. 매쉬 컴바인을 한 게임오브젝트의 매쉬를 인스턴싱
            instanceRootMesh = UnityEngine.Object.Instantiate(_instanceRoots[i].MeshFilter.sharedMesh);

            // 인스턴싱된 매쉬를 Asset으로써 저장한다. 실제 매쉬가 없으면 프리팹으로 저장을 할 때 매쉬가 없는 채로 저장됨.
            AssetDatabase.CreateAsset(instanceRootMesh, path);
            AssetDatabase.SaveAssets();

            // 6. 인스턴싱 매쉬를 다시 원소의 sharedMesh에 부여한다. (프리팹에 매쉬 자동 부여용도)
            _instanceRoots[i].MeshFilter.sharedMesh = instanceRootMesh;

            // 7. 매쉬 컴바인된 게임오브젝트를 프리팹으로써 저장한다.
            PrefabUtility.SaveAsPrefabAsset(instanceRoot, instancePath, out bool success);

            if (success == true)
                Debug.Log(instanceRoot.name + " 안에 속해있는 오브젝트들의 매쉬 컴바인에 성공하였습니다!");
            else
                Debug.Log(instanceRoot.name + " 안에 속해있는 오브젝트들의 매쉬 컴바인에 실패하였습니다...");

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
            "디렉토리가 없어서 에셋번들 생성에 실패했습니다. 디렉토리를 만들었으니 다시 시도해주세요."
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