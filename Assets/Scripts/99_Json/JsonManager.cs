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

            JSONNode monstermushroombehaviordataNode = behaviordatasRoot["MonsterMushroomBehaviorData(Behaviorable)"];
            MonsterMushroomBehaviorData monstermushroombehaviordata = JsonConvert.DeserializeObject<MonsterMushroomBehaviorData>(monstermushroombehaviordataNode.ToString());
            JsonDatas.Add(monstermushroombehaviordata.Index, monstermushroombehaviordata);

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

            JSONNode monstermushroomattack01dataNode = skilldatasRoot["MonsterMushroomAttack01Data(Skillable)"];
            MonsterMushroomAttack01Data monstermushroomattack01data = JsonConvert.DeserializeObject<MonsterMushroomAttack01Data>(monstermushroomattack01dataNode.ToString());
            JsonDatas.Add(monstermushroomattack01data.Index, monstermushroomattack01data);

            JSONNode monstermushroomattack02dataNode = skilldatasRoot["MonsterMushroomAttack02Data(Skillable)"];
            MonsterMushroomAttack02Data monstermushroomattack02data = JsonConvert.DeserializeObject<MonsterMushroomAttack02Data>(monstermushroomattack02dataNode.ToString());
            JsonDatas.Add(monstermushroomattack02data.Index, monstermushroomattack02data);

            JSONNode monstermushroomattack03dataNode = skilldatasRoot["MonsterMushroomAttack03Data(Skillable)"];
            MonsterMushroomAttack03Data monstermushroomattack03data = JsonConvert.DeserializeObject<MonsterMushroomAttack03Data>(monstermushroomattack03dataNode.ToString());
            JsonDatas.Add(monstermushroomattack03data.Index, monstermushroomattack03data);

            var weapondatas = Resources.Load<TextAsset>("99_Database/Json/WeaponDatas");
            JSONNode weapondatasRoot = JSONNode.Parse(weapondatas.text);
            JSONNode woodenbowdataNode = weapondatasRoot["WoodenBowData(Weaponable)"];
            WoodenBowData woodenbowdata = JsonConvert.DeserializeObject<WoodenBowData>(woodenbowdataNode.ToString());
            JsonDatas.Add(woodenbowdata.Index, woodenbowdata);
        }
    }
}
