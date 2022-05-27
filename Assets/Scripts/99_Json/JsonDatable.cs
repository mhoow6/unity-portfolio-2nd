namespace DatabaseSystem
{
    public abstract class JsonDatable
    {
        public static string[] AutomationExcepts = new string[] { "Index", "PurposeCount", "SkillName", "SkillDescription", "QuestType" };
        public int Index;
    }

    public class Questable : JsonDatable
    {
        public int QuestType;
        public int PurposeCount;
    }

    public class PassiveSkillable : JsonDatable
    {
        public string SkillName;
        public string SkillDescription;
    }
}

