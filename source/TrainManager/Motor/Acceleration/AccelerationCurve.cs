namespace TrainManager.Motor
{
	/// <summary>An abstract acceleration curve</summary>
	public abstract class AccelerationCurve
	{
		/// <summary>Gets the acceleration output for this curve</summary>
		/// <param name="Speed">The current speed</param>
		/// <param name="Loading">A double between 0 (Unloaded) and 1.0 (Loaded) representing the load factor</param>
		/// <returns>The acceleration output</returns>
		public abstract double GetAccelerationOutput(double Speed, double Loading);
	}
}
