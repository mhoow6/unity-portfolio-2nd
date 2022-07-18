

    public enum AniType
    {
        
        /// <summary> 가만히 있을 때 애니메이션 </summary> ///
        IDLE_0 = 0,

        /// <summary> 가만히 있을 때 애니메이션 </summary> ///
        IDLE_1 = 1,

        /// <summary> 가만히 있을 때 애니메이션 </summary> ///
        IDLE_2 = 2,

        /// <summary> 가만히 있을 때 애니메이션 </summary> ///
        IDLE_3 = 3,

        /// <summary> 가만히 있을 때 애니메이션 </summary> ///
        IDLE_4 = 4,

        /// <summary> 달리는 애니메이션 </summary> ///
        RUN_0 = 5,

        /// <summary> 달리는 애니메이션 </summary> ///
        RUN_1 = 6,

        /// <summary> 달리는 애니메이션 </summary> ///
        RUN_2 = 7,

        /// <summary> 달리는 애니메이션 </summary> ///
        RUN_3 = 8,

        /// <summary> 달리는 애니메이션 </summary> ///
        RUN_4 = 9,

        /// <summary> 죽을 때 애니메이션 </summary> ///
        DEAD_0 = 10,

        /// <summary> 죽을 때 애니메이션 </summary> ///
        DEAD_1 = 11,

        /// <summary> 죽을 때 애니메이션 </summary> ///
        DEAD_2 = 12,

        /// <summary> 죽을 때 애니메이션 </summary> ///
        DEAD_3 = 13,

        /// <summary> 죽을 때 애니메이션 </summary> ///
        DEAD_4 = 14,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_0 = 15,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_1 = 16,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_2 = 17,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_3 = 18,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_4 = 19,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_5 = 30,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_6 = 31,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_7 = 32,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_8 = 33,

        /// <summary> 공격할 때 애니메이션 </summary> ///
        ATTACK_9 = 34,

        /// <summary> 점프할 때 애니메이션 </summary> ///
        JUMP_0 = 20,

        /// <summary> 점프할 때 애니메이션 </summary> ///
        JUMP_1 = 21,

        /// <summary> 점프할 때 애니메이션 </summary> ///
        JUMP_2 = 22,

        /// <summary> 점프할 때 애니메이션 </summary> ///
        JUMP_3 = 23,

        /// <summary> 점프할 때 애니메이션 </summary> ///
        JUMP_4 = 24,

        /// <summary> 가속(회피)할 때 애니메이션 </summary> ///
        DASH_0 = 25,

        /// <summary> 가속(회피)할 때 애니메이션 </summary> ///
        DASH_1 = 26,

        /// <summary> 가속(회피)할 때 애니메이션 </summary> ///
        DASH_2 = 27,

        /// <summary> 가속(회피)할 때 애니메이션 </summary> ///
        DASH_3 = 28,

        /// <summary> 가속(회피)할 때 애니메이션 </summary> ///
        DASH_4 = 29,

        /// <summary> 앉을 때 애니메이션 </summary> ///
        SIT_0 = 35,

        /// <summary> 일어날 때 애니메이션 </summary> ///
        RISE_0 = 36,

        /// <summary>  </summary> ///
        NONE = -1,

    }

    public enum QuestType
    {
        
        /// <summary> 적 처치 횟수 </summary> ///
        KILL_ENEMY = 0,

        /// <summary> 피격 횟수 n회 이하 </summary> ///
        GET_DAMAGED = 1,

        /// <summary> 전투 불능 횟수 n회 이하 </summary> ///
        INCAPCITATED = 2,

        /// <summary> 골드 소모시 카운트 증가 </summary> ///
        CONSUME_GOLDS = 3,

        /// <summary> 에너지 소모시 카운트 증가 </summary> ///
        CONSUME_ENERGY = 4,

        /// <summary> 특정 캐릭터 흭득시 카운트 증가 </summary> ///
        GET_SPECIFIC_CHARACTER = 5,

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
        
        /// <summary> 생물속성 궁수 </summary> ///
        CHAR_Sparcher = 0,

        /// <summary> 생물속성 거미 </summary> ///
        CHAR_GreenSpider = 1,

        /// <summary> 이능속성 거미 </summary> ///
        CHAR_PurpleSpider = 2,

        /// <summary> 기계속성 더미 캐릭터 </summary> ///
        CHAR_Dummy = 3,

        /// <summary> 발사용 나무 화살 </summary> ///
        PROJ_WoodenArrow = 4,

        /// <summary> Area 벽에 캐릭터가 닿았을 때 나오는 이펙트 </summary> ///
        EFFECT_CharacterHitWall = 5,

        /// <summary> 생물속성 해적(몬스터) </summary> ///
        CHAR_MonsterPirate = 6,

        /// <summary> 생물속성 해적(플레이어블) </summary> ///
        CHAR_Pirate = 7,

        /// <summary> 캐릭터가 교체되었을 때 나오는 이펙트 </summary> ///
        EFFECT_CharacterSwap = 8,

        /// <summary> 대포알 </summary> ///
        PROJ_CanonBullet = 9,

        /// <summary> 대포발사 이펙트 </summary> ///
        Effect_CanonBulletFired = 10,

        /// <summary> Sp를 회복하게 하는 아이템 </summary> ///
        LOOT_SPRecovery = 11,

        /// <summary> Hp를 회복하게 하는 아이템 </summary> ///
        LOOT_HPRecovery = 12,

        /// <summary> 스테이지에서 얻을 수 있는 골드 </summary> ///
        LOOT_Gold = 13,

        /// <summary> 이능속성 보스 버섯 </summary> ///
        CHAR_MonsterMushroom = 16,

        /// <summary> 스테이지에서 얻을 수 있는 전리품 </summary> ///
        LOOT_Item = 17,

        /// <summary> 생물속성 기사 </summary> ///
        CHAR_Knight = 18,

        /// <summary> 더미 코드 </summary> ///
        NONE = -1,

    }

    public enum SceneCode
    {
        
        /// <summary> 로고 </summary> ///
        Logo = 0,

        /// <summary> 로비 </summary> ///
        Lobby = 1,

        /// <summary> 테스트 </summary> ///
        Stage0000 = 2,

        /// <summary> 1-1 </summary> ///
        Stage0101 = 3,

        /// <summary> 에러 </summary> ///
        None = -1,

    }

    public enum ItemType
    {
        
        /// <summary> 더미 </summary> ///
        None = 0,

        /// <summary> 캐릭터 레벨업 아이템 </summary> ///
        CharacterLevelUpChip = 1,

        /// <summary> 무기 레벨업 아이템 </summary> ///
        WeaponLevelUpChip = 2,

        /// <summary> 활 </summary> ///
        Bow = 3,

        /// <summary> 한손검 </summary> ///
        Broadsword = 4,

    }


