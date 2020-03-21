using System;
using System.Collections.Generic;
using OpenBveApi.Colors;
using OpenBveApi.Math;
using OpenBveApi.Objects;
using OpenBveApi.Interface;
using OpenBveApi.Routes;
using OpenBveApi.Trains;
using RouteManager2;
using RouteManager2.SignalManager;
using RouteManager2.Stations;

namespace OpenBve {
	internal partial class CsvRwRouteParser {
		internal static string ObjectPath;
		internal static string SoundPath;
		internal static string TrainPath;
		internal static string CompatibilityFolder;
		internal static bool CylinderHack = false;
		internal static bool IsRW = false;

		internal static CurrentRoute CurrentRoute;
		// parse route
		internal static void ParseRoute(string FileName, bool isRW, System.Text.Encoding Encoding, string trainPath, string objectPath, string soundPath, bool PreviewOnly, out object route) {
			// initialize data
			CurrentRoute = new CurrentRoute(Program.Renderer);
			CurrentRoute.Tracks[0] = new Track();
			/*
			 * Store paths for later use
			 */
			ObjectPath = objectPath;
			SoundPath = soundPath;
			TrainPath = trainPath;
			IsRW = isRW;
			if (!PreviewOnly)
			{
				for (int i = 0; i < Program.CurrentHost.Plugins.Length; i++)
				{
					if (Program.CurrentHost.Plugins[i].Object != null)
					{
						Program.CurrentHost.Plugins[i].Object.SetObjectParser(SoundPath); //HACK: Pass out the sound folder path to those plugins which consume it
					}
				}
			}
			freeObjCount = 0;
			railtypeCount = 0;
			Program.CurrentOptions.UnitOfSpeed = "km/h";
			Program.CurrentOptions.SpeedConversionFactor = 0.0;
			CompatibilityFolder = Program.FileSystem.GetDataFolder("Compatibility");
			if (!PreviewOnly)
			{
				CompatibilityObjects.LoadCompatibilityObjects(OpenBveApi.Path.CombineFile(CompatibilityFolder,"CompatibilityObjects.xml"));
			}
			RouteData Data = new RouteData
			{
				BlockInterval = 25.0,
				AccurateObjectDisposal = false,
				FirstUsedBlock = -1,
				Blocks = new Block[1]
			};
			Data.Blocks[0] = new Block();
			Data.Blocks[0].Rails = new Dictionary<int, Rail>();
			Data.Blocks[0].Rails.Add(0, new Rail { RailStarted =  true });
			Data.Blocks[0].RailType = new int[] { 0 };
			Data.Blocks[0].Limits = new Limit[] { };
			Data.Blocks[0].StopPositions = new Stop[] { };
			Data.Blocks[0].Station = -1;
			Data.Blocks[0].StationPassAlarm = false;
			Data.Blocks[0].Accuracy = 2.0;
			Data.Blocks[0].AdhesionMultiplier = 1.0;
			Data.Blocks[0].CurrentTrackState = new TrackElement(0.0);
			if (!PreviewOnly)
			{
				Data.Blocks[0].Background = 0;
				Data.Blocks[0].BrightnessChanges = new Brightness[] {};
				Data.Blocks[0].Fog.Start = CurrentRoute.NoFogStart;
				Data.Blocks[0].Fog.End = CurrentRoute.NoFogEnd;
				Data.Blocks[0].Fog.Color = Color24.Grey;
				Data.Blocks[0].Cycle = new int[] {-1};
				Data.Blocks[0].RailCycles = new RailCycle[1];
				Data.Blocks[0].RailCycles[0].RailCycleIndex = -1;
				Data.Blocks[0].Height = IsRW ? 0.3 : 0.0;
				Data.Blocks[0].RailFreeObj = new FreeObj[][] {};
				Data.Blocks[0].GroundFreeObj = new FreeObj[] {};
				Data.Blocks[0].RailWall = new WallDike[] {};
				Data.Blocks[0].RailDike = new WallDike[] {};
				Data.Blocks[0].RailPole = new Pole[] {};
				Data.Blocks[0].Forms = new Form[] {};
				Data.Blocks[0].Cracks = new Crack[] {};
				Data.Blocks[0].Signals = new Signal[] {};
				Data.Blocks[0].Sections = new Section[] {};
				Data.Blocks[0].SoundEvents = new Sound[] {};
				Data.Blocks[0].Transponders = new Transponder[] {};
				Data.Blocks[0].DestinationChanges = new DestinationEvent[] {};
				Data.Blocks[0].PointsOfInterest = new PointOfInterest[] {};
				Data.Markers = new Marker[] {};
				Data.RequestStops = new StopRequest[] { };
				string PoleFolder = OpenBveApi.Path.CombineDirectory(CompatibilityFolder, "Poles");
				Data.Structure.Poles = new PoleDictionary();
				Data.Structure.Poles.Add(0, new ObjectDictionary());
				Data.Structure.Poles[0].Add(0, LoadStaticObject(OpenBveApi.Path.CombineFile(PoleFolder, "pole_1.csv"), System.Text.Encoding.UTF8, false));
				Data.Structure.Poles.Add(1, new ObjectDictionary());
				Data.Structure.Poles[1].Add(0, LoadStaticObject(OpenBveApi.Path.CombineFile(PoleFolder, "pole_2.csv"), System.Text.Encoding.UTF8, false));
				Data.Structure.Poles.Add(2, new ObjectDictionary());
				Data.Structure.Poles[2].Add(0, LoadStaticObject(OpenBveApi.Path.CombineFile(PoleFolder, "pole_3.csv"), System.Text.Encoding.UTF8, false));
				Data.Structure.Poles.Add(3, new ObjectDictionary());
				Data.Structure.Poles[3].Add(0, LoadStaticObject(OpenBveApi.Path.CombineFile(PoleFolder, "pole_4.csv"), System.Text.Encoding.UTF8, false));
				
				Data.Structure.RailObjects = new ObjectDictionary();
				Data.Structure.RailObjects = new ObjectDictionary();
				Data.Structure.Ground = new ObjectDictionary();
				Data.Structure.WallL = new ObjectDictionary();
				Data.Structure.WallR = new ObjectDictionary();
				Data.Structure.DikeL = new ObjectDictionary();
				Data.Structure.DikeR = new ObjectDictionary();
				Data.Structure.FormL = new ObjectDictionary();
				Data.Structure.FormR = new ObjectDictionary();
				Data.Structure.FormCL = new ObjectDictionary();
				Data.Structure.FormCR = new ObjectDictionary();
				Data.Structure.RoofL = new ObjectDictionary();
				Data.Structure.RoofR = new ObjectDictionary();
				Data.Structure.RoofCL = new ObjectDictionary();
				Data.Structure.RoofCR = new ObjectDictionary();
				Data.Structure.CrackL = new ObjectDictionary();
				Data.Structure.CrackR = new ObjectDictionary();
				Data.Structure.FreeObjects = new ObjectDictionary();
				Data.Structure.Beacon = new ObjectDictionary();
				Data.Structure.Cycles = new int[][] {};
				Data.Structure.RailCycles = new int[][] { };
				Data.Structure.Run = new int[] {};
				Data.Structure.Flange = new int[] {};
				Data.Backgrounds = new BackgroundDictionary();
				Data.TimetableDaytime = new OpenBveApi.Textures.Texture[] {null, null, null, null};
				Data.TimetableNighttime = new OpenBveApi.Textures.Texture[] {null, null, null, null};
				// signals
				string SignalFolder = OpenBveApi.Path.CombineDirectory(CompatibilityFolder, "Signals");
				Data.Signals = new SignalDictionary();
				Data.Signals.Add(3, new CompatibilitySignalData(new int[] {0, 2, 4}, new StaticObject[]
				{
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_3_0.csv"), System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_3_2.csv"), System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_3_4.csv"), System.Text.Encoding.UTF8, false)
				}));
				Data.Signals.Add(4, new CompatibilitySignalData(new int[] {0, 1, 2, 4}, new StaticObject[]
				{
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4_0.csv"), System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4a_1.csv"), System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4a_2.csv"), System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4a_4.csv"), System.Text.Encoding.UTF8, false)
				}));
				Data.Signals.Add(5, new CompatibilitySignalData(new int[] {0, 1, 2, 3, 4}, new StaticObject[]
				{
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_0.csv"), System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5a_1.csv"), System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_2.csv"), System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_3.csv"), System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_4.csv"), System.Text.Encoding.UTF8, false)
				}));
				Data.Signals.Add(6, new CompatibilitySignalData(new int[] {0, 3, 4}, new StaticObject[]
				{
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "repeatingsignal_0.csv"),
						System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "repeatingsignal_3.csv"),
						System.Text.Encoding.UTF8, false),
					LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "repeatingsignal_4.csv"),
						System.Text.Encoding.UTF8, false)
				}));
				// compatibility signals
				Data.CompatibilitySignals = new CompatibilitySignalData[9];
				Data.CompatibilitySignals[0] = new CompatibilitySignalData(new int[] {0, 2},
					new StaticObject[]
					{
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_2_0.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_2a_2.csv"), System.Text.Encoding.UTF8, false)
					});
				Data.CompatibilitySignals[1] = new CompatibilitySignalData(new int[] {0, 4},
					new StaticObject[]
					{
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_2_0.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_2b_4.csv"), System.Text.Encoding.UTF8, false)
					});
				Data.CompatibilitySignals[2] = new CompatibilitySignalData(new int[] {0, 2, 4},
					new StaticObject[]
					{
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_3_0.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_3_2.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_3_4.csv"), System.Text.Encoding.UTF8, false)
					});
				Data.CompatibilitySignals[3] = new CompatibilitySignalData(new int[] {0, 1, 2, 4},
					new StaticObject[]
					{
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4_0.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4a_1.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4a_2.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4a_4.csv"), System.Text.Encoding.UTF8, false)
					});
				Data.CompatibilitySignals[4] = new CompatibilitySignalData(new int[] {0, 2, 3, 4},
					new StaticObject[]
					{
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4_0.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4b_2.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4b_3.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_4b_4.csv"), System.Text.Encoding.UTF8, false)
					});
				Data.CompatibilitySignals[5] = new CompatibilitySignalData(new int[] {0, 1, 2, 3, 4},
					new StaticObject[]
					{
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_0.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5a_1.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_2.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_3.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_4.csv"), System.Text.Encoding.UTF8, false)
					});
				Data.CompatibilitySignals[6] = new CompatibilitySignalData(new int[] {0, 2, 3, 4, 5},
					new StaticObject[]
					{
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_0.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_2.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_3.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5_4.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_5b_5.csv"), System.Text.Encoding.UTF8, false)
					});
				Data.CompatibilitySignals[7] = new CompatibilitySignalData(new int[] {0, 1, 2, 3, 4, 5},
					new StaticObject[]
					{
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_6_0.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_6_1.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_6_2.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_6_3.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_6_4.csv"), System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "signal_6_5.csv"), System.Text.Encoding.UTF8, false)
					});
				Data.CompatibilitySignals[8] = new CompatibilitySignalData(new int[] {0, 3, 4},
					new StaticObject[]
					{
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "repeatingsignal_0.csv"),
							System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "repeatingsignal_3.csv"),
							System.Text.Encoding.UTF8, false),
						LoadStaticObject(OpenBveApi.Path.CombineFile(SignalFolder, "repeatingsignal_4.csv"),
							System.Text.Encoding.UTF8, false)
					});
				// game data
				CurrentRoute.Sections = new []{new RouteManager2.SignalManager.Section() };
				CurrentRoute.Sections[0].Aspects = new SectionAspect[]
				{new SectionAspect(0, 0.0), new SectionAspect(4, double.PositiveInfinity)};
				CurrentRoute.Sections[0].CurrentAspect = 0;
				CurrentRoute.Sections[0].NextSection = null;
				CurrentRoute.Sections[0].PreviousSection = null;
				CurrentRoute.Sections[0].StationIndex = -1;
				CurrentRoute.Sections[0].TrackPosition = 0;
				CurrentRoute.Sections[0].Trains = new AbstractTrain[] {};

				/*
				 * These are the speed limits for the default Japanese signal aspects, and in most cases will be overwritten
				 */
				Data.SignalSpeeds = new double[]
				{0.0, 6.94444444444444, 15.2777777777778, 20.8333333333333, double.PositiveInfinity, double.PositiveInfinity};
			}
			ParseRouteForData(FileName, Encoding, ref Data, PreviewOnly);
			if (Program.Cancel)
			{
				route = null;
				return;
			}
			ApplyRouteData(FileName, ref Data, PreviewOnly);
			route = CurrentRoute;

