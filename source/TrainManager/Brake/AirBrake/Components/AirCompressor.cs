using OpenBveApi.Trains;
using SoundManager;

namespace TrainManager.Brake
{
	/// <summary>An air compressor</summary>
	public class Compressor
	{
		/// <summary>Whether this compressor is currently active</summary>
		private bool Enabled;
		/// <summary>The compression rate in Pa/s</summary>
		private readonly double Rate;
		/// <summary>The sound played when the compressor loop starts</summary>
		public CarSound StartSound;
		/// <summary>The sound played whilst the compressor is running</summary>
		public CarSound LoopSound;
		/// <summary>The sound played when the compressor loop stops</summary>
		public CarSound EndSound;
		/// <summary>Whether the sound loop has started</summary>
		private bool LoopStarted;
		/// <summary>Stores the time the compressor has been active for</summary>
		private double ActiveTime;
		/// <summary>Holds the reference to the main reservoir</summary>
		private readonly MainReservoir mainReservoir;
		/// <summary>Holds the reference to the car</summary>
		private readonly AbstractCar baseCar;
		/// <summary>Allows for playback of associated sounds</summary>
		private readonly SoundsBase Sounds;

		public Compressor(double rate, MainReservoir reservoir, AbstractCar car, SoundsBase sounds)
		{
			Rate = rate;
			Enabled = false;
			StartSound = new CarSound();
			LoopSound = new CarSound();
			EndSound = new CarSound();
			mainReservoir = reservoir;
			baseCar = car;
			Sounds = sounds;
		}

		public void Update(double TimeElapsed)
		{
			if (Enabled)
			{
				if (mainReservoir.CurrentPressure > mainReservoir.MaximumPressure)
				{
					ActiveTime = 0;
					Enabled = false;
					LoopStarted = false;
					SoundBuffer buffer = EndSound.Buffer;
					if (buffer != null && Sounds != null)
					{
						Sounds.PlaySound(buffer, 1.0, 1.0, EndSound.Position, baseCar, false);
					}

					buffer = LoopSound.Buffer;
					if (buffer != null)
					{
						LoopSound.Stop();
					}
				}
				else
				{
					ActiveTime += TimeElapsed;
					mainReservoir.CurrentPressure += Rate * TimeElapsed;
					if (!LoopStarted && ActiveTime > 5.0)
					{
						LoopStarted = true;
						SoundBuffer buffer = LoopSound.Buffer;
						if (buffer != null && Sounds != null)
						{
							LoopSound.Source = Sounds.PlaySound(buffer, 1.0, 1.0, LoopSound.Position, baseCar, true);
						}
					}
				}
			}
			else
			{
				if (mainReservoir.CurrentPressure < mainReservoir.MinimumPressure)
				{
					ActiveTime = 0;
					Enabled = true;
					SoundBuffer buffer = StartSound.Buffer;
					if (buffer != null && Sounds != null)
					{
						Sounds.PlaySound(buffer, 1.0, 1.0, StartSound.Position, baseCar, false);
					}
				}
			}
		}
	}
}
