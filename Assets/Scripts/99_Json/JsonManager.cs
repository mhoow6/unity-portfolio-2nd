using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Newtonsoft.Json;

namespace DatabaseSystem
{
    public class JsonManager
    {
        public static JsonManager Instance { get; private set; } = new JsonManager();
        public Dictionary<int, JsonDatable> JsonDatas { get; private set; } = new Dictionary<int, JsonDatable>();

        public void LoadJson()
        {
            var skilldatas = Resources.Load<TextAsset>("99_Database/Json/SkillDatas");
            JSONNode skilldatasRoot = JSONNode.Parse(skilldatas.text);
            JSONNode sparcherbasicattackdataNode = skilldatasRoot["SparcherBasicAttackData"];
            SparcherBasicAttackData sparcherbasicattackdata = JsonConvert.DeserializeObject<SparcherBasicAttackData>(sparcherbasicattackdataNode.ToString());
            JsonDatas.Add(sparcherbasicattackdata.Index, sparcherbasicattackdata);
        }
    }
}
