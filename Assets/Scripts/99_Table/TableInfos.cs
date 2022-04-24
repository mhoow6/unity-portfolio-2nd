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

	public struct CharacterTable
	{
		/// <summary> 오브젝트 코드 </summary> ///
		public ObjectCode Code;
		/// <summary> 캐릭터 속성 </summary> ///
		public CharacterType Type;
		/// <summary> 게임 내에서 사용되는 이름 </summary> ///
		public string Name;
		/// <summary> 레벨 1일때 기본 Hp </summary> ///
		public float BaseHp;
		/// <summary> 레벨업시 Hp 증가율 </summary> ///
		public float HpIncreaseRatioByLevelUp;
		/// <summary> 레벨 1일때 기본 Sp </summary> ///
		public float BaseSp;
		/// <summary> 레벨업시 Sp 증가율 </summary> ///
		public float SpIncreaseRatioByLevelUp;
		/// <summary> 레벨 1일때 기본 공격력 </summary> ///
		public float BaseDamage;
		/// <summary> 레벨업시 공격력 증가율 </summary> ///
		public float DamageIncreaseRatioByLevelUp;
		/// <summary> 레벨 1일때 기본 방어력 </summary> ///
		public float BaseDefense;
		/// <summary> 레벨업시 방어력 증가율 </summary> ///
		public float DefenseIncreaseRatioByLevelUp;
		/// <summary> 레벨 1일때 기본 치명타 확률 </summary> ///
		public float BaseCritical;
		/// <summary> 레벨업시 치명타확률 증가율 </summary> ///
		public float CriticalIncreaseRatioByLevelUp;
		/// <summary> 이동속도 </summary> ///
		public float BaseSpeed;
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

	public struct ProjectileTable
	{
		/// <summary>  </summary> ///
		public ObjectCode Code;
		/// <summary>  </summary> ///
		public int LifeTime;
		/// <summary>  </summary> ///
		public float MoveSpeed;
		/// <summary>  </summary> ///
		public string PrefabPath;
	}

	public struct QuestTable
	{
		/// <summary> 퀘스트 인덱스 </summary> ///
		public int Index;
		/// <summary> 카운트 도달 시 퀘스트 성공여부 </summary> ///
		public bool Positive;
		/// <summary> 퀘스트 종류 </summary> ///
		public QuestType Type;
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
	}

}
