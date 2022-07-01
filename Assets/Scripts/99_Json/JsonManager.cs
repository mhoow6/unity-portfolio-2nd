using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using Newtonsoft.Json;

namespace DatabaseSystem
{
    public partial class JsonManager
    {
        public static JsonManager Instance { get; private set; } = new JsonManager();
        public Dictionary<int, JsonDatable> JsonDatas { get; private set; } = new Dictionary<int, JsonDatable>();

        public void LoadJson()
        {
            var behaviordatas = Resources.Load<TextAsset>("99_Database/Json/BehaviorDatas");
            JSONNode behaviordatasRoot = JSONNode.Parse(behaviordatas.text);
            JSONNode monsterpiratebehaviordataNode = behaviordatasRoot["MonsterPirateBehaviorData(Behaviorable)"];
            MonsterPirateBehaviorData monsterpiratebehaviordata = JsonConvert.DeserializeObject<MonsterPirateBehaviorData>(monsterpiratebehaviordataNode.ToString());
            JsonDatas.Add(monsterpiratebehaviordata.Index, monsterpiratebehaviordata);

            var skilldatas = Resources.Load<TextAsset>("99_Database/Json/SkillDatas");
            JSONNode skilldatasRoot = JSONNode.Parse(skilldatas.text);
            JSONNode sparcherbasicattackdataNode = skilldatasRoot["SparcherBasicAttackData(Skillable)"];
            SparcherBasicAttackData sparcherbasicattackdata = JsonConvert.DeserializeObject<SparcherBasicAttackData>(sparcherbasicattackdataNode.ToString());
            JsonDatas.Add(sparcherbasicattackdata.Index, sparcherbasicattackdata);

            JSONNode sparcherpassiveskilldataNode = skilldatasRoot["SparcherPassiveSkillData(SkillDescriptable)"];
            SparcherPassiveSkillData sparcherpassiveskilldata = JsonConvert.DeserializeObject<SparcherPassiveSkillData>(sparcherpassiveskilldataNode.ToString());
            JsonDatas.Add(sparcherpassiveskilldata.Index, sparcherpassiveskilldata);

            JSONNode sparcherdashdataNode = skilldatasRoot["SparcherDashData(Skillable)"];
            SparcherDashData sparcherdashdata = JsonConvert.DeserializeObject<SparcherDashData>(sparcherdashdataNode.ToString());
            JsonDatas.Add(sparcherdashdata.Index, sparcherdashdata);

            JSONNode sparcherultidataNode = skilldatasRoot["SparcherUltiData(Skillable)"];
            SparcherUltiData sparcherultidata = JsonConvert.DeserializeObject<SparcherUltiData>(sparcherultidataNode.ToString());
            JsonDatas.Add(sparcherultidata.Index, sparcherultidata);

            JSONNode monsterpirateattackdataNode = skilldatasRoot["MonsterPirateAttackData(Skillable)"];
            MonsterPirateAttackData monsterpirateattackdata = JsonConvert.DeserializeObject<MonsterPirateAttackData>(monsterpirateattackdataNode.ToString());
            JsonDatas.Add(monsterpirateattackdata.Index, monsterpirateattackdata);
        }
    }
}
