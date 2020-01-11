namespace OpenBveApi.Trains
{
	/// <summary>Represents the base abstract cargo type to be carried by a car</summary>
	public abstract class AbstractCargo
	{
		/// <summary>The current cargo ratio</summary>
		/// <remarks>Must be a number between 0 and 250, where 100 represents a nominal loading</remarks>
		public double Ratio;
		/// <summary>Whether the cargo is being damaged by excessive acceleration etc.</summary>
		/// <remarks>Used for scoring purposes</remarks>
		public bool Damaged;

		/// <summary>Returns the total mass of the cargo</summary>
		public virtual double Mass
		{
			get
			{
				return 0.0;
			}
		}

		/// <summary>Called once a frame to update the cargo</summary>
		/// <param name="Acceleration">The current acceleration of the train</param>
		/// <param name="TimeElapsed">The elapsed time</param>
		public virtual void Update(double Acceleration, double TimeElapsed)
		{

		}

		/// <summary>Updates the mass of the cargo from a new loading ratio</summary>
		public virtual void UpdateLoading(double ratio)
		{

		}
	}
}
