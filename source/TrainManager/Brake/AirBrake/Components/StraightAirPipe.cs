namespace TrainManager.Brake
{
	/// <summary>A straight air pipe</summary>
	public class StraightAirPipe
	{
		/// <summary>The current pressure</summary>
		public double CurrentPressure;
		/// <summary>The rate in Pa/s when making a service brake application</summary>
		public readonly double ServiceRate;
		/// <summary>The rate in Pa/s when making an EB application</summary>
		public readonly double EmergencyRate;
		/// <summary>The release rate in Pa/s</summary>
		public readonly double ReleaseRate;

		public StraightAirPipe(double serviceRate, double emergencyRate, double releaseRate)
		{
			CurrentPressure = 0.0;
			ServiceRate = serviceRate;
			EmergencyRate = emergencyRate;
			ReleaseRate = releaseRate;
		}
		
	}
}
