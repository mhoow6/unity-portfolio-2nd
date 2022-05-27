namespace DatabaseSystem
{
    public class SparcherBasicAttackData : JsonDatable
    {
        public int AniType;
        public string Name;
        public string IconPath;
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
    public class SparcherPassiveSkillData : PassiveSkillable
    {
        public string Name;
        public string Description;
        public float IncreaseCriticalRatio;
        public string CharacterType;
        
    }
}
