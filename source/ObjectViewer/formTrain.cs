using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrainManager.BrakeSystems;
using OpenBveApi.Trains;
using TrainManager.Car;
using TrainManager.Handles;
using TrainManager.Power;

namespace OpenBve
{
	public partial class formTrain : Form
	{
		private class PluginState
		{
			internal int Number;
			internal int State;

			internal PluginState(int number, int state)
			{
				Number = number;
				State = state;
			}
		}

		private readonly List<PluginState> PluginStates;

		private static Task FormTrainTask;

		private formTrain()
		{
			InitializeComponent();

			checkBoxEnableTrain.Checked = TrainManager.Trains.Length != 0;
			tabControlSettings.Enabled = checkBoxEnableTrain.Checked;

			labelEmergency.Enabled = false;
			numericUpDownEmergency.Enabled = false;
			checkBoxSetHoldBrake.Enabled = false;
			checkBoxSetConstSpeed.Enabled = false;

			if (checkBoxEnableTrain.Checked)
			{
				numericUpDownCars.Value = TrainManager.Trains[0].Cars.Length;
				numericUpDownSpeed.Value = (decimal)(TrainManager.Trains[0].Cars[0].CurrentSpeed * 3.6);
				numericUpDownAccel.Value = (decimal)(TrainManager.Trains[0].Cars[0].Specs.CurrentAcceleration * 3.6);

				numericUpDownMain.Value = (decimal)(TrainManager.Trains[0].Cars[0].CarBrake.mainReservoir.CurrentPressure / 1000.0);
				numericUpDownPipe.Value = (decimal)(TrainManager.Trains[0].Cars[0].CarBrake.brakePipe.CurrentPressure / 1000.0);
				numericUpDownCylinder.Value = (decimal)(TrainManager.Trains[0].Cars[0].CarBrake.brakeCylinder.CurrentPressure / 1000.0);
				numericUpDownAirPipe.Value = (decimal)(TrainManager.Trains[0].Cars[0].CarBrake.straightAirPipe.CurrentPressure / 1000.0);

				numericUpDownLeft.Value = (decimal)TrainManager.Trains[0].Cars[0].Doors[0].State;
				numericUpDownRight.Value = (decimal)TrainManager.Trains[0].Cars[0].Doors[1].State;
				checkBoxLeftTarget.Checked = TrainManager.Trains[0].Cars[0].Doors[0].AnticipatedOpen;
				checkBoxRightTarget.Checked = TrainManager.Trains[0].Cars[0].Doors[1].AnticipatedOpen;

				numericUpDownReverser.Value = (decimal)TrainManager.Trains[0].Handles.Reverser.Driver;
				numericUpDownPowerNotch.Value = TrainManager.Trains[0].Handles.Power.Driver;
				numericUpDownPowerNotches.Value = TrainManager.Trains[0].Handles.Power.MaximumNotch;
				checkBoxAirBrake.Checked = TrainManager.Trains[0].Cars[0].CarBrake is AutomaticAirBrake;
				if (checkBoxAirBrake.Checked)
				{
					numericUpDownBrakeNotch.Value = (int)TrainManager.Trains[0].Handles.Brake.Driver;
					numericUpDownBrakeNotch.Maximum = 2;
					numericUpDownBrakeNotches.Value = 2;
					numericUpDownBrakeNotches.Enabled = false;
					checkBoxHoldBrake.Enabled = false;
				}
				else
				{
					numericUpDownBrakeNotch.Value = TrainManager.Trains[0].Handles.Brake.Driver;
					numericUpDownBrakeNotches.Value = TrainManager.Trains[0].Handles.Brake.MaximumNotch;
					checkBoxHoldBrake.Checked = TrainManager.Trains[0].Handles.HasHoldBrake;
					if (checkBoxHoldBrake.Checked)
					{
						checkBoxSetHoldBrake.Enabled = true;
						checkBoxSetHoldBrake.Checked = TrainManager.Trains[0].Handles.HoldBrake.Driver;
					}
				}
				checkBoxSetEmergency.Checked = TrainManager.Trains[0].Handles.EmergencyBrake.Driver;
				checkBoxConstSpeed.Checked = TrainManager.Trains[0].Specs.HasConstSpeed;
				if (checkBoxConstSpeed.Checked)
				{
					checkBoxSetConstSpeed.Enabled = true;
					checkBoxSetConstSpeed.Checked = TrainManager.Trains[0].Specs.CurrentConstSpeed;
				}

				checkBoxEnablePlugin.Checked = TrainManager.Trains[0].Specs.SafetySystemPlugin;
			}
			panelPlugin.Enabled = checkBoxEnablePlugin.Checked;
			buttonRemove.Enabled = false;
			labelNumber.Enabled = false;
			numericUpDownNumber.Enabled = false;
			labelState.Enabled = false;
			numericUpDownState.Enabled = false;
			PluginStates = new List<PluginState>();
			for (int i = 0; i < PluginManager.CurrentPlugin.Panel.Length; i++)
			{
				if (PluginManager.CurrentPlugin.Panel[i] != 0)
				{
					PluginStates.Add(new PluginState(i, PluginManager.CurrentPlugin.Panel[i]));
				}
			}
			ListPluginStates();
		}

