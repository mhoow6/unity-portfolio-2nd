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

}
