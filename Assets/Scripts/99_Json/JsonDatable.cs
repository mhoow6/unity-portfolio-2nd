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
            "CoolTime",
            "DamageScale",
            "ActorCode",
            "BehaviorDecisionTime",
            "GroggyPoint",
            "WeaponCritical",
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
        public float DamageScale;
        public int GroggyPoint;

        #region implicit/explicit operator ������ �����ε� ����
        // implicit operator bool(Skillable skill)��?
        // Skillable s = new Skillable();
        // bool b = s;
        //
        // explicit operator bool(Skillable skill)��?
        // Skillable s = new Skillable();
        // bool b = (Skillable)s;
        //
        // ���� ���ÿ� �� ���� ���ٴ� ���� �������.
        #endregion
        public static implicit operator bool(Skillable skill)
        {
            if (skill != null)
                return true;
            return false;
        }
    }

    public class SkillDescriptable : Skillable
    {
        public string Name;
        public string Description;
    }
    #endregion

    #region AI
    public class Behaviorable : JsonDatable
    {
        public int ActorCode;
        public float BehaviorDecisionTime;
    }
    #endregion

    #region ����
    public class Weaponable : JsonDatable
    {
        public float WeaponCritical;
    }
    #endregion
}

