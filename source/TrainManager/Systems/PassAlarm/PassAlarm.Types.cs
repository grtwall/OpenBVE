namespace TrainManager.Systems
{
	/// <summary>Defines the differing types of station pass alarm a train may be fitted with</summary>
	public enum PassAlarmType
	{
		/// <summary>No pass alarm</summary>
		None = 0,
		/// <summary>The alarm sounds once</summary>
		Single = 1,
		/// <summary>The alarm loops until cancelled</summary>
		Loop = 2
	}
}
