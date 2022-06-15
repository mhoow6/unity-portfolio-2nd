namespace DatabaseSystem
{
    public abstract class JsonDatable
    {
        // JsonDatable을 상속받은 클래스에 새로운 필드가 추가되면, 그 필드명을 여기에 적으세요
        public readonly static string[] AutomationExcepts = new string[]
        { 
            "Index",
            "QuestType",
            "PurposeCount",
            "SpCost",
            "IconPath",
            "Name",
            "Description",
            "Stack",
            "CoolTime"
        };

        public int Index;
    }

    #region 퀘스트
    public class Questable : JsonDatable
    {
        public int QuestType;
        public int PurposeCount;
    }
    #endregion

    #region 스킬
    public class Skillable : JsonDatable
    {
        public int SpCost;
        public string IconPath;
        public int Stack;
        public float CoolTime;
    }

    public class SkillDescriptable : Skillable
    {
        public string Name;
        public string Description;
    }
    #endregion
}

