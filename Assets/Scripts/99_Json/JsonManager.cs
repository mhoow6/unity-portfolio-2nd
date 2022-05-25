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
            var achievements = Resources.Load<TextAsset>("99_Database/Json/Achievements");
            JSONNode achievementsRoot = JSONNode.Parse(achievements.text);
            JSONNode achievementconsumegolds10000Node = achievementsRoot["AchievementConsumeGolds10000(Questable)"];
            AchievementConsumeGolds10000 achievementconsumegolds10000 = JsonConvert.DeserializeObject<AchievementConsumeGolds10000>(achievementconsumegolds10000Node.ToString());
            JsonDatas.Add(achievementconsumegolds10000.Index, achievementconsumegolds10000);

            JSONNode achievementconsumeenergy100Node = achievementsRoot["AchievementConsumeEnergy100(Questable)"];
            AchievementConsumeEnergy100 achievementconsumeenergy100 = JsonConvert.DeserializeObject<AchievementConsumeEnergy100>(achievementconsumeenergy100Node.ToString());
            JsonDatas.Add(achievementconsumeenergy100.Index, achievementconsumeenergy100);

            JSONNode achievementgetpirateNode = achievementsRoot["AchievementGetPirate(Questable)"];
            AchievementGetPirate achievementgetpirate = JsonConvert.DeserializeObject<AchievementGetPirate>(achievementgetpirateNode.ToString());
            JsonDatas.Add(achievementgetpirate.Index, achievementgetpirate);

            var skilldatas = Resources.Load<TextAsset>("99_Database/Json/SkillDatas");
            JSONNode skilldatasRoot = JSONNode.Parse(skilldatas.text);
            JSONNode sparcherbasicattackdataNode = skilldatasRoot["SparcherBasicAttackData(JsonDatable)"];
            SparcherBasicAttackData sparcherbasicattackdata = JsonConvert.DeserializeObject<SparcherBasicAttackData>(sparcherbasicattackdataNode.ToString());
            JsonDatas.Add(sparcherbasicattackdata.Index, sparcherbasicattackdata);
        }
    }
}
