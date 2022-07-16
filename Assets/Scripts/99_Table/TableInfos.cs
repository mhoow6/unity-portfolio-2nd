namespace DatabaseSystem
{
	public struct AniTypeDialogueTable
	{
		/// <summary> 오브젝트 코드 </summary> ///
		public ObjectCode ObjectCode;
		/// <summary> 애니메이션 종류 </summary> ///
		public int AniType;
		/// <summary> 클릭 시 대화내용 </summary> ///
		public string Dialog;
	}

	public struct CharacterLevelExperienceTable
	{
		/// <summary> 캐릭터 레벨 </summary> ///
		public int Level;
		/// <summary> 캐릭터 최대 경험치 </summary> ///
		public int MaxExperience;
	}

	public struct CharacterTable
	{
		/// <summary> 오브젝트 코드 </summary> ///
		public ObjectCode Code;
		/// <summary> 캐릭터 속성 </summary> ///
		public CharacterType Type;
		/// <summary> 게임 내에서 사용되는 이름 </summary> ///
		public string Name;
		/// <summary> 레벨 1일때 기본 Hp </summary> ///
		public int BaseHp;
		/// <summary> 레벨업시 Hp 증가율 </summary> ///
		public float HpIncreaseRatioByLevelUp;
		/// <summary> 레벨 1일때 기본 Sp </summary> ///
		public int BaseSp;
		/// <summary> 레벨업시 Sp 증가율 </summary> ///
		public float SpIncreaseRatioByLevelUp;
		/// <summary> 레벨 1일때 기본 공격력 </summary> ///
		public int BaseDamage;
		/// <summary> 레벨업시 공격력 증가율 </summary> ///
		public float DamageIncreaseRatioByLevelUp;
		/// <summary> 레벨 1일때 기본 방어력 </summary> ///
		public int BaseDefense;
		/// <summary> 레벨업시 방어력 증가율 </summary> ///
		public float DefenseIncreaseRatioByLevelUp;
		/// <summary> 레벨 1일때 기본 치명타 확률 </summary> ///
		public int BaseCritical;
		/// <summary> 레벨업시 치명타확률 증가율 </summary> ///
		public float CriticalIncreaseRatioByLevelUp;
		/// <summary> 이동속도 </summary> ///
		public float BaseSpeed;
		/// <summary> 캐릭터 초상화 이름 </summary> ///
		public string PortraitName;
		/// <summary> 로비에서 쓰일 애니메이션 컨트롤러 경로 </summary> ///
		public string LobbyAnimatorPath;
		/// <summary> 그로기 피로도 (0~100) </summary> ///
		public int MaxGroggyExhaustion;
		/// <summary> 그로기 걸렸을때 회복속도(ms) </summary> ///
		public int GroggyRecoverySpeed;
		/// <summary> 그로기 자연 회복속도(ms) </summary> ///
		public int GroggyNaturalRecoverySpeed;
	}

	public struct HpRecoveryItemTable
	{
		/// <summary> 아이템 IDX </summary> ///
		public int ItemIdx;
		/// <summary> 아이템을 쓸 수 있는 최소 캐릭터 레벨 </summary> ///
		public int MinUseLevel;
		/// <summary> 아이템이 쓸 수 있는 최대 캐릭터 레벨 </summary> ///
		public int MaxUseLevel;
		/// <summary> HP 회복량 </summary> ///
		public int HpRecoveryPoint;
	}

	public struct ItemTable
	{
		/// <summary> 아이템 인덱스 </summary> ///
		public int Index;
		/// <summary> 아이템 타입 </summary> ///
		public ItemType Type;
		/// <summary> 게임에서 사용되는 이름 </summary> ///
		public string Name;
		/// <summary> 아이템 별의 갯수 </summary> ///
		public int StarCount;
		/// <summary> 아이콘 이름 </summary> ///
		public string IconName;
		/// <summary> 적용 포인트 </summary> ///
		public int Point;
	}

	public struct PlayerLevelEnergyTable
	{
		/// <summary> 레벨 </summary> ///
		public int Level;
		/// <summary> 최대에너지 </summary> ///
		public int MaxEnergy;
	}

	public struct PlayerLevelExperienceTable
	{
		/// <summary> 레벨 </summary> ///
		public int Level;
		/// <summary> 최대 경험치 </summary> ///
		public int MaxExperience;
	}

	public struct QuestDescriptionTable
	{
		/// <summary> 퀘스트 타입 </summary> ///
		public QuestType Type;
		/// <summary> 퀘스트 설명 </summary> ///
		public string Description;
	}

	public struct QuestTable
	{
		/// <summary> 퀘스트 인덱스 </summary> ///
		public int Index;
		/// <summary> 카운트 도달 시 퀘스트 성공여부 </summary> ///
		public bool Positive;
		/// <summary> 퀘스트 종류 </summary> ///
		public QuestType Type;
		/// <summary> 퀘스트 타겟 코드 </summary> ///
		public int Target;
		/// <summary> 목표 카운트 </summary> ///
		public int PurposeCount;
	}

	public struct RandomNicknameTable
	{
		/// <summary> 닉네임 공백 시 랜덤 닉네임 </summary> ///
		public string RandomNickname;
	}

	public struct SlangTable
	{
		/// <summary> 비속어 단어 </summary> ///
		public string SlangWord;
	}

	public struct SpRecoveryItemTable
	{
		/// <summary> 아이템 IDX </summary> ///
		public int ItemIdx;
		/// <summary> 아이템을 쓸 수 있는 최소 캐릭터 레벨 </summary> ///
		public int MinUseLevel;
		/// <summary> 아이템이 쓸 수 있는 최대 캐릭터 레벨 </summary> ///
		public int MaxUseLevel;
		/// <summary> SP 회복량 </summary> ///
		public int SpRecoveryPoint;
	}

	public struct StageDialogueTable
	{
		/// <summary>  </summary> ///
		public int Index;
		/// <summary>  </summary> ///
		public int WorldIdx;
		/// <summary>  </summary> ///
		public int StageIdx;
		/// <summary>  </summary> ///
		public int AreaIdx;
		/// <summary>  </summary> ///
		public bool IsLeft;
		/// <summary>  </summary> ///
		public bool NpcTalk;
		/// <summary>  </summary> ///
		public string NpcName;
		/// <summary>  </summary> ///
		public string NpcImage;
		/// <summary>  </summary> ///
		public string Dialogue;
	}

	public struct StageDropItemTable
	{
		/// <summary> 월드 IDX </summary> ///
		public int WorldIdx;
		/// <summary> 스테이지 IDX </summary> ///
		public int StageIdx;
		/// <summary> 드랍하는 골드의 최소값 </summary> ///
		public int MinGoldDropValue;
		/// <summary> 드랍하는 골드의 최대값 </summary> ///
		public int MaxGoldDropValue;
		/// <summary> 드랍하는 아이템1 인덱스 </summary> ///
		public int DropItem1Index;
		/// <summary> 드랍하는 아이템2 인덱스 </summary> ///
		public int DropItem2Index;
		/// <summary> 드랍하는 아이템3 인덱스 </summary> ///
		public int DropItem3Index;
		/// <summary> 드랍하는 아이템4 인덱스 </summary> ///
		public int DropItem4Index;
		/// <summary> 드랍하는 아이템5 인덱스 </summary> ///
		public int DropItem5Index;
	}

	public struct StageTable
	{
		/// <summary> 월드 IDX </summary> ///
		public int WorldIdx;
		/// <summary> 스테이지 IDX </summary> ///
		public int StageIdx;
		/// <summary> 스테이지 이름 </summary> ///
		public string StageName;
		/// <summary> 스테이지 설명 </summary> ///
		public string StageDescription;
		/// <summary> 첫번째 목표 IDX </summary> ///
		public int Quest1Idx;
		/// <summary> 두번째 목표 IDX </summary> ///
		public int Quest2Idx;
		/// <summary> 세번째 목표 IDX </summary> ///
		public int Quest3Idx;
		/// <summary> 스테이지 진입 시 드는 에너지 비용 </summary> ///
		public int EnergyCost;
		/// <summary> 컨텐츠 잠금여부 </summary> ///
		public bool LockContent;
		/// <summary> 스테이지 썸네일 이름 </summary> ///
		public string StageImage;
		/// <summary> GameEnum.csv 참고 </summary> ///
		public SceneCode SceneCode;
		/// <summary> 스테이지 프리팹 이름 </summary> ///
		public string WorldPrefabName;
		/// <summary> 스테이지를 클리어 할 수 있는 최대시간(sec) </summary> ///
		public int ClearTimelimit;
	}

}
