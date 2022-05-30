namespace DatabaseSystem
{
    public abstract class JsonDatable
    {
        public static string[] AutomationExcepts = new string[]
        { 
            "Index",
            "PurposeCount",
            "Name",
            "Description",
            "QuestType",
            "SpCost",
            "IconPath",
        };

        public int Index;
    }

    public class Questable : JsonDatable
    {
        public int QuestType;
        public int PurposeCount;
    }

    public class Skillable : JsonDatable
    {
        public int SpCost;
        public string IconPath;
        public string Name;
        public string Description;
    }
}

