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
		/// <summary> 퀘스트IDX </summary> ///
		public int Index;
		/// <summary> 퀘스트 타입 </summary> ///
		public QuestType Type;
		/// <summary> 목표 횟수 </summary> ///
		public int PurposeCount;
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
