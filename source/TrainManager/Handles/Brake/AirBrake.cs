﻿namespace TrainManager.Handles
{
	/// <summary>Represents an air-brake handle</summary>
	public class AirBrakeHandle : AbstractHandle
	{
		private AirBrakeHandleState DelayedValue;
		private double DelayedTime;

		public AirBrakeHandle()
		{
			this.MaximumNotch = 3;
		}

		public override void Update(double CurrentTime)
		{
			if (DelayedValue != AirBrakeHandleState.Invalid)
			{
				if (DelayedTime <= CurrentTime)
				{
					Actual = (int)DelayedValue;
					DelayedValue = AirBrakeHandleState.Invalid;
				}
			}
			else
			{
				if (Safety == (int)AirBrakeHandleState.Release & Actual != (int)AirBrakeHandleState.Release)
				{
					DelayedValue = AirBrakeHandleState.Release;
					DelayedTime = CurrentTime;
				}
				else if (Safety == (int)AirBrakeHandleState.Service & Actual != (int)AirBrakeHandleState.Service)
				{
					DelayedValue = AirBrakeHandleState.Service;
					DelayedTime = CurrentTime;
				}
				else if (Safety == (int)AirBrakeHandleState.Lap)
				{
					Actual = (int)AirBrakeHandleState.Lap;
				}
			}
		}
	}
}