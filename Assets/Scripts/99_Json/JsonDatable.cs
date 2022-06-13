namespace DatabaseSystem
{
    public abstract class JsonDatable
    {
        public readonly static string[] AutomationExcepts = new string[]
        { 
            "Index",
            "QuestType",
            "PurposeCount",
            "SpCost",
            "IconPath",
            "Name",
            "Description",
        };

        public int Index;
    }

    #region Äù½ºÆ®
    public class Questable : JsonDatable
    {
        public int QuestType;
        public int PurposeCount;
    }
    #endregion

    #region ½ºÅ³
    public class Skillable : JsonDatable
    {
        public int SpCost;
        public string IconPath;
    }

    public class SkillDescriptable : Skillable
    {
        public string Name;
        public string Description;
    }
    #endregion
}

