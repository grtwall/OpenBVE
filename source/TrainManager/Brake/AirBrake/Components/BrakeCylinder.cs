namespace TrainManager.Brake
{
	/// <summary>A brake cylinder</summary>
	public class BrakeCylinder
	{
		/// <summary>The current pressure</summary>
		public double CurrentPressure;
		/// <summary>The maximum pressure when an EB application is made</summary>
		public readonly double EmergencyMaximumPressure;
		/// <summary>The maximum pressure when full service brakes are applied</summary>
		public readonly double ServiceMaximumPressure;
		/// <summary>The pressure increase in Pa/s when making an EB application</summary>
		public readonly double EmergencyChargeRate;
		/// <summary>The pressure increase in Pa/s when making a service brake application</summary>
		public readonly double ServiceChargeRate;
		/// <summary>The pressure release rate in Pa/s</summary>
		public readonly double ReleaseRate;
		public double SoundPlayedForPressure;

		public BrakeCylinder(double serviceMaximumPressure, double emergencyMaximumPressure, double serviceChargeRate, double emergencyChargeRate, double releaseRate)
		{
			ServiceMaximumPressure = serviceMaximumPressure;
			EmergencyMaximumPressure = emergencyMaximumPressure;
			ServiceChargeRate = serviceChargeRate;
			EmergencyChargeRate = emergencyChargeRate;
			ReleaseRate = releaseRate;
			SoundPlayedForPressure = emergencyMaximumPressure;
			CurrentPressure = EmergencyMaximumPressure;
		}
	}
}
