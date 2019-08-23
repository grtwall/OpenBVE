﻿using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TrainEditor2.Extensions;
using TrainEditor2.Models.Others;
using TrainEditor2.Models.Trains;
using TrainEditor2.ViewModels.Dialogs;
using TrainEditor2.ViewModels.Others;

namespace TrainEditor2.ViewModels.Trains
{
	internal class MotorViewModel : BaseViewModel
	{
		internal ReadOnlyReactivePropertySlim<MessageBoxViewModel> MessageBox
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<ToolTipViewModel> ToolTipVertexPitch
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<ToolTipViewModel> ToolTipVertexVolume
		{
			get;
		}

		internal ReactiveProperty<InputEventModel.ModifierKeys> CurrentModifierKeys
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<InputEventModel.CursorType> CurrentCursorType
		{
			get;
		}

		internal ReactiveProperty<string> MinVelocity
		{
			get;
		}

		internal ReactiveProperty<string> MaxVelocity
		{
			get;
		}

		internal ReactiveProperty<string> MinPitch
		{
			get;
		}

		internal ReactiveProperty<string> MaxPitch
		{
			get;
		}

		internal ReactiveProperty<string> MinVolume
		{
			get;
		}

		internal ReactiveProperty<string> MaxVolume
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<double> NowVelocity
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<double> NowPitch
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<double> NowVolume
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<Motor.TrackInfo> CurrentSelectedTrack
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<Motor.InputMode> CurrentInputMode
		{
			get;
		}

		internal ReactiveProperty<int> SelectedSoundIndex
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<Motor.ToolMode> CurrentToolMode
		{
			get;
		}

		private ReadOnlyReactivePropertySlim<bool> StoppedSim
		{
			get;
		}

		internal ReadOnlyReactivePropertySlim<bool> EnabledDirect
		{
			get;
		}

		internal ReactiveProperty<string> DirectX
		{
			get;
		}

		internal ReactiveProperty<string> DirectY
		{
			get;
		}

		internal ReactiveProperty<int> ImageWidth
		{
			get;
		}

		internal ReactiveProperty<int> ImageHeight
		{
			get;
		}

		internal ReactiveProperty<Bitmap> Image
		{
			get;
		}

		internal ReactiveCommand<Motor.TrackInfo> ChangeSelectedTrack
		{
			get;
		}

		internal ReactiveCommand<Motor.InputMode> ChangeInputMode
		{
			get;
		}

		internal ReactiveCommand<Motor.ToolMode> ChangeToolMode
		{
			get;
		}

		internal ReactiveCommand ZoomIn
		{
			get;
		}

		internal ReactiveCommand ZoomOut
		{
			get;
		}

		internal ReactiveCommand Reset
		{
			get;
		}

		internal ReactiveCommand MoveLeft
		{
			get;
		}

		internal ReactiveCommand MoveRight
		{
			get;
		}

		internal ReactiveCommand MoveBottom
		{
			get;
		}

		internal ReactiveCommand MoveTop
		{
			get;
		}

		internal ReactiveCommand Undo
		{
			get;
		}

		internal ReactiveCommand Redo
		{
			get;
		}

		internal ReactiveCommand TearingOff
		{
			get;
		}

		internal ReactiveCommand Copy
		{
			get;
		}

		internal ReactiveCommand Paste
		{
			get;
		}

		internal ReactiveCommand Cleanup
		{
			get;
		}

		internal ReactiveCommand Delete
		{
			get;
		}

		internal ReactiveCommand<InputEventModel.EventArgs> MouseDown
		{
			get;
		}

		internal ReactiveCommand<InputEventModel.EventArgs> MouseMove
		{
			get;
		}

		internal ReactiveCommand MouseUp
		{
			get;
		}

		internal ReactiveCommand DirectDot
		{
			get;
		}

		internal ReactiveCommand DirectMove
		{
			get;
		}

