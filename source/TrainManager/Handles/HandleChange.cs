namespace TrainManager.Handles
{
	/// <summary>A delayed change to be performed to a handle position</summary>
	public struct HandleChange
	{
		/// <summary>The relative offset</summary>
		public int Value;
		/// <summary>The delay time</summary>
		public double Time;
	}
}
