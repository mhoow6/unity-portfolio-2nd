namespace DatabaseSystem
{
    public abstract class JsonDatable
    {
        // JsonDatable�� ��ӹ��� Ŭ������ ���ο� �ʵ尡 �߰��Ǹ�, �� �ʵ���� ���⿡ ��������
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

    #region ����Ʈ
    public class Questable : JsonDatable
    {
        public int QuestType;
        public int PurposeCount;
    }
    #endregion

    #region ��ų
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

