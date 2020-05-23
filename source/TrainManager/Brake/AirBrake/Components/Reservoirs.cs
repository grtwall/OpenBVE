﻿using System;

namespace TrainManager.Brake
{
	/// <summary>An auxiliary reservoir</summary>
	public class AuxiliaryReservoir
	{
		/// <summary>The charge rate in Pa/s</summary>
		public readonly double ChargeRate;
		/// <summary>The current pressure</summary>
		public double CurrentPressure;
		/// <summary>The maximum pressure</summary>
		public readonly double MaximumPressure;
		/// <summary>The co-efficient used when transferring pressure to the brake pipe</summary>
		public readonly double BrakePipeCoefficient;
		/// <summary>The co-efficient used when transferring pressure to the brake cylinder</summary>
		public readonly double BrakeCylinderCoefficient;

		public AuxiliaryReservoir(double maximumPressure, double chargeRate, double brakePipeCoefficient, double brakeCylinderCoefficent)
		{
			ChargeRate = chargeRate;
			MaximumPressure = maximumPressure;
			CurrentPressure = maximumPressure;
			BrakePipeCoefficient = brakePipeCoefficient;
			BrakeCylinderCoefficient = brakeCylinderCoefficent;
		}
	}

	/// <summary>An equalising reservoir</summary>
	public class EqualizingReservoir
	{
		/// <summary>The rate when service brakes are applied in Pa/s</summary>
		public readonly double ServiceRate;
		/// <summary>The rate when EB brakes are applied in Pa/s</summary>
		public readonly double EmergencyRate;
		/// <summary>The charge rate in Pa/s</summary>
		public readonly double ChargeRate;
		/// <summary>The current pressure</summary>
		public double CurrentPressure;
		/// <summary>The normal pressure</summary>
		public double NormalPressure;

		public EqualizingReservoir(double serviceRate, double emergencyRate, double chargeRate)
		{
			ServiceRate = serviceRate;
			EmergencyRate = emergencyRate;
			ChargeRate = chargeRate;
			CurrentPressure = 0.0;
		}
	}

	/// <summary>A main reservoir</summary>
	public class MainReservoir
	{
		/// <summary>The current pressure</summary>
		public double CurrentPressure;
		/// <summary>The minimum pressure</summary>
		public readonly double MinimumPressure;
		/// <summary>The maximum pressure</summary>
		public readonly double MaximumPressure;
		/// <summary>The co-efficient used when transferring pressure to the equalizing reservoir</summary>
		public readonly double EqualizingReservoirCoefficient;
		/// <summary>The co-efficient used when transferring pressure to the brake pipe</summary>
		public readonly double BrakePipeCoefficient;

		public MainReservoir(double minimumPressure, double maximumPressure, double equalizingReservoirCoefficient, double brakePipeCoefficient)
		{
			MinimumPressure = minimumPressure;
			MaximumPressure = maximumPressure;
			EqualizingReservoirCoefficient = equalizingReservoirCoefficient;
			BrakePipeCoefficient = brakePipeCoefficient;
			CurrentPressure = MinimumPressure + (MaximumPressure - MinimumPressure) * new Random().NextDouble();
		}
	}
}