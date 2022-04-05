

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

    }

    public enum QuestType
    {
        
        /// <summary> 적 처치 횟수 </summary> ///
        POSITIVE_KILL_ENEMY = 0,

        /// <summary> 피격 횟수 n회 이하 </summary> ///
        NEGATIVE_GET_DAMAGED = 1,

        /// <summary> 전투 불능 횟수 n회 이하 </summary> ///
        NEGATIVE_INCAPCITATED = 2,

    }