		internal MotorViewModel(Motor motor)
		{
			CultureInfo culture = CultureInfo.InvariantCulture;

			MessageBox = motor
				.ObserveProperty(x => x.MessageBox)
				.Do(_ => MessageBox?.Value.Dispose())
				.Select(x => new MessageBoxViewModel(x))
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			ToolTipVertexPitch = motor
				.ObserveProperty(x => x.ToolTipVertexPitch)
				.Do(_ => ToolTipVertexPitch?.Value.Dispose())
				.Select(x => new ToolTipViewModel(x))
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			ToolTipVertexVolume = motor
				.ObserveProperty(x => x.ToolTipVertexVolume)
				.Do(_ => ToolTipVertexVolume?.Value.Dispose())
				.Select(x => new ToolTipViewModel(x))
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			CurrentModifierKeys = motor
				.ToReactivePropertyAsSynchronized(x => x.CurrentModifierKeys)
				.AddTo(disposable);

			CurrentCursorType = motor
				.ObserveProperty(x => x.CurrentCursorType)
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			MinVelocity = motor
				.ToReactivePropertyAsSynchronized(
					x => x.MinVelocity,
					x => x.ToString(culture),
					double.Parse,
					ignoreValidationErrorValue: true
				)
				.AddTo(disposable);

			MinVelocity.Subscribe(_ => motor.DrawImage()).AddTo(disposable);

			MaxVelocity = motor
				.ToReactivePropertyAsSynchronized(
					x => x.MaxVelocity,
					x => x.ToString(culture),
					double.Parse,
					ignoreValidationErrorValue: true
				)
				.AddTo(disposable);

			MaxVelocity.Subscribe(_ => motor.DrawImage()).AddTo(disposable);

			MinPitch = motor
				.ToReactivePropertyAsSynchronized(
					x => x.MinPitch,
					x => x.ToString(culture),
					double.Parse,
					ignoreValidationErrorValue: true
				)
				.AddTo(disposable);

			MinPitch.Subscribe(_ => motor.DrawImage()).AddTo(disposable);

			MaxPitch = motor
				.ToReactivePropertyAsSynchronized(
					x => x.MaxPitch,
					x => x.ToString(culture),
					double.Parse,
					ignoreValidationErrorValue: true
				)
				.AddTo(disposable);

			MaxPitch.Subscribe(_ => motor.DrawImage()).AddTo(disposable);

			MinVolume = motor
				.ToReactivePropertyAsSynchronized(
					x => x.MinVolume,
					x => x.ToString(culture),
					double.Parse,
					ignoreValidationErrorValue: true
				)
				.AddTo(disposable);

			MinVolume.Subscribe(_ => motor.DrawImage()).AddTo(disposable);

			MaxVolume = motor
				.ToReactivePropertyAsSynchronized(
					x => x.MaxVolume,
					x => x.ToString(culture),
					double.Parse,
					ignoreValidationErrorValue: true
				)
				.AddTo(disposable);

			MaxVolume.Subscribe(_ => motor.DrawImage()).AddTo(disposable);

			NowVelocity = motor
				.ObserveProperty(x => x.NowVelocity)
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			NowPitch = motor
				.ObserveProperty(x => x.NowPitch)
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			NowVolume = motor
				.ObserveProperty(x => x.NowVolume)
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			CurrentSelectedTrack = motor
				.ObserveProperty(x => x.SelectedTrackInfo)
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			CurrentSelectedTrack
				.Subscribe(_ =>
				{
					motor.ResetSelect();
					motor.DrawImage();
				})
				.AddTo(disposable);

			CurrentInputMode = motor
				.ObserveProperty(x => x.CurrentInputMode)
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			CurrentInputMode
				.Subscribe(_ =>
				{
					motor.ResetSelect();
					motor.DrawImage();
				})
				.AddTo(disposable);

			SelectedSoundIndex = motor
				.ToReactivePropertyAsSynchronized(x => x.SelectedSoundIndex)
				.AddTo(disposable);

			CurrentToolMode = motor
				.ObserveProperty(x => x.CurrentToolMode)
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			CurrentToolMode
				.Subscribe(_ =>
				{
					motor.ResetSelect();
					motor.DrawImage();
				})
				.AddTo(disposable);

			StoppedSim = motor
				.ObserveProperty(x => x.CurrentSimState)
				.Select(x => x == Motor.SimulationState.Disable || x == Motor.SimulationState.Stopped)
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			EnabledDirect = new[]
				{
					CurrentInputMode.Select(x => x != Motor.InputMode.SoundIndex),
					StoppedSim
				}
				.CombineLatestValuesAreAllTrue()
				.ToReadOnlyReactivePropertySlim()
				.AddTo(disposable);

			DirectX = new ReactiveProperty<string>(0.0.ToString(culture))
				.SetValidateNotifyError(x =>
				{
					double result;
					string message;

					switch (CurrentToolMode.Value)
					{
						case Motor.ToolMode.Move:
							Utilities.TryParse(x, Utilities.NumberRange.Any, out result, out message);
							break;
						case Motor.ToolMode.Dot:
							Utilities.TryParse(x, Utilities.NumberRange.NonNegative, out result, out message);
							break;
						default:
							message = null;
							break;
					}

					return message;
				});

			DirectY = new ReactiveProperty<string>(0.0.ToString(culture))
				.SetValidateNotifyError(x =>
				{
					double result;
					string message;

					switch (CurrentToolMode.Value)
					{
						case Motor.ToolMode.Move:
							Utilities.TryParse(x, Utilities.NumberRange.Any, out result, out message);
							break;
						case Motor.ToolMode.Dot:
							Utilities.TryParse(x, Utilities.NumberRange.NonNegative, out result, out message);
							break;
						default:
							message = null;
							break;
					}

					return message;
				});

			CurrentToolMode
				.Subscribe(_ =>
				{
					DirectX.ForceValidate();
					DirectY.ForceValidate();
				})
				.AddTo(disposable);

			ImageWidth = motor
				.ToReactivePropertyAsSynchronized(
					x => x.ImageWidth,
					ignoreValidationErrorValue: true
				)
				.SetValidateNotifyError(x => x <= 0 ? string.Empty : null)
				.AddTo(disposable);

			ImageWidth.Subscribe(_ => motor.DrawImage()).AddTo(disposable);

			ImageHeight = motor
				.ToReactivePropertyAsSynchronized(
					x => x.ImageHeight,
					ignoreValidationErrorValue: true
				)
				.SetValidateNotifyError(x => x <= 0 ? string.Empty : null)
				.AddTo(disposable);

			ImageHeight.Subscribe(_ => motor.DrawImage()).AddTo(disposable);

			Image = motor
				.ObserveProperty(x => x.Image)
				.ToReactiveProperty()
				.AddTo(disposable);

			ChangeSelectedTrack = new ReactiveCommand<Motor.TrackInfo>();
			ChangeSelectedTrack.Subscribe(x => motor.SelectedTrackInfo = x).AddTo(disposable);

			ChangeInputMode = new ReactiveCommand<Motor.InputMode>();
			ChangeInputMode.Subscribe(x => motor.CurrentInputMode = x).AddTo(disposable);

			ChangeToolMode = new[]
				{
					CurrentInputMode.Select(x => x != Motor.InputMode.SoundIndex),
					StoppedSim
				}
				.CombineLatestValuesAreAllTrue()
				.ToReactiveCommand<Motor.ToolMode>()
				.AddTo(disposable);

			ChangeToolMode.Subscribe(x => motor.CurrentToolMode = x).AddTo(disposable);

			ZoomIn = new ReactiveCommand();
			ZoomIn.Subscribe(motor.ZoomIn).AddTo(disposable);

			ZoomOut = new ReactiveCommand();
			ZoomOut.Subscribe(motor.ZoomOut).AddTo(disposable);

			Reset = new ReactiveCommand();
			Reset.Subscribe(motor.Reset).AddTo(disposable);

			MoveLeft = new ReactiveCommand();
			MoveLeft.Subscribe(motor.MoveLeft).AddTo(disposable);

			MoveRight = new ReactiveCommand();
			MoveRight.Subscribe(motor.MoveRight).AddTo(disposable);

			MoveBottom = new ReactiveCommand();
			MoveBottom.Subscribe(motor.MoveBottom).AddTo(disposable);

			MoveTop = new ReactiveCommand();
			MoveTop.Subscribe(motor.MoveTop).AddTo(disposable);

			Undo = new[]
				{
					new[]
						{
							motor.PropertyChangedAsObservable().Where(x => x.PropertyName == nameof(motor.SelectedTrackInfo)).OfType<object>(),
							motor.PrevTrackStates.CollectionChangedAsObservable().OfType<object>()
						}
						.Merge()
						.Select(_ => motor.PrevTrackStates.Any(x => x.Info == motor.SelectedTrackInfo)),
					StoppedSim
				}
				.CombineLatestValuesAreAllTrue()
				.ToReactiveCommand(false)
				.AddTo(disposable);

			Undo.Subscribe(motor.Undo).AddTo(disposable);

			Redo = new[]
				{
					new[]
						{
							motor.PropertyChangedAsObservable().Where(x => x.PropertyName == nameof(motor.SelectedTrackInfo)).OfType<object>(),
							motor.NextTrackStates.CollectionChangedAsObservable().OfType<object>()
						}
						.Merge()
						.Select(_ => motor.NextTrackStates.Any(x => x.Info == motor.SelectedTrackInfo)),
					StoppedSim
				}
				.CombineLatestValuesAreAllTrue()
				.ToReactiveCommand(false)
				.AddTo(disposable);

			Redo.Subscribe(motor.Redo).AddTo(disposable);

			TearingOff = StoppedSim
				.ToReactiveCommand()
				.AddTo(disposable);

			TearingOff.Subscribe(motor.TearingOff).AddTo(disposable);

			Copy = StoppedSim
				.ToReactiveCommand()
				.AddTo(disposable);

			Copy.Subscribe(motor.Copy).AddTo(disposable);

			Paste = new[]
				{
					motor.ObserveProperty(x => x.CopyTrack).Select(x => x != null),
					StoppedSim
				}
				.CombineLatestValuesAreAllTrue()
				.ToReactiveCommand()
				.AddTo(disposable);

			Paste.Subscribe(motor.Paste).AddTo(disposable);

			Cleanup = StoppedSim
				.ToReactiveCommand()
				.AddTo(disposable);

			Cleanup.Subscribe(motor.Cleanup).AddTo(disposable);

			Delete = StoppedSim
				.ToReactiveCommand()
				.AddTo(disposable);

			Delete.Subscribe(motor.Delete).AddTo(disposable);

			MouseDown = new ReactiveCommand<InputEventModel.EventArgs>();
			MouseDown.Subscribe(motor.MouseDown).AddTo(disposable);

			MouseMove = new ReactiveCommand<InputEventModel.EventArgs>();
			MouseMove.Subscribe(motor.MouseMove).AddTo(disposable);

			MouseUp = new ReactiveCommand();
			MouseUp.Subscribe(motor.MouseUp).AddTo(disposable);

			DirectDot = new[]
				{
					StoppedSim,
					CurrentToolMode.Select(x => x == Motor.ToolMode.Dot),
					DirectX.ObserveHasErrors.Select(x => !x),
					DirectY.ObserveHasErrors.Select(x => !x)
				}
				.CombineLatestValuesAreAllTrue()
				.ToReactiveCommand()
				.AddTo(disposable);

			DirectDot.Subscribe(() => motor.DirectDot(double.Parse(DirectX.Value), double.Parse(DirectY.Value))).AddTo(disposable);

			DirectMove = new[]
				{
					StoppedSim,
					CurrentToolMode.Select(x => x == Motor.ToolMode.Move),
					DirectX.ObserveHasErrors.Select(x => !x),
					DirectY.ObserveHasErrors.Select(x => !x)
				}
				.CombineLatestValuesAreAllTrue()
				.ToReactiveCommand()
				.AddTo(disposable);

			DirectMove.Subscribe(() => motor.DirectMove(double.Parse(DirectX.Value), double.Parse(DirectY.Value))).AddTo(disposable);

			MinVelocity
				.SetValidateNotifyError(x =>
				{
					double min;
					string message;

					if (Utilities.TryParse(x, Utilities.NumberRange.NonNegative, out min, out message))
					{
						double max;

						if (Utilities.TryParse(MaxVelocity.Value, Utilities.NumberRange.NonNegative, out max) && min >= max)
						{
							message = "MinはMax未満でなければなりません。";
						}
					}

					return message;
				})
				.Subscribe(_ => MaxVelocity.ForceValidate())
				.AddTo(disposable);

			MinVelocity.ObserveHasErrors
				.ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DistinctUntilChanged)
				.Where(x => !x)
				.Subscribe(_ => MinVelocity.ForceNotify())
				.AddTo(disposable);

