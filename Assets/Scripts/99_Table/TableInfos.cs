namespace TableSystem
{
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

	public struct SparcherAniTypeDialogueTable
	{
		/// <summary> 애니메이션 종류 </summary> ///
		public AniType AniType;
		/// <summary> 클릭 시 대화내용 </summary> ///
		public string Dialog;
	}

	public struct StageQuestTable
	{
		/// <summary> 월드 IDX </summary> ///
		public int WorldIdx;
		/// <summary> 스테이지 IDX </summary> ///
		public int StageIdx;
		/// <summary> 퀘스트1 IDX </summary> ///
		public int Quest1Idx;
		/// <summary> 퀘스트2 IDX </summary> ///
		public int Quest2Idx;
		/// <summary> 퀘스트3 IDX </summary> ///
		public int Quest3Idx;
	}

}
