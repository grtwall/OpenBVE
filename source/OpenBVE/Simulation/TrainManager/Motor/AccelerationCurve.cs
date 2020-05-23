using System;
using TrainManager.Motor;

namespace OpenBve
{
	/// <summary>The TrainManager is the root class containing functions to load and manage trains within the simulation world.</summary>
	public static partial class TrainManager
	{
		/// <summary>Represents a BVE2 / BVE4 format Acceleration Curve</summary>
		internal class BveAccelerationCurve : AccelerationCurve
		{
			internal double StageZeroAcceleration;
			internal double StageOneSpeed;
			internal double StageOneAcceleration;
			internal double StageTwoSpeed;
			internal double StageTwoExponent;
			internal double Multiplier;

			public override double GetAccelerationOutput(double Speed, double Loading)
			{
				if (Speed <= 0.0)
				{
					return Multiplier * this.StageZeroAcceleration;
				}
				if (Speed < this.StageOneSpeed)
				{
					double t = Speed / this.StageOneSpeed;
					return Multiplier * (this.StageZeroAcceleration * (1.0 - t) + this.StageOneAcceleration * t);
				}
				if (Speed < this.StageTwoSpeed)
				{
					return Multiplier * this.StageOneSpeed * this.StageOneAcceleration / Speed;
				}
				return Multiplier * this.StageOneSpeed * this.StageOneAcceleration * Math.Pow(this.StageTwoSpeed, this.StageTwoExponent - 1.0) * Math.Pow(Speed, -this.StageTwoExponent);
			}

			internal BveAccelerationCurve Clone(double multiplier)
			{
				return new BveAccelerationCurve
				{
					StageZeroAcceleration = this.StageZeroAcceleration,
					StageOneSpeed = this.StageOneSpeed,
					StageOneAcceleration = this.StageOneAcceleration,
					StageTwoSpeed = this.StageTwoSpeed,
					StageTwoExponent = this.StageTwoExponent,
					Multiplier = multiplier
				};
			}
		}

		internal class BveDecelerationCurve : AccelerationCurve
		{
			private readonly double MaxDecelerationOutput;
			public override double GetAccelerationOutput(double Speed, double Loading)
			{
				return this.MaxDecelerationOutput;
			}

			internal BveDecelerationCurve(double Deceleration)
			{
				this.MaxDecelerationOutput = Deceleration;
			}
		}
	}
}
