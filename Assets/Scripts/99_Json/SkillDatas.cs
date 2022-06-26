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


    public class SparcherPassiveSkillData : SkillDescriptable
    {
        public float IncreaseCriticalRatio;
        public string CharacterType;
        
    }


    public class SparcherDashData : Skillable
    {
        public float DashDistance;
        public float ArriveTime;
        
    }


}
