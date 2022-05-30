namespace DatabaseSystem
{
    public class SparcherBasicAttackData : Skillable
    {
        public int AniType;
        public float DamageScale;
        public int ArrowObjectCode;
        public int ArrowLifeTime;
        public int ArrowMoveSpeed;
        public string ArrowPrefabPath;
        public float AutoTargetDetectRange;
        public int DamageType;
        
    }
}

namespace DatabaseSystem
{
    public class SparcherPassiveSkillData : Skillable
    {
        public string SkillName;
        public string SkillDescription;
        public float IncreaseCriticalRatio;
        public string CharacterType;
        
    }
}
