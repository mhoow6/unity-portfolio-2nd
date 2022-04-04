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

	public struct SparcherAniTypeDialogueTable
	{
		/// <summary> 애니메이션 종류 </summary> ///
		public AniType AniType;
		/// <summary> 클릭 시 대화내용 </summary> ///
		public string Dialog;
	}

	public struct StageQuestTable
	{
		/// <summary> ���� IDX </summary> ///
		public int WorldIdx;
		/// <summary> �������� IDX </summary> ///
		public int StageIdx;
		/// <summary> ����Ʈ ���� </summary> ///
		public QuestType QuestType;
		/// <summary> ��ǥ Ƚ�� </summary> ///
		public int PurposeCount;
	}

}
