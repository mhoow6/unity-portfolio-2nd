namespace DatabaseSystem
{
    public partial class TableManager
    {
        public static bool operator !(TableManager tm)
        {
            if (tm.LoadedData == 0)
                return true;
            return false;
        }
    }

    public partial class JsonManager
    {
        public static bool operator !(JsonManager jm)
        {
            if (jm.JsonDatas.Count == 0)
                return true;
            return false;
        }
    }
}

