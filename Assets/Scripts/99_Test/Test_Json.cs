using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Json : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        JTestClass test = new JTestClass(true);
        string str = ObjectToJson(test);
        Debug.Log(str);

    }

    string ObjectToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    T JsonToObject<T>(string jsonData)
    {
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}
