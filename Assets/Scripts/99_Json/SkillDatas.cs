﻿namespace DatabaseSystem
{
    
    public class SparcherBasicAttackData : Skillable
    {
        public int AniType;
        public int ArrowObjectCode;
        public int ArrowLifeTime;
        public int ArrowMoveSpeed;
        public string ArrowPrefabPath;
        public float AutoTargetDetectRange;
        
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


    public class SparcherUltiData : Skillable
    {
        public float HitBoxRange;
        public int MaximumHits;
        
    }


    public class MonsterPirateAttackData : Skillable
    {
        public int BulletObjectCode;
        public int BulletSpeed;
        public int BulletLifeTime;
        public float BulletShootHeight;
        
    }


    public class MonsterMushroomAttack01Data : Skillable
    {
        
    }


    public class MonsterMushroomAttack02Data : Skillable
    {
        public float AttackDistance;
        public float HitboxLifeTime;
        public int MaximumHits;
        
    }


    public class MonsterMushroomAttack03Data : Skillable
    {
        public float AttackDistance;
        public float AttackAngle;
        
    }


    public class KnightPassiveSkillData : SkillDescriptable
    {
        public float IncreaseDefenseRatio;
        
    }


    public class KnightAInputData : Skillable
    {
        public int HitAngle;
        public float HitRange;
        public int HitGainSp;
        public int MaximumHits;
        
    }


    public class KnightXInputData : Skillable
    {
        public int BuffDuration;
        public float BuffSetSpeed;
        
    }


    public class KnightBInputData : Skillable
    {
        public int BuffDuration;
        public int BuffIncreaseDef;
        
    }


}
