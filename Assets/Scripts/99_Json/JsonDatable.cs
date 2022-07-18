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
            "CoolTime",
            "DamageScale",
            "ActorCode",
            "BehaviorDecisionTime",
            "GroggyPoint",
            "WeaponCritical",
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
        public float DamageScale;
        public int GroggyPoint;

        #region implicit/explicit operator 연산자 오버로딩 설명
        // implicit operator bool(Skillable skill)란?
        // Skillable s = new Skillable();
        // bool b = s;
        //
        // explicit operator bool(Skillable skill)란?
        // Skillable s = new Skillable();
        // bool b = (Skillable)s;
        //
        // 둘이 동시에 쓸 수는 없다는 점을 기억하자.
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

    #region 무기
    public class Weaponable : JsonDatable
    {
        public float WeaponCritical;
    }
    #endregion
}