			MaxVelocity
				.SetValidateNotifyError(x =>
				{
					double max;
					string message;

					if (Utilities.TryParse(x, Utilities.NumberRange.NonNegative, out max, out message))
					{
						double min;

						if (Utilities.TryParse(MinVelocity.Value, Utilities.NumberRange.NonNegative, out min) && max <= min)
						{
							message = "MinはMax未満でなければなりません。";
						}
					}

					return message;
				})
				.Subscribe(_ => MinVelocity.ForceValidate())
				.AddTo(disposable);

			MaxVelocity.ObserveHasErrors
				.ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DistinctUntilChanged)
				.Where(x => !x)
				.Subscribe(_ => MaxVelocity.ForceNotify())
				.AddTo(disposable);

			MinPitch
				.SetValidateNotifyError(x =>
				{
					double min;
					string message;

					if (Utilities.TryParse(x, Utilities.NumberRange.NonNegative, out min, out message))
					{
						double max;

						if (Utilities.TryParse(MaxPitch.Value, Utilities.NumberRange.NonNegative, out max) && min >= max)
						{
							message = "MinはMax未満でなければなりません。";
						}
					}

					return message;
				})
				.Subscribe(_ => MaxPitch.ForceValidate())
				.AddTo(disposable);

			MinPitch.ObserveHasErrors
				.ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DistinctUntilChanged)
				.Where(x => !x)
				.Subscribe(_ => MinPitch.ForceNotify())
				.AddTo(disposable);

			MaxPitch
				.SetValidateNotifyError(x =>
				{
					double max;
					string message;

					if (Utilities.TryParse(x, Utilities.NumberRange.NonNegative, out max, out message))
					{
						double min;

						if (Utilities.TryParse(MinPitch.Value, Utilities.NumberRange.NonNegative, out min) && max <= min)
						{
							message = "MinはMax未満でなければなりません。";
						}
					}

					return message;
				})
				.Subscribe(_ => MinPitch.ForceValidate())
				.AddTo(disposable);

			MaxPitch.ObserveHasErrors
				.ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DistinctUntilChanged)
				.Where(x => !x)
				.Subscribe(_ => MaxPitch.ForceNotify())
				.AddTo(disposable);

			MinVolume
				.SetValidateNotifyError(x =>
				{
					double min;
					string message;

					if (Utilities.TryParse(x, Utilities.NumberRange.NonNegative, out min, out message))
					{
						double max;

						if (Utilities.TryParse(MaxVolume.Value, Utilities.NumberRange.NonNegative, out max) && min >= max)
						{
							message = "MinはMax未満でなければなりません。";
						}
					}

					return message;
				})
				.Subscribe(_ => MaxVolume.ForceValidate())
				.AddTo(disposable);

			MinVolume.ObserveHasErrors
				.ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DistinctUntilChanged)
				.Where(x => !x)
				.Subscribe(_ => MinVolume.ForceNotify())
				.AddTo(disposable);

			MaxVolume
				.SetValidateNotifyError(x =>
				{
					double max;
					string message;

					if (Utilities.TryParse(x, Utilities.NumberRange.NonNegative, out max, out message))
					{
						double min;

						if (Utilities.TryParse(MinVolume.Value, Utilities.NumberRange.NonNegative, out min) && max <= min)
						{
							message = "MinはMax未満でなければなりません。";
						}
					}

					return message;
				})
				.Subscribe(_ => MinVolume.ForceValidate())
				.AddTo(disposable);

			MaxVolume.ObserveHasErrors
				.ToReadOnlyReactivePropertySlim(mode: ReactivePropertyMode.DistinctUntilChanged)
				.Where(x => !x)
				.Subscribe(_ => MaxVolume.ForceNotify())
				.AddTo(disposable);
		}
	}
}
