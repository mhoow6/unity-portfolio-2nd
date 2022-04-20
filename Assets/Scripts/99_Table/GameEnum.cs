

    public enum AniType
    {
        
        /// <summary> 가만히 있을 때 애니메이션 </summary> ///
        IDLE = 0,

        /// <summary> 달리는 애니메이션 </summary> ///
        RUN = 1,

        /// <summary> 죽을 때 애니메이션 </summary> ///
        DEAD = 2,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK = 3,

        /// <summary> 점프할 때 애니메이션 </summary> ///
        JUMP = 4,

        /// <summary> 가속(회피)할 때 애니메이션 </summary> ///
        DASH = 5,

        /// <summary> 스킬 쓸 때 애니메이션 </summary> ///
        SKILL = 6,

        /// <summary> 궁극기 쓸 때 애니메이션 </summary> ///
        ULTIMATE = 7,

    }

    public enum QuestType
    {
        
        /// <summary> 적 처치 횟수 </summary> ///
        KILL_ENEMY = 0,

        /// <summary> 피격 횟수 n회 이하 </summary> ///
        GET_DAMAGED = 1,

        /// <summary> 전투 불능 횟수 n회 이하 </summary> ///
        INCAPCITATED = 2,

    }

    public enum CharacterType
    {
        
        /// <summary> 생물 </summary> ///
        Biology = 0,

        /// <summary> 기계 </summary> ///
        Machine = 1,

        /// <summary> 이능 </summary> ///
        Supernatural = 2,

    }

    public enum ObjectCode
    {
        
        /// <summary> 궁수 캐릭터 </summary> ///
        CHAR_Sparcher = 0,

        /// <summary> 생물속성 거미 </summary> ///
        CHAR_GreenSpider = 1,

        /// <summary> 이능속성 거미 </summary> ///
        CHAR_PurpleSpider = 2,

        /// <summary> 테스트용 캐릭터 </summary> ///
        CHAR_Dummy = 3,

    }


