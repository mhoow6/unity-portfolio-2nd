namespace DatabaseSystem
{
    public abstract class JsonDatable
    {
        public static string[] JsonDataExceptFields = new string[] { "Index", "PurposeCount" };
        public int Index;
    }

    public class Questable : JsonDatable
    {
        public int PurposeCount;
    }
}