//		    if (PreviewOnly == true && customLoadScreen == false)
//		    {
//		        Renderer.TextureLogo = null;
//		    }
		}

		// ================================

		// parse route for data
		
		private static void ParseRouteForData(string FileName, System.Text.Encoding Encoding, ref RouteData Data, bool PreviewOnly) {
			//Read the entire routefile into memory
			string[] Lines = System.IO.File.ReadAllLines(FileName, Encoding);
			Expression[] Expressions;
			PreprocessSplitIntoExpressions(FileName, Lines, out Expressions, true, 0.0);
			PreprocessChrRndSub(FileName, Encoding, ref Expressions);
			double[] UnitOfLength = new double[] { 1.0 };
			//Set units of speed initially to km/h
			//This represents 1km/h in m/s
			Data.UnitOfSpeed = 0.277777777777778;
			PreprocessOptions(Expressions, ref Data, ref UnitOfLength, PreviewOnly);
			PreprocessSortByTrackPosition(UnitOfLength, ref Expressions);
			ParseRouteForData(FileName, Encoding, Expressions, UnitOfLength, ref Data, PreviewOnly);
			CurrentRoute.UnitOfLength = UnitOfLength;
		}

		// preprocess sort by track position
		private struct PositionedExpression {
			internal double TrackPosition;
			internal Expression Expression;
		}
		

		private static int freeObjCount = 0;
		private static int railtypeCount = 0;

		// parse route for data
		private static void ParseRouteForData(string FileName, System.Text.Encoding Encoding, Expression[] Expressions, double[] UnitOfLength, ref RouteData Data, bool PreviewOnly) {
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			string Section = ""; bool SectionAlwaysPrefix = false;
			int BlockIndex = 0;
			int BlocksUsed = Data.Blocks.Length;
			CurrentRoute.Stations = new RouteStation[] { };
			Data.RequestStops = new StopRequest[] { };
			int CurrentStation = -1;
			int CurrentStop = -1;
			bool DepartureSignalUsed = false;
			int CurrentSection = 0;
			double progressFactor = Expressions.Length == 0 ? 0.3333 : 0.3333 / (double)Expressions.Length;
			// process non-track namespaces
			//Check for any special-cased fixes we might need
			CheckRouteSpecificFixes(FileName, ref Data, ref Expressions, PreviewOnly);
			//Apply parameters to object loaders
			if (!PreviewOnly)
			{
				for (int i = 0; i < Program.CurrentHost.Plugins.Length; i++)
				{
					if (Program.CurrentHost.Plugins[i].Object != null)
					{
						Program.CurrentHost.Plugins[i].Object.SetCompatibilityHacks(Program.CurrentOptions.EnableBveTsHacks, CylinderHack);
						//Remember that these will be ignored if not the correct plugin
						Program.CurrentHost.Plugins[i].Object.SetObjectParser(Program.CurrentOptions.CurrentXParser);
						Program.CurrentHost.Plugins[i].Object.SetObjectParser(Program.CurrentOptions.CurrentObjParser);
					}
				}
			}
			
			for (int j = 0; j < Expressions.Length; j++) {
				Program.CurrentProgress = (double)j * progressFactor;
				if ((j & 255) == 0) {
					System.Threading.Thread.Sleep(1);
					if (Program.Cancel) return;
				}
				if (Expressions[j].Text.StartsWith("[") & Expressions[j].Text.EndsWith("]")) {
					Section = Expressions[j].Text.Substring(1, Expressions[j].Text.Length - 2).Trim(new char[] { });
					if (string.Compare(Section, "object", StringComparison.OrdinalIgnoreCase) == 0) {
						Section = "Structure";
					} else if (string.Compare(Section, "railway", StringComparison.OrdinalIgnoreCase) == 0) {
						Section = "Track";
					}
					SectionAlwaysPrefix = true;
				} else {
					// find equals
					Expressions[j].ConvertRwToCsv(Section, SectionAlwaysPrefix);
					// separate command and arguments
					string Command, ArgumentSequence;
					Expressions[j].SeparateCommandsAndArguments(out Command, out ArgumentSequence, Culture, false, IsRW, Section);
					// process command
					double Number;
					bool NumberCheck = !IsRW || string.Compare(Section, "track", StringComparison.OrdinalIgnoreCase) == 0;
					if (NumberCheck && NumberFormats.TryParseDouble(Command, UnitOfLength, out Number)) {
						// track position (ignored)
					} else {
						// split arguments
						string[] Arguments;
						{
							int n = 0;
							for (int k = 0; k < ArgumentSequence.Length; k++) {
								if (IsRW & ArgumentSequence[k] == ',') {
									n++;
								} else if (ArgumentSequence[k] == ';') {
									n++;
								}
							}
							Arguments = new string[n + 1];
							int a = 0, h = 0;
							for (int k = 0; k < ArgumentSequence.Length; k++) {
								if (IsRW & ArgumentSequence[k] == ',') {
									Arguments[h] = ArgumentSequence.Substring(a, k - a).Trim(new char[] { });
									a = k + 1; h++;
								} else if (ArgumentSequence[k] == ';') {
									Arguments[h] = ArgumentSequence.Substring(a, k - a).Trim(new char[] { });
									a = k + 1; h++;
								}
							}
							if (ArgumentSequence.Length - a > 0) {
								Arguments[h] = ArgumentSequence.Substring(a).Trim(new char[] { });
								h++;
							}
							Array.Resize<string>(ref Arguments, h);
						}
						// preprocess command
						if (Command.ToLowerInvariant() == "with") {
							if (Arguments.Length >= 1) {
								Section = Arguments[0];
								SectionAlwaysPrefix = false;
							} else {
								Section = "";
								SectionAlwaysPrefix = false;
							}
							Command = null;
						} else {
							if (Command.StartsWith(".")) {
								Command = Section + Command;
							} else if (SectionAlwaysPrefix) {
								Command = Section + "." + Command;
							}
							Command = Command.Replace(".Void", "");
							if (Command.StartsWith("structure", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".load", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 5).TrimEnd(new char[] { });
							} else if (Command.StartsWith("texture.background", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".load", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 5).TrimEnd(new char[] { });
							} else if (Command.StartsWith("texture.background", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".x", StringComparison.OrdinalIgnoreCase))
							{
								Command = "texture.background.x" + Command.Substring(18, Command.Length - 20).TrimEnd(new char[] { });
							} else if (Command.StartsWith("texture.background", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".aspect", StringComparison.OrdinalIgnoreCase))
							{
								Command = "texture.background.aspect" + Command.Substring(18, Command.Length - 25).TrimEnd(new char[] { });
							} else if (Command.StartsWith("structure.back", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".x", StringComparison.OrdinalIgnoreCase))
							{
								Command = "texture.background.x" + Command.Substring(14, Command.Length - 16).TrimEnd(new char[] { });
							} else if (Command.StartsWith("structure.back", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".aspect", StringComparison.OrdinalIgnoreCase))
							{
								Command = "texture.background.aspect" + Command.Substring(14, Command.Length - 21).TrimEnd(new char[] { });
							} else if (Command.StartsWith("cycle", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".params", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 7).TrimEnd(new char[] { });
							} else if (Command.StartsWith("signal", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".load", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 5).TrimEnd(new char[] { });
							} else if (Command.StartsWith("train.run", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".set", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 4).TrimEnd(new char[] { });
							} else if (Command.StartsWith("train.flange", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".set", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 4).TrimEnd(new char[] { });
							} else if (Command.StartsWith("train.timetable", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".day.load", StringComparison.OrdinalIgnoreCase)) {
								Command = "train.timetable.day" + Command.Substring(15, Command.Length - 24).Trim(new char[] { });
							} else if (Command.StartsWith("train.timetable", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".night.load", StringComparison.OrdinalIgnoreCase)) {
								Command = "train.timetable.night" + Command.Substring(15, Command.Length - 26).Trim(new char[] { });
							} else if (Command.StartsWith("train.timetable", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".day", StringComparison.OrdinalIgnoreCase)) {
								Command = "train.timetable.day" + Command.Substring(15, Command.Length - 19).Trim(new char[] { });
							} else if (Command.StartsWith("train.timetable", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".night", StringComparison.OrdinalIgnoreCase)) {
								Command = "train.timetable.night" + Command.Substring(15, Command.Length - 21).Trim(new char[] { });
							} else if (Command.StartsWith("route.signal", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".set", StringComparison.OrdinalIgnoreCase))  {
								Command = Command.Substring(0, Command.Length - 4).TrimEnd(new char[] { });
							} else if (Command.StartsWith("route.runinterval", StringComparison.OrdinalIgnoreCase)) {
								Command = "train.interval" + Command.Substring(14, Command.Length - 14);
							} else if (Command.StartsWith("train.gauge", StringComparison.OrdinalIgnoreCase)) {
								Command = "route.gauge" + Command.Substring(11, Command.Length - 11);
							} else if (Command.StartsWith("texture.", StringComparison.OrdinalIgnoreCase)) {
								Command = "structure." + Command.Substring(8, Command.Length - 8);
							}
						}
						// handle indices
						int CommandIndex1 = 0, CommandIndex2 = 0;
						if (Command != null && Command.EndsWith(")")) {
							for (int k = Command.Length - 2; k >= 0; k--) {
								if (Command[k] == '(')
								{
									string Indices = Command.Substring(k + 1, Command.Length - k - 2).TrimStart(new char[] { });
									Command = Command.Substring(0, k).TrimEnd(new char[] { });
									int h = Indices.IndexOf(";", StringComparison.Ordinal);
									if (h >= 0)
									{
										string a = Indices.Substring(0, h).TrimEnd(new char[] { });
										string b = Indices.Substring(h + 1).TrimStart(new char[] { });
										if (a.Length > 0 && !NumberFormats.TryParseIntVb6(a, out CommandIndex1)) {
											Program.CurrentHost.AddMessage(MessageType.Error, false, "Invalid first index appeared at line " + Expressions[j].Line.ToString(Culture) + ", column " + Expressions[j].Column.ToString(Culture) + " in file " + Expressions[j].File + ".");
											Command = null;
										} 
										if (b.Length > 0 && !NumberFormats.TryParseIntVb6(b, out CommandIndex2)) {
											Program.CurrentHost.AddMessage(MessageType.Error, false, "Invalid second index appeared at line " + Expressions[j].Line.ToString(Culture) + ", column " + Expressions[j].Column.ToString(Culture) + " in file " + Expressions[j].File + ".");
											Command = null;
										}
									} else {
										if (Indices.Length > 0 && !NumberFormats.TryParseIntVb6(Indices, out CommandIndex1)) {
											Program.CurrentHost.AddMessage(MessageType.Error, false, "Invalid index appeared at line " + Expressions[j].Line.ToString(Culture) + ", column " + Expressions[j].Column.ToString(Culture) + " in file " + Expressions[j].File + ".");
											Command = null;
										}
									}
									break;
								}
							}
						}
						// process command
						if (!string.IsNullOrEmpty(Command))
						{
							int period = Command.IndexOf('.');
							string nameSpace = string.Empty;
							if (period != -1)
							{
								nameSpace = Command.Substring(0, period).ToLowerInvariant();
								Command = Command.Substring(period + 1).ToLowerInvariant();
							}
							
							switch (nameSpace)
							{
								case "options":
									ParseOptionCommand(Command, Arguments, UnitOfLength, Expressions[j], ref Data, PreviewOnly);
									break;
								case "route":
									ParseRouteCommand(Command, Arguments, CommandIndex1, FileName, UnitOfLength, Expressions[j], ref Data, PreviewOnly);
									break;
								case "train":
									ParseTrainCommand(Command, Arguments, CommandIndex1, UnitOfLength, Expressions[j], ref Data, PreviewOnly);
									break;
								case "structure":
									ParseStructureCommand(Command, Arguments, CommandIndex1, CommandIndex2, Encoding, UnitOfLength, Expressions[j], ref Data, PreviewOnly);
									break;
								case "":
									ParseSignalCommand(Command, Arguments, CommandIndex1, Encoding, Expressions[j], ref Data, PreviewOnly);
									break;
								case "cycle":
									ParseCycleCommand(Command, Arguments, CommandIndex1, Expressions[j], ref Data, PreviewOnly);
									break;
								case "track":
									break;
								default:
									/*
									 * This needs an unrecognised command at some stage
									 */
									break;
							}
						}
					}
				}
			}
			
			// process track namespace
			for (int j = 0; j < Expressions.Length; j++) {
				Program.CurrentProgress = 0.3333 + (double)j * progressFactor;
				if ((j & 255) == 0) {
					System.Threading.Thread.Sleep(1);
					if (Program.Cancel) return;
				}
				if (Data.LineEndingFix)
				{
					if (Expressions[j].Text.EndsWith("_"))
					{
						Expressions[j].Text = Expressions[j].Text.Substring(0, Expressions[j].Text.Length - 1).Trim(new char[] { });
					}
				}
				if (Expressions[j].Text.StartsWith("[") & Expressions[j].Text.EndsWith("]")) {
					Section = Expressions[j].Text.Substring(1, Expressions[j].Text.Length - 2).Trim(new char[] { });
					if (string.Compare(Section, "object", StringComparison.OrdinalIgnoreCase) == 0) {
						Section = "Structure";
					} else if (string.Compare(Section, "railway", StringComparison.OrdinalIgnoreCase) == 0) {
						Section = "Track";
					}
					SectionAlwaysPrefix = true;
				} else {
					Expressions[j].ConvertRwToCsv(Section, SectionAlwaysPrefix);
					// separate command and arguments
					string Command, ArgumentSequence;
					Expressions[j].SeparateCommandsAndArguments(out Command, out ArgumentSequence, Culture, false, IsRW, Section);
					// process command
					double Number;
					bool NumberCheck = !IsRW || string.Compare(Section, "track", StringComparison.OrdinalIgnoreCase) == 0;
					if (NumberCheck && NumberFormats.TryParseDouble(Command, UnitOfLength, out Number)) {
						// track position
						if (ArgumentSequence.Length != 0) {
							Program.CurrentHost.AddMessage(MessageType.Error, false, "A track position must not contain any arguments at line " + Expressions[j].Line.ToString(Culture) + ", column " + Expressions[j].Column.ToString(Culture) + " in file " + Expressions[j].File);
						} else if (Number < 0.0) {
							Program.CurrentHost.AddMessage(MessageType.Error, false, "Negative track position encountered at line " + Expressions[j].Line.ToString(Culture) + ", column " + Expressions[j].Column.ToString(Culture) + " in file " + Expressions[j].File);
						} else {
							if (Program.CurrentOptions.EnableBveTsHacks && IsRW && Number == 4535545100)
							{
								//WMATA Red line has an erroneous track position causing an out of memory cascade
								Number = 45355;
							}
							Data.TrackPosition = Number;
							BlockIndex = (int)Math.Floor(Number / Data.BlockInterval + 0.001);
							if (Data.FirstUsedBlock == -1) Data.FirstUsedBlock = BlockIndex;
							Data.CreateMissingBlocks(ref BlocksUsed, BlockIndex, PreviewOnly);
						}
					} else {
						// split arguments
						string[] Arguments;
						{
							int n = 0;
							for (int k = 0; k < ArgumentSequence.Length; k++) {
								if (IsRW & ArgumentSequence[k] == ',') {
									n++;
								} else if (ArgumentSequence[k] == ';') {
									n++;
								}
							}
							Arguments = new string[n + 1];
							int a = 0, h = 0;
							for (int k = 0; k < ArgumentSequence.Length; k++) {
								if (IsRW & ArgumentSequence[k] == ',') {
									Arguments[h] = ArgumentSequence.Substring(a, k - a).Trim(new char[] { });
									a = k + 1; h++;
								} else if (ArgumentSequence[k] == ';') {
									Arguments[h] = ArgumentSequence.Substring(a, k - a).Trim(new char[] { });
									a = k + 1; h++;
								}
							}
							if (ArgumentSequence.Length - a > 0) {
								Arguments[h] = ArgumentSequence.Substring(a).Trim(new char[] { });
								h++;
							}
							Array.Resize<string>(ref Arguments, h);
						}
						// preprocess command
						if (Command.ToLowerInvariant() == "with") {
							if (Arguments.Length >= 1) {
								Section = Arguments[0];
								SectionAlwaysPrefix = false;
							} else {
								Section = "";
								SectionAlwaysPrefix = false;
							}
							Command = null;
						} else {
							if (Command.StartsWith(".")) {
								Command = Section + Command;
							} else if (SectionAlwaysPrefix) {
								Command = Section + "." + Command;
							}
							Command = Command.Replace(".Void", "");
							if (Command.StartsWith("structure", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".load", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 5).TrimEnd(new char[] { });
							} else if (Command.StartsWith("texture.background", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".load", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 5).TrimEnd(new char[] { });
							} else if (Command.StartsWith("texture.background", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".x", StringComparison.OrdinalIgnoreCase))
							{
								Command = "texture.background.x" + Command.Substring(18, Command.Length - 20).TrimEnd(new char[] { });
							} else if (Command.StartsWith("texture.background", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".aspect", StringComparison.OrdinalIgnoreCase))
							{
								Command = "texture.background.aspect" + Command.Substring(18, Command.Length - 25).TrimEnd(new char[] { });
							} else if (Command.StartsWith("structure.back", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".x", StringComparison.OrdinalIgnoreCase))
							{
								Command = "texture.background.x" + Command.Substring(14, Command.Length - 16).TrimEnd(new char[] { });
							} else if (Command.StartsWith("structure.back", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".aspect", StringComparison.OrdinalIgnoreCase))
							{
								Command = "texture.background.aspect" + Command.Substring(14, Command.Length - 21).TrimEnd(new char[] { });
							} else if (Command.StartsWith("cycle", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".params", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 7).TrimEnd(new char[] { });
							} else if (Command.StartsWith("signal", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".load", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 5).TrimEnd(new char[] { });
							} else if (Command.StartsWith("train.run", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".set", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 4).TrimEnd(new char[] { });
							} else if (Command.StartsWith("train.flange", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".set", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 4).TrimEnd(new char[] { });
							} else if (Command.StartsWith("train.timetable", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".day.load", StringComparison.OrdinalIgnoreCase)) {
								Command = "train.timetable.day" + Command.Substring(15, Command.Length - 24).Trim(new char[] { });
							} else if (Command.StartsWith("train.timetable", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".night.load", StringComparison.OrdinalIgnoreCase)) {
								Command = "train.timetable.night" + Command.Substring(15, Command.Length - 26).Trim(new char[] { });
							} else if (Command.StartsWith("train.timetable", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".day", StringComparison.OrdinalIgnoreCase)) {
								Command = "train.timetable.day" + Command.Substring(15, Command.Length - 19).Trim(new char[] { });
							} else if (Command.StartsWith("train.timetable", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".night", StringComparison.OrdinalIgnoreCase)) {
								Command = "train.timetable.night" + Command.Substring(15, Command.Length - 21).Trim(new char[] { });
							} else if (Command.StartsWith("route.signal", StringComparison.OrdinalIgnoreCase) & Command.EndsWith(".set", StringComparison.OrdinalIgnoreCase))
							{
								Command = Command.Substring(0, Command.Length - 4).TrimEnd(new char[] { });
							}
						}
						// handle indices
						int CommandIndex1, CommandIndex2;
						if (Command != null && Command.EndsWith(")")) {
							for (int k = Command.Length - 2; k >= 0; k--) {
								if (Command[k] == '(')
								{
									string Indices = Command.Substring(k + 1, Command.Length - k - 2).TrimStart(new char[] { });
									Command = Command.Substring(0, k).TrimEnd(new char[] { });
									int h = Indices.IndexOf(";", StringComparison.Ordinal);
									if (h >= 0)
									{
										string a = Indices.Substring(0, h).TrimEnd(new char[] { });
										string b = Indices.Substring(h + 1).TrimStart(new char[] { });
										if (a.Length > 0 && !NumberFormats.TryParseIntVb6(a, out CommandIndex1)) {
											Program.CurrentHost.AddMessage(MessageType.Error, false, "Invalid first index appeared at line " + Expressions[j].Line.ToString(Culture) + ", column " + Expressions[j].Column.ToString(Culture) + " in file " + Expressions[j].File + ".");
											Command = null;
										} 
										if (b.Length > 0 && !NumberFormats.TryParseIntVb6(b, out CommandIndex2)) {
											Program.CurrentHost.AddMessage(MessageType.Error, false, "Invalid second index appeared at line " + Expressions[j].Line.ToString(Culture) + ", column " + Expressions[j].Column.ToString(Culture) + " in file " + Expressions[j].File + ".");
											Command = null;
										}
									} else {
										if (Indices.Length > 0 && !NumberFormats.TryParseIntVb6(Indices, out CommandIndex1)) {
											Program.CurrentHost.AddMessage(MessageType.Error, false, "Invalid index appeared at line " + Expressions[j].Line.ToString(Culture) + ", column " + Expressions[j].Column.ToString(Culture) + " in file " + Expressions[j].File + ".");
											Command = null;
										}
									}
									break;
								}
							}
						}
						// process command
						if (!string.IsNullOrEmpty(Command)) {
							int period = Command.IndexOf('.');
							string nameSpace = string.Empty;
							if (period != -1)
							{
								nameSpace = Command.Substring(0, period).ToLowerInvariant();
								Command = Command.Substring(period + 1).ToLowerInvariant();
							}

							if (nameSpace == "track")
							{
								ParseTrackCommand(Command, Arguments, FileName, UnitOfLength, Expressions[j], ref Data, BlockIndex, CurrentStation, CurrentStop, DepartureSignalUsed, CurrentSection, PreviewOnly);
							}
							else
							{
								Program.CurrentHost.AddMessage(MessageType.Warning, false, "The command " + Command + " is not supported at line " + Expressions[j].Line.ToString(Culture) + ", column " + Expressions[j].Column.ToString(Culture) + " in file " + Expressions[j].File);
							}
						}
					}
				}
			}
			// blocks
			Array.Resize<Block>(ref Data.Blocks, BlocksUsed);
		}
	}
}