		internal static void ShowTrainSettings()
		{
			if (FormTrainTask == null || FormTrainTask.IsCompleted)
			{
				FormTrainTask = Task.Factory.StartNew(() =>
				{
					formTrain dialog = new formTrain();
					dialog.ShowDialog();
				});
			}
		}

		private void checkBoxEnableTrain_Check(object sender, EventArgs e)
		{
			tabControlSettings.Enabled = checkBoxEnableTrain.Checked;
		}

		private void checkBoxAirBrake_Check(object sender, EventArgs e)
		{
			if (checkBoxAirBrake.Checked)
			{
				if (numericUpDownBrakeNotch.Value > 2)
				{
					numericUpDownBrakeNotch.Value = 2;
				}
				numericUpDownBrakeNotch.Maximum = 2;
				numericUpDownBrakeNotches.Value = 2;
				numericUpDownBrakeNotches.Enabled = false;
				checkBoxHoldBrake.Checked = false;
				checkBoxHoldBrake.Enabled = false;
				checkBoxSetHoldBrake.Checked = false;
				checkBoxSetHoldBrake.Enabled = false;
			}
			else
			{
				numericUpDownBrakeNotch.Maximum = 100;
				numericUpDownBrakeNotches.Enabled = true;
				checkBoxHoldBrake.Enabled = true;
			}
		}

		private void checkBoxHoldBrake_Check(object sender, EventArgs e)
		{
			if (checkBoxHoldBrake.Checked)
			{
				checkBoxSetHoldBrake.Enabled = true;
			}
			else
			{
				checkBoxSetHoldBrake.Checked = false;
				checkBoxSetHoldBrake.Enabled = false;
			}
		}

		private void checkBoxConstSpeed_Check(object sender, EventArgs e)
		{
			if (checkBoxConstSpeed.Checked)
			{
				checkBoxSetConstSpeed.Enabled = true;
			}
			else
			{
				checkBoxSetConstSpeed.Checked = false;
				checkBoxSetConstSpeed.Enabled = false;
			}
		}

		private void checkBoxEnablePlugin_Check(object sender, EventArgs e)
		{
			panelPlugin.Enabled = checkBoxEnablePlugin.Checked;
		}

		private void ListPluginStates()
		{
			ListViewItem[] items = new ListViewItem[PluginStates.Count];
			for (int i = 0; i < items.Length; i++)
			{
				items[i] = new ListViewItem(new[] { "", "" });
				UpdatePluginListViewItem(items[i], i, false);
			}
			listViewPlugin.Items.AddRange(items);
			if (items.Length != 0)
			{
				listViewPlugin.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			}
			else
			{
				listViewPlugin.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
			}
		}

		private void UpdatePluginListViewItem(ListViewItem item, int index, bool resizeColumns)
		{
			item.SubItems[0].Text = PluginStates[index].Number.ToString();
			item.SubItems[1].Text = PluginStates[index].State.ToString();
			if (resizeColumns)
			{
				listViewPlugin.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			}
		}

