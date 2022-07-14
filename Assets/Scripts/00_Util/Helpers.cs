using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public static class FileHelper
{
    #region 파일 읽기/쓰기/삭제
    public static void WriteFile(string filePath, string content, Action onExceptionCB = null)
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

    public static void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }

    public static void WriteSceneData(GameObject parent, StreamWriter streamWriter, bool hasChild = true)
    {
        if (hasChild)
        {
            streamWriter.WriteLine(parent.name);
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                streamWriter.WriteLine(
                    FileHelper.EraseBracketInName(parent.transform.GetChild(i).name) + "," +
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
        else
        {
            streamWriter.WriteLine(
                    FileHelper.EraseBracketInName(parent.name) + "," +
                    parent.transform.position.x + "," +
                    parent.transform.position.y + "," +
                    parent.transform.position.z + "," +
                    parent.transform.rotation.eulerAngles.x + "," +
                    parent.transform.rotation.eulerAngles.y + "," +
                    parent.transform.rotation.eulerAngles.z + "," +
                    parent.transform.localScale.x + "," +
                    parent.transform.localScale.y + "," +
                    parent.transform.localScale.z
                    );
        }
    }

    public static bool DirectoryCheck(string directory, string notFoundMessage = null)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            Debug.LogError($"{notFoundMessage}");
            return false;
        }
        return true;
    }
    #endregion

    #region 문자열 처리
    public static List<string> GetStringsFromByCSVFormat(string filePath)
    {
        TextAsset txtAsset = Resources.Load<TextAsset>(filePath);
        if (txtAsset == null)
            return null;

        char[] option = { '\r', '\n' };
        string[] _lines = txtAsset.text.Split(option);
        List<string> lines = new List<string>();

        foreach (string line in _lines)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            lines.Add(line);
        }

        return lines;
    }

    public static List<string> GetStringsFrom(string filePath)
    {
        string line = string.Empty;
        List<string> lines = new List<string>();

        using (FileStream f = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(f, System.Text.Encoding.UTF8))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
        }

        return lines;
    }

    public static string GetStringFrom(string filePath)
    {
        string line = string.Empty;
        string lines = string.Empty;

        try
        {
            using (FileStream f = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(f, System.Text.Encoding.UTF8))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines += line;
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning($"{filePath}을 찾을 수 없습니다.");
        }

        return lines;
    }

    public static string EraseBracketInName(this string name)
    {
        string mobNameWithNoSpace = name.Replace(" ", "");
        int index = mobNameWithNoSpace.IndexOf('(');

        if (index == -1)
            return name;

        return mobNameWithNoSpace.Remove(index);
    }

    /// <summary>
    /// oridinal번째에 있는 특정 문자를 word에서 찾아, 해당 인덱스를 돌려줍니다.
    /// </summary>
    public static int FindSpecificChar(this string word, char search, int oridinal)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < word.Length; i++)
        {
            char x = word[i];
            if (word[i] == search)
                indices.Add(i);
        }

        if (indices.Count == 0)
        {
            Debug.LogError($"{word}에서 {search}에 해당되는 문자를 찾을 수 없습니다.");
            return -1;
        }

        return indices[oridinal - 1];
    }

    public static string BeautifyJson(this string jsonString)
    {
        string beautifiedJson = JValue.Parse(jsonString).ToString(Formatting.Indented);
        return beautifiedJson;
    }
    #endregion
}

public static class UnityHelper
{
    public static Bounds GetBoundsFromMeshFilter(MeshFilter mf)
    {
        Vector3[] meshVertices = mf.mesh.vertices;
        Bounds result = new Bounds();

        // C# 7.0
        var container =
            (min_x: meshVertices[0].x,
             max_x: meshVertices[0].x,
             min_y: meshVertices[0].y,
             max_y: meshVertices[0].y,
             min_z: meshVertices[0].z,
             max_z: meshVertices[0].z);

        foreach (Vector3 vertex in meshVertices)
        {
            if (vertex.x < container.min_x)
                container.min_x = vertex.x;

            if (vertex.x > container.max_x)
                container.max_x = vertex.x;

            if (vertex.y < container.min_y)
                container.min_y = vertex.y;

            if (vertex.y > container.max_y)
                container.max_y = vertex.y;

            if (vertex.z < container.min_z)
                container.min_z = vertex.z;

            if (vertex.z > container.max_z)
                container.max_z = vertex.z;
        }

        float Length_x = container.max_x - container.min_x;
        float Length_y = container.max_y - container.min_y;
        float Length_z = container.max_z - container.min_z;

        result.center = mf.gameObject.transform.position;
        result.size = new Vector3(Length_x * mf.transform.localScale.x, Length_y * mf.transform.localScale.y, Length_z * mf.transform.localScale.z);

        return result;
    }

    /// <summary> 해당 이름의 노드를 탐색합니다. </summary>
    public static void GetNodeObject(Transform root, string nodeName, ref Transform node)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            // 이미 nodeName에 맞는 것을 찾아서 null이 아닐 경우 의미없는 호출을 방지하기 위해 함수 종료
            if (node != null)
                return;

            Transform child = root.GetChild(i);

            if (child.name != nodeName)
            {
                // 자식이 또다른 자식을 가질 경우 자식의 자식들을 탐색
                if (child.childCount != 0)
                    GetNodeObject(child, nodeName, ref node);
            }

            if (child.name == nodeName)
                node = child;
        }
    }

    /// <summary> 구 충돌을 시도합니다.</summary>
    public static bool TrySphereCollide(this GameObject lhs, GameObject rhs, float lhsRadius)
    {
        // 화살과 타겟의 충돌처리
        Vector3 offset = lhs.transform.position - rhs.transform.position;
        float sqrLength = offset.sqrMagnitude;

        // 충돌했다면?
        if (sqrLength < lhsRadius * lhsRadius)
            return true;
        return false;
    }

    /// <summary> AABB 충돌을 시도합니다. </summary>
    public static bool TryAABB(this ref Bounds lhsBounds, ref Bounds rhsBounds)
    {
        if (lhsBounds.Intersects(rhsBounds))
            return true;

        return false;
    }

    /// <summary>
    /// Returns true if the object is either a null reference or has been destroyed by unity.
    /// This will respect ISPDisposable over all else.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNullOrDestroyed(this System.Object obj)
    {
        if (object.ReferenceEquals(obj, null))
            return true;

        if (obj is UnityEngine.Object)
            return (obj as UnityEngine.Object) == null;

        return false;
    }

    public static bool RaycastDown(this Vector3 position, out Vector3 hitPoint)
    {
        // 포탄이 닿을 오브젝트 감지
        Ray ray = new Ray(position, Vector3.down);
        RaycastHit hitInfo;
        if (UnityEngine.Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            hitPoint = hitInfo.point;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }

    public static bool Similar(this Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) < 0.01f;
    }

    public static void DODisable(this GameObject gameObject, float lifeDuration)
    {
        var _helper = new GameObject("CoroutineHelper", typeof(CoroutineHelper));
        var helper = _helper.GetComponent<CoroutineHelper>();
        helper.HelpDODisable(gameObject, lifeDuration);
    }
}