		private void UpdatePluginComponent(PluginState state)
		{
			buttonRemove.Enabled = true;
			labelNumber.Enabled = true;
			numericUpDownNumber.Enabled = true;
			numericUpDownNumber.Value = state.Number;
			labelState.Enabled = true;
			numericUpDownState.Enabled = true;
			numericUpDownState.Value = state.State;
		}

		private void listViewPlugin_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewPlugin.SelectedIndices.Count == 1)
			{
				int i = listViewPlugin.SelectedIndices[0];
				Tag = new object();
				UpdatePluginComponent(PluginStates[i]);
				Tag = null;
			}
			else
			{
				Tag = new object();
				buttonRemove.Enabled = false;
				labelNumber.Enabled = false;
				numericUpDownNumber.Enabled = false;
				labelState.Enabled = false;
				numericUpDownState.Enabled = false;
				Tag = null;
			}
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			int n = PluginStates.Count;
			for (int i = 0; i < n; i++)
			{
				listViewPlugin.Items[i].Selected = false;
			}
			PluginStates.Add(new PluginState(0, 0));
			ListViewItem item = new ListViewItem(new[] { "", "" });
			UpdatePluginListViewItem(item, n, true);
			listViewPlugin.Items.Add(item);
			item.Selected = true;
		}

		private void buttonRemove_Click(object sender, EventArgs e)
		{
			if (listViewPlugin.SelectedIndices.Count == 1)
			{
				int i = listViewPlugin.SelectedIndices[0];
				PluginStates.RemoveAt(i);
				listViewPlugin.Items[i].Remove();
			}
		}

		private void numericUpDownNumber_ValueChanged(object sender, EventArgs e)
		{
			if (Tag == null && listViewPlugin.SelectedIndices.Count == 1)
			{
				int i = listViewPlugin.SelectedIndices[0];
				PluginStates[i].Number = (int)numericUpDownNumber.Value;
				UpdatePluginListViewItem(listViewPlugin.Items[i], i, true);
			}
		}

		private void numericUpDownState_ValueChanged(object sender, EventArgs e)
		{
			if (Tag == null && listViewPlugin.SelectedIndices.Count == 1)
			{
				int i = listViewPlugin.SelectedIndices[0];
				PluginStates[i].State = (int)numericUpDownState.Value;
				UpdatePluginListViewItem(listViewPlugin.Items[i], i, true);
			}
		}

		private void ApplyTrain()
		{
			lock (Program.LockObj)
			{
				TrainManager.Trains = new TrainManager.Train[] { };
				if (checkBoxEnableTrain.Checked)
				{
					Array.Resize(ref TrainManager.Trains, 1);
					TrainManager.Trains[0] = new TrainManager.Train
					{
						State = TrainState.Available
					};
					Array.Resize(ref TrainManager.Trains[0].Cars, (int)numericUpDownCars.Value);
					for (int i = 0; i < TrainManager.Trains[0].Cars.Length; i++)
					{
						TrainManager.Trains[0].Cars[i] = new TrainManager.Car(TrainManager.Trains[0]);
						TrainManager.Trains[0].Cars[i].CurrentSpeed = (int)numericUpDownSpeed.Value / 3.6;
						TrainManager.Trains[0].Cars[i].Specs.CurrentPerceivedSpeed = (int)numericUpDownSpeed.Value / 3.6;
						TrainManager.Trains[0].Cars[i].Specs.CurrentAcceleration = (int)numericUpDownAccel.Value / 3.6;
						if (checkBoxAirBrake.Checked)
						{
							TrainManager.Trains[0].Cars[i].CarBrake = new AutomaticAirBrake(EletropneumaticBrakeType.None, TrainManager.Trains[0].Handles.EmergencyBrake, TrainManager.Trains[0].Handles.Reverser, true, 0.0, 0.0, new AccelerationCurve[] {});
						}
						else
						{
							TrainManager.Trains[0].Cars[i].CarBrake = new ElectromagneticStraightAirBrake(EletropneumaticBrakeType.None, TrainManager.Trains[0].Handles.EmergencyBrake, TrainManager.Trains[0].Handles.Reverser, true, 0.0, 0.0, new AccelerationCurve[] {});
						}
						//At the minute, Object Viewer uses dummy brake systems
						TrainManager.Trains[0].Cars[i].CarBrake.mainReservoir = new MainReservoir((int)numericUpDownMain.Value * 1000);
						TrainManager.Trains[0].Cars[i].CarBrake.brakePipe = new BrakePipe((int)numericUpDownPipe.Value * 1000);
						TrainManager.Trains[0].Cars[i].CarBrake.brakeCylinder = new BrakeCylinder((int)numericUpDownCylinder.Value * 1000);
						TrainManager.Trains[0].Cars[i].CarBrake.straightAirPipe = new StraightAirPipe((int)numericUpDownAirPipe.Value * 1000);

						TrainManager.Trains[0].Cars[i].Doors[0] = new Door
						{
							Direction = -1, 
							State = (double) numericUpDownLeft.Value,
							AnticipatedOpen = checkBoxLeftTarget.Checked
						};
						TrainManager.Trains[0].Cars[i].Doors[1] = new Door
						{
							Direction = 1, 
							State = (double) numericUpDownRight.Value,
							AnticipatedOpen = checkBoxRightTarget.Checked
						};
					}

					TrainManager.Trains[0].Handles.Reverser.Driver = (ReverserPosition)numericUpDownReverser.Value;
					TrainManager.Trains[0].Handles.Reverser.Actual = (ReverserPosition)numericUpDownReverser.Value;
					if ((int)numericUpDownPowerNotches.Value != TrainManager.Trains[0].Handles.Power.MaximumNotch)
					{
						TrainManager.Trains[0].Handles.Power = new PowerHandle((int)numericUpDownPowerNotches.Value, (int)numericUpDownPowerNotches.Value, new double[] {}, new double[] {});
					}
					TrainManager.Trains[0].Handles.Power.Driver = (int)numericUpDownPowerNotch.Value;
					if (checkBoxAirBrake.Checked)
					{
						TrainManager.Trains[0].Handles.Brake.Driver = (int)numericUpDownBrakeNotch.Value;
					}
					else
					{
						if ((int)numericUpDownBrakeNotches.Value != TrainManager.Trains[0].Handles.Brake.MaximumNotch)
						{
							TrainManager.Trains[0].Handles.Brake = new BrakeHandle((int)numericUpDownBrakeNotches.Value, (int)numericUpDownBrakeNotches.Value, null, new double[] {}, new double[] {});
						}
						TrainManager.Trains[0].Handles.Brake.Driver = (int)numericUpDownBrakeNotch.Value;
						TrainManager.Trains[0].Handles.HasHoldBrake = checkBoxHoldBrake.Checked;
						if (checkBoxHoldBrake.Checked)
						{
							TrainManager.Trains[0].Handles.HoldBrake.Driver = checkBoxSetHoldBrake.Checked;
						}
					}
					TrainManager.Trains[0].Handles.EmergencyBrake.Driver = checkBoxSetEmergency.Checked;
					TrainManager.Trains[0].Specs.HasConstSpeed = checkBoxConstSpeed.Checked;
					if (checkBoxConstSpeed.Checked)
					{
						TrainManager.Trains[0].Specs.CurrentConstSpeed = checkBoxSetConstSpeed.Checked;
					}

					TrainManager.Trains[0].Specs.SafetySystemPlugin = checkBoxEnablePlugin.Checked;
					if (checkBoxEnablePlugin.Checked && PluginStates.Count != 0)
					{
						PluginManager.CurrentPlugin.Panel = new int[PluginStates.Max(value => value.Number) + 1];
						foreach (PluginState state in PluginStates)
						{
							PluginManager.CurrentPlugin.Panel[state.Number] = state.State;
						}
					}
					else
					{
						PluginManager.CurrentPlugin.Panel = new int[] { };
					}
				}
			}
		}

		private void buttonApply_Click(object sender, EventArgs e)
		{
			ApplyTrain();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			ApplyTrain();
			Close();
		}
	}
}
