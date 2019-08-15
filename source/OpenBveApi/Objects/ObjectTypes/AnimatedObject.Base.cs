using System.IO;
using CSScriptLibrary;
using OpenBveApi.FunctionScripting;
using OpenBveApi.Graphics;
using OpenBveApi.Hosts;
using OpenBveApi.Interface;
using OpenBveApi.Math;
using OpenBveApi.Trains;

namespace OpenBveApi.Objects
{
	/// <summary>The base type for an animated object</summary>
	public abstract class AnimatedObjectBase
	{
		/// <summary>The array of states</summary>
		public AnimatedObjectState[] States;
		/// <summary>The function script controlling state changes</summary>
		public FunctionScript StateFunction;
		/// <summary>The index of the current state</summary>
		public int CurrentState;
		/// <summary>A 3D vector describing the direction taken when TranslateXFunction is called</summary>
		public Vector3 TranslateXDirection;
		/// <summary>A 3D vector describing the direction taken when TranslateXYFunction is called</summary>
		public Vector3 TranslateYDirection;
		/// <summary>A 3D vector describing the direction taken when TranslateZFunction is called</summary>
		public Vector3 TranslateZDirection;
		/// <summary>The function script controlling translation in the X direction</summary>
		/// <remarks>X is an arbitrary 3D direction, nominally 1,0,0</remarks>
		public FunctionScript TranslateXFunction;
		/// <summary>The function script controlling translation in the Y direction</summary>
		/// <remarks>Y is an arbitrary 3D direction, nominally 0,1,0</remarks>
		public FunctionScript TranslateYFunction;
		/// <summary>The function script controlling translation in the Z direction</summary>
		/// <remarks>Z is an arbitrary 3D direction, nominally 0,0,1</remarks>
		public FunctionScript TranslateZFunction;
		/// <summary>A 3D vector describing the rotation performed when RotateXFunction is called</summary>
		public Vector3 RotateXDirection;
		/// <summary>A 3D vector describing the rotation performed when RotateYFunction is called</summary>
		public Vector3 RotateYDirection;
		/// <summary>A 3D vector describing the rotation performed when RotateZFunction is called</summary>
		public Vector3 RotateZDirection;
		/// <summary>The function script controlling translation in the X direction</summary>
		/// <remarks>X is an arbitrary 3D direction, nominally 1,0,0</remarks>
		public FunctionScript RotateXFunction;
		/// <summary>The function script controlling translation in the Y direction</summary>
		/// <remarks>Y is an arbitrary 3D direction, nominally 0,1,0</remarks>
		public FunctionScript RotateYFunction;
		/// <summary>The function script controlling translation in the Z direction</summary>
		/// <remarks>Z is an arbitrary 3D direction, nominally 0,0,1</remarks>
		public FunctionScript RotateZFunction;
		/// <summary>The damping (if any) to perform when RotateXFunction is called</summary>
		public Damping RotateXDamping;
		/// <summary>The damping (if any) to perform when RotateYFunction is called</summary>
		public Damping RotateYDamping;
		/// <summary>The damping (if any) to perform when RotateZFunction is called</summary>
		public Damping RotateZDamping;
		/// <summary>A 2D vector describing the texture shifting performed when TextureShiftXFunction is called</summary>
		public Vector2 TextureShiftXDirection;
		/// <summary>A 2D vector describing the texture shifting performed when TextureShiftYFunction is called</summary>
		public Vector2 TextureShiftYDirection;
		/// <summary>The function script controlling texture shifting in the X direction</summary>
		/// <remarks>X is an arbitrary 2D direction, nominally 1,0</remarks>
		public FunctionScript TextureShiftXFunction;
		/// <summary>The function script controlling texture shifting in the Y direction</summary>
		/// <remarks>X is an arbitrary 2D direction, nominally 0,1</remarks>
		public FunctionScript TextureShiftYFunction;
		/// <summary>If the LED function is used, this controls whether the winding is clockwise or anti-clockwise</summary>
		public bool LEDClockwiseWinding;
		/// <summary>The initial angle of the LED function</summary>
		public double LEDInitialAngle;
		/// <summary>The final angle of the LED function</summary>
		public double LEDLastAngle;
		/// <summary>If LEDFunction is used, an array of five vectors representing the bottom-left, up-left, up-right, bottom-right and center coordinates of the LED square, or a null reference otherwise.</summary>
		public Vector3[] LEDVectors;
		/// <summary>The function script controlling the LED square</summary>
		public FunctionScript LEDFunction;
		/// <summary>The refresh rate in seconds</summary>
		public double RefreshRate;
		/// <summary>The time since the last update of this object</summary>
		public double SecondsSinceLastUpdate;

		//This section holds script files executed by CS-Script
		/// <summary>The absolute path to the script file to be evaluated when TranslateXScript is called</summary>
		public string TranslateXScriptFile;
		/// <summary>The actual script interface</summary>
		public AnimationScript TranslateXAnimationScript;
		/// <summary>The absolute path to the script file to be evaluated when TranslateYScript is called</summary>
		public AnimationScript TranslateYAnimationScript;
		/// <summary>The actual script interface</summary>
		public string TranslateYScriptFile;
		/// <summary>The absolute path to the script file to be evaluated when TranslateZScript is called</summary>
		public AnimationScript TranslateZAnimationScript;
		/// <summary>The actual script interface</summary>
		public string TranslateZScriptFile;
		/// <summary>The function script controlling movement along the track</summary>
		public FunctionScript TrackFollowerFunction;
		/// <summary>The front axle position if TrackFollowerFunction is used</summary>
		public double FrontAxlePosition = 1;
		/// <summary>The rear axle position if TrackFollowerFunction is used</summary>
		public double RearAxlePosition = -1;
		/// <summary>Holds a reference to the internal static object used for display</summary>
		/// <remarks>This is a fully transformed deep copy of the current state</remarks>
		public StaticObject internalObject;
		/// <summary>Holds a reference to the host interface of the current application</summary>
		public Hosts.HostInterface currentHost;

		/// <summary>Checks whether this object contains any functions</summary>
		public bool IsFreeOfFunctions()
		{
			if (this.StateFunction != null) return false;
			if (this.TrackFollowerFunction != null) return false;
			if (this.TranslateXFunction != null | this.TranslateYFunction != null | this.TranslateZFunction != null) return false;
			if (this.RotateXFunction != null | this.RotateYFunction != null | this.RotateZFunction != null) return false;
			if (this.TextureShiftXFunction != null | this.TextureShiftYFunction != null) return false;
			if (this.LEDFunction != null) return false;
			if (this.TranslateXScriptFile != null | this.TranslateYScriptFile != null | this.TranslateZScriptFile != null) return false;
			if (this.TrackFollowerFunction != null) return false;
			return true;
		}

		/// <summary>Initialises the object</summary>
		/// <param name="StateIndex">The state to show</param>
		/// <param name="Overlay">Whether this object is in overlay mode</param>
		/// <param name="Show">Whether the object should be shown immediately on initialisation</param>
		public void Initialize(int StateIndex, bool Overlay, bool Show)
		{
			currentHost.HideObject(ref internalObject);
			int t = StateIndex;
			if (t >= 0 && States[t].Object != null)
			{
				int m = States[t].Object.Mesh.Vertices.Length;
				internalObject.Mesh.Vertices = new VertexTemplate[m];
				for (int k = 0; k < m; k++)
				{
					if (States[t].Object.Mesh.Vertices[k] is ColoredVertex)
					{
						internalObject.Mesh.Vertices[k] = new ColoredVertex((ColoredVertex) States[t].Object.Mesh.Vertices[k]);
					}
					else
					{
						internalObject.Mesh.Vertices[k] = new Vertex((Vertex) States[t].Object.Mesh.Vertices[k]);
					}

				}

				m = States[t].Object.Mesh.Faces.Length;
				internalObject.Mesh.Faces = new MeshFace[m];
				for (int k = 0; k < m; k++)
				{
					internalObject.Mesh.Faces[k].Flags = States[t].Object.Mesh.Faces[k].Flags;
					internalObject.Mesh.Faces[k].Material = States[t].Object.Mesh.Faces[k].Material;
					int o = States[t].Object.Mesh.Faces[k].Vertices.Length;
					internalObject.Mesh.Faces[k].Vertices = new MeshFaceVertex[o];
					for (int h = 0; h < o; h++)
					{
						internalObject.Mesh.Faces[k].Vertices[h] = States[t].Object.Mesh.Faces[k].Vertices[h];
					}
				}

				internalObject.Mesh.Materials = States[t].Object.Mesh.Materials;
			}
			else
			{
				/* Must internally reset the object, not create a new one.
				 * This allows the reference to keep pointing to the same place
				 */
				internalObject.Mesh = new Mesh
				{
					Faces = new MeshFace[] { },
					Materials = new MeshMaterial[] { },
					Vertices = new VertexTemplate[] { }
				};

			}

			CurrentState = StateIndex;
			if (Show)
			{
				if (Overlay)
				{
					currentHost.ShowObject(internalObject, ObjectType.Overlay);
				}
				else
				{
					currentHost.ShowObject(internalObject, ObjectType.Dynamic);
				}
			}
		}

		/// <summary> Updates the position and state of the animated object</summary>
		/// <param name="IsPartOfTrain">Whether this object forms part of a train</param>
		/// <param name="Train">The train, or a null reference otherwise</param>
		/// <param name="CarIndex">If this object forms part of a train, the car index it refers to</param>
		/// <param name="SectionIndex">If this object has been placed via Track.Sig, the index of the section it is attached to</param>
		/// <param name="TrackPosition"></param>
		/// <param name="Position"></param>
		/// <param name="Direction"></param>
		/// <param name="Up"></param>
		/// <param name="Side"></param>
		/// <param name="UpdateFunctions">Whether the functions associated with this object should be re-evaluated</param>
		/// <param name="Show"></param>
		/// <param name="TimeElapsed">The time elapsed since this object was last updated</param>
		/// <param name="EnableDamping">Whether damping is to be applied for this call</param>
		/// <param name="IsTouch">Whether Animated Object belonging to TouchElement class.</param>
		/// <param name="Camera"></param>
		public void Update(bool IsPartOfTrain, AbstractTrain Train, int CarIndex, int SectionIndex, double TrackPosition, Vector3 Position, Vector3 Direction, Vector3 Up, Vector3 Side, bool UpdateFunctions, bool Show, double TimeElapsed, bool EnableDamping, bool IsTouch = false, dynamic Camera = null)
		{
			int s = CurrentState;
			// state change
			if (StateFunction != null & UpdateFunctions)
			{
				double sd = StateFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
				int si = (int) System.Math.Round(sd);
				int sn = States.Length;
				if (si < 0 | si >= sn) si = -1;
				if (s != si)
				{
					Initialize(si, Camera != null, Show);
					s = si;
				}
			}

			if (s == -1) return;
			// translation
			if (TranslateXFunction != null)
			{
				double x;
				if (UpdateFunctions)
				{
					x = TranslateXFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
				}
				else
				{
					x = TranslateXFunction.LastResult;
				}

				Vector3 translationVector = new Vector3(TranslateXDirection); //Must clone
				translationVector.Rotate(Direction, Up, Side);
				translationVector *= x;
				Position += translationVector;
			}
			else if (TranslateXScriptFile != null)
			{
				//Translate X Script
				if (TranslateXAnimationScript == null)
				{
					//Load the script if required
					try
					{
						CSScript.GlobalSettings.TargetFramework = "v4.0";
						TranslateXAnimationScript = CSScript.LoadCode(File.ReadAllText(TranslateXScriptFile))
							.CreateObject("OpenBVEScript")
							.AlignToInterface<AnimationScript>(true);
					}
					catch
					{
						currentHost.AddMessage(MessageType.Error, false,
							"An error occcured whilst parsing script " + TranslateXScriptFile);
						TranslateXScriptFile = null;
						return;
					}
				}

				double x = TranslateXAnimationScript.ExecuteScript(Train, Position, TrackPosition, SectionIndex,
					IsPartOfTrain, TimeElapsed);
				Vector3 translationVector = new Vector3(TranslateXDirection); //Must clone
				translationVector.Rotate(Direction, Up, Side);
				translationVector *= x;
				Position += translationVector;
			}


			if (TranslateYFunction != null)
			{
				double y;
				if (UpdateFunctions)
				{
					y = TranslateYFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
				}
				else
				{
					y = TranslateYFunction.LastResult;
				}

				Vector3 translationVector = new Vector3(TranslateYDirection); //Must clone
				translationVector.Rotate(Direction, Up, Side);
				translationVector *= y;
				Position += translationVector;
			}
			else if (TranslateYScriptFile != null)
			{
				//Translate X Script
				if (TranslateYAnimationScript == null)
				{
					//Load the script if required
					try
					{
						CSScript.GlobalSettings.TargetFramework = "v4.0";
						TranslateYAnimationScript = CSScript.LoadCode(File.ReadAllText(TranslateYScriptFile))
							.CreateObject("OpenBVEScript")
							.AlignToInterface<AnimationScript>(true);
					}
					catch
					{
						currentHost.AddMessage(MessageType.Error, false,
							"An error occcured whilst parsing script " + TranslateYScriptFile);
						TranslateYScriptFile = null;
						return;
					}
				}

				double y = TranslateYAnimationScript.ExecuteScript(Train, Position, TrackPosition, SectionIndex,
					IsPartOfTrain, TimeElapsed);
				Vector3 translationVector = new Vector3(TranslateYDirection); //Must clone
				translationVector.Rotate(Direction, Up, Side);
				translationVector *= y;
				Position += translationVector;
			}

			if (TranslateZFunction != null)
			{
				double z;
				if (UpdateFunctions)
				{
					z = TranslateZFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
				}
				else
				{
					z = TranslateZFunction.LastResult;
				}

				Vector3 translationVector = new Vector3(TranslateZDirection); //Must clone
				translationVector.Rotate(Direction, Up, Side);
				translationVector *= z;
				Position += translationVector;
			}
			else if (TranslateZScriptFile != null)
			{
				//Translate X Script
				if (TranslateZAnimationScript == null)
				{
					//Load the script if required
					try
					{
						CSScript.GlobalSettings.TargetFramework = "v4.0";
						TranslateZAnimationScript = CSScript.LoadCode(File.ReadAllText(TranslateZScriptFile))
							.CreateObject("OpenBVEScript")
							.AlignToInterface<AnimationScript>(true);
					}
					catch
					{
						currentHost.AddMessage(MessageType.Error, false,
							"An error occcured whilst parsing script " + TranslateZScriptFile);
						TranslateZScriptFile = null;
						return;
					}
				}

				double z = TranslateZAnimationScript.ExecuteScript(Train, Position, TrackPosition, SectionIndex,
					IsPartOfTrain, TimeElapsed);
				Vector3 translationVector = new Vector3(TranslateZDirection); //Must clone
				translationVector.Rotate(Direction, Up, Side);
				translationVector *= z;
				Position += translationVector;
			}

			// rotation
			bool rotateX = RotateXFunction != null;
			bool rotateY = RotateYFunction != null;
			bool rotateZ = RotateZFunction != null;
			double cosX, sinX;
			if (rotateX)
			{
				double a;
				if (UpdateFunctions)
				{
					a = RotateXFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
				}
				else
				{
					a = RotateXFunction.LastResult;
				}

				if (RotateXDamping != null)
				{
					RotateXDamping.Update(TimeElapsed, ref a, EnableDamping);
				}

				cosX = System.Math.Cos(a);
				sinX = System.Math.Sin(a);
			}
			else
			{
				cosX = 0.0;
				sinX = 0.0;
			}

			double cosY, sinY;
			if (rotateY)
			{
				double a;
				if (UpdateFunctions)
				{
					a = RotateYFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
				}
				else
				{
					a = RotateYFunction.LastResult;
				}

				if (RotateYDamping != null)
				{
					RotateYDamping.Update(TimeElapsed, ref a, EnableDamping);
				}

				cosY = System.Math.Cos(a);
				sinY = System.Math.Sin(a);
			}
			else
			{
				cosY = 0.0;
				sinY = 0.0;
			}

			double cosZ, sinZ;
			if (rotateZ)
			{
				double a;
				if (UpdateFunctions)
				{
					a = RotateZFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
				}
				else
				{
					a = RotateZFunction.LastResult;
				}

				if (RotateZDamping != null)
				{
					RotateZDamping.Update(TimeElapsed, ref a, EnableDamping);
				}

				cosZ = System.Math.Cos(a);
				sinZ = System.Math.Sin(a);
			}
			else
			{
				cosZ = 0.0;
				sinZ = 0.0;
			}

			// texture shift
			bool shiftx = TextureShiftXFunction != null;
			bool shifty = TextureShiftYFunction != null;
			if ((shiftx | shifty) & UpdateFunctions)
			{
				for (int k = 0; k < internalObject.Mesh.Vertices.Length; k++)
				{
					internalObject.Mesh.Vertices[k].TextureCoordinates = States[s].Object.Mesh.Vertices[k].TextureCoordinates;
				}

				if (shiftx)
				{
					double x = TextureShiftXFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
					x -= System.Math.Floor(x);
					for (int k = 0; k < internalObject.Mesh.Vertices.Length; k++)
					{
						internalObject.Mesh.Vertices[k].TextureCoordinates.X += (float) (x * TextureShiftXDirection.X);
						internalObject.Mesh.Vertices[k].TextureCoordinates.Y += (float) (x * TextureShiftXDirection.Y);
					}
				}

				if (shifty)
				{
					double y = TextureShiftYFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
					y -= System.Math.Floor(y);
					for (int k = 0; k < internalObject.Mesh.Vertices.Length; k++)
					{
						internalObject.Mesh.Vertices[k].TextureCoordinates.X += (float) (y * TextureShiftYDirection.X);
						internalObject.Mesh.Vertices[k].TextureCoordinates.Y += (float) (y * TextureShiftYDirection.Y);
					}
				}
			}

			// led
			bool led = LEDFunction != null;
			double ledangle;
			if (led)
			{
				if (UpdateFunctions)
				{
					ledangle = LEDFunction.Perform(Train, CarIndex, Position, TrackPosition, SectionIndex, IsPartOfTrain, TimeElapsed, CurrentState);
				}
				else
				{
					ledangle = LEDFunction.LastResult;
				}
			}
			else
			{
				ledangle = 0.0;
			}

			// null object
			if (States[s].Object == null)
			{
				return;
			}

			// initialize vertices
			for (int k = 0; k < States[s].Object.Mesh.Vertices.Length; k++)
			{
				internalObject.Mesh.Vertices[k].Coordinates = States[s].Object.Mesh.Vertices[k].Coordinates;
			}

			// led
			if (led)
			{
				/*
				 * Edges:         Vertices:
				 * 0 - bottom     0 - bottom-left
				 * 1 - left       1 - top-left
				 * 2 - top        2 - top-right
				 * 3 - right      3 - bottom-right
				 *                4 - center
				 * */
				int v = 1;
				if (LEDClockwiseWinding)
				{
					/* winding is clockwise*/
					if (ledangle < LEDInitialAngle)
					{
						ledangle = LEDInitialAngle;
					}

					if (ledangle < LEDLastAngle)
					{
						double currentEdgeFloat = System.Math.Floor(0.636619772367582 * (ledangle + 0.785398163397449));
						int currentEdge = ((int) currentEdgeFloat % 4 + 4) % 4;
						double lastEdgeFloat = System.Math.Floor(0.636619772367582 * (LEDLastAngle + 0.785398163397449));
						int lastEdge = ((int) lastEdgeFloat % 4 + 4) % 4;
						if (lastEdge < currentEdge | lastEdge == currentEdge & System.Math.Abs(currentEdgeFloat - lastEdgeFloat) > 2.0)
						{
							lastEdge += 4;
						}

						if (currentEdge == lastEdge)
						{
							/* current angle to last angle */
							{
								double t = 0.5 + (0.636619772367582 * ledangle) - currentEdgeFloat;
								if (t < 0.0)
								{
									t = 0.0;
								}
								else if (t > 1.0)
								{
									t = 1.0;
								}

								t = 0.5 * (1.0 - System.Math.Tan(0.25 * (System.Math.PI - 2.0 * System.Math.PI * t)));
								double cx = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].X + t * LEDVectors[currentEdge].X;
								double cy = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].Y + t * LEDVectors[currentEdge].Y;
								double cz = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].Z + t * LEDVectors[currentEdge].Z;
								States[s].Object.Mesh.Vertices[v].Coordinates = new Vector3(cx, cy, cz);
								v++;
							}
							{
								double t = 0.5 + (0.636619772367582 * LEDLastAngle) - lastEdgeFloat;
								if (t < 0.0)
								{
									t = 0.0;
								}
								else if (t > 1.0)
								{
									t = 1.0;
								}

								t = 0.5 * (1.0 - System.Math.Tan(0.25 * (System.Math.PI - 2.0 * System.Math.PI * t)));
								double lx = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].X + t * LEDVectors[lastEdge].X;
								double ly = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].Y + t * LEDVectors[lastEdge].Y;
								double lz = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].Z + t * LEDVectors[lastEdge].Z;
								States[s].Object.Mesh.Vertices[v].Coordinates = new Vector3(lx, ly, lz);
								v++;
							}
						}
						else
						{
							{
								/* current angle to square vertex */
								double t = 0.5 + (0.636619772367582 * ledangle) - currentEdgeFloat;
								if (t < 0.0)
								{
									t = 0.0;
								}
								else if (t > 1.0)
								{
									t = 1.0;
								}

								t = 0.5 * (1.0 - System.Math.Tan(0.25 * (System.Math.PI - 2.0 * System.Math.PI * t)));
								double cx = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].X + t * LEDVectors[currentEdge].X;
								double cy = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].Y + t * LEDVectors[currentEdge].Y;
								double cz = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].Z + t * LEDVectors[currentEdge].Z;
								States[s].Object.Mesh.Vertices[v + 0].Coordinates = new Vector3(cx, cy, cz);
								States[s].Object.Mesh.Vertices[v + 1].Coordinates = LEDVectors[currentEdge];
								v += 2;
							}
							for (int j = currentEdge + 1; j < lastEdge; j++)
							{
								/* square-vertex to square-vertex */
								States[s].Object.Mesh.Vertices[v + 0].Coordinates = LEDVectors[(j + 3) % 4];
								States[s].Object.Mesh.Vertices[v + 1].Coordinates = LEDVectors[j % 4];
								v += 2;
							}

							{
								/* square vertex to last angle */
								double t = 0.5 + (0.636619772367582 * LEDLastAngle) - lastEdgeFloat;
								if (t < 0.0)
								{
									t = 0.0;
								}
								else if (t > 1.0)
								{
									t = 1.0;
								}

								t = 0.5 * (1.0 - System.Math.Tan(0.25 * (System.Math.PI - 2.0 * System.Math.PI * t)));
								double lx = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].X + t * LEDVectors[lastEdge % 4].X;
								double ly = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].Y + t * LEDVectors[lastEdge % 4].Y;
								double lz = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].Z + t * LEDVectors[lastEdge % 4].Z;
								States[s].Object.Mesh.Vertices[v + 0].Coordinates = LEDVectors[(lastEdge + 3) % 4];
								States[s].Object.Mesh.Vertices[v + 1].Coordinates = new Vector3(lx, ly, lz);
								v += 2;
							}
						}
					}
				}
				else
				{
					/* winding is counter-clockwise*/
					if (ledangle > LEDInitialAngle)
					{
						ledangle = LEDInitialAngle;
					}

					if (ledangle > LEDLastAngle)
					{
						double currentEdgeFloat = System.Math.Floor(0.636619772367582 * (ledangle + 0.785398163397449));
						int currentEdge = ((int) currentEdgeFloat % 4 + 4) % 4;
						double lastEdgeFloat = System.Math.Floor(0.636619772367582 * (LEDLastAngle + 0.785398163397449));
						int lastEdge = ((int) lastEdgeFloat % 4 + 4) % 4;
						if (currentEdge < lastEdge | lastEdge == currentEdge & System.Math.Abs(currentEdgeFloat - lastEdgeFloat) > 2.0)
						{
							currentEdge += 4;
						}

						if (currentEdge == lastEdge)
						{
							/* current angle to last angle */
							{
								double t = 0.5 + (0.636619772367582 * LEDLastAngle) - lastEdgeFloat;
								if (t < 0.0)
								{
									t = 0.0;
								}
								else if (t > 1.0)
								{
									t = 1.0;
								}

								t = 0.5 * (1.0 - System.Math.Tan(0.25 * (System.Math.PI - 2.0 * System.Math.PI * t)));
								double lx = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].X + t * LEDVectors[lastEdge].X;
								double ly = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].Y + t * LEDVectors[lastEdge].Y;
								double lz = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].Z + t * LEDVectors[lastEdge].Z;
								States[s].Object.Mesh.Vertices[v].Coordinates = new Vector3(lx, ly, lz);
								v++;
							}
							{
								double t = 0.5 + (0.636619772367582 * ledangle) - currentEdgeFloat;
								if (t < 0.0)
								{
									t = 0.0;
								}
								else if (t > 1.0)
								{
									t = 1.0;
								}

								t = t - System.Math.Floor(t);
								t = 0.5 * (1.0 - System.Math.Tan(0.25 * (System.Math.PI - 2.0 * System.Math.PI * t)));
								double cx = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].X + t * LEDVectors[currentEdge].X;
								double cy = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].Y + t * LEDVectors[currentEdge].Y;
								double cz = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].Z + t * LEDVectors[currentEdge].Z;
								States[s].Object.Mesh.Vertices[v].Coordinates = new Vector3(cx, cy, cz);
								v++;
							}
						}
						else
						{
							{
								/* current angle to square vertex */
								double t = 0.5 + (0.636619772367582 * ledangle) - currentEdgeFloat;
								if (t < 0.0)
								{
									t = 0.0;
								}
								else if (t > 1.0)
								{
									t = 1.0;
								}

								t = 0.5 * (1.0 - System.Math.Tan(0.25 * (System.Math.PI - 2.0 * System.Math.PI * t)));
								double cx = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].X + t * LEDVectors[currentEdge % 4].X;
								double cy = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].Y + t * LEDVectors[currentEdge % 4].Y;
								double cz = (1.0 - t) * LEDVectors[(currentEdge + 3) % 4].Z + t * LEDVectors[currentEdge % 4].Z;
								States[s].Object.Mesh.Vertices[v + 0].Coordinates = LEDVectors[(currentEdge + 3) % 4];
								States[s].Object.Mesh.Vertices[v + 1].Coordinates = new Vector3(cx, cy, cz);
								v += 2;
							}
							for (int j = currentEdge - 1; j > lastEdge; j--)
							{
								/* square-vertex to square-vertex */
								States[s].Object.Mesh.Vertices[v + 0].Coordinates = LEDVectors[(j + 3) % 4];
								States[s].Object.Mesh.Vertices[v + 1].Coordinates = LEDVectors[j % 4];
								v += 2;
							}

							{
								/* square vertex to last angle */
								double t = 0.5 + (0.636619772367582 * LEDLastAngle) - lastEdgeFloat;
								if (t < 0.0)
								{
									t = 0.0;
								}
								else if (t > 1.0)
								{
									t = 1.0;
								}

								t = 0.5 * (1.0 - System.Math.Tan(0.25 * (System.Math.PI - 2.0 * System.Math.PI * t)));
								double lx = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].X + t * LEDVectors[lastEdge].X;
								double ly = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].Y + t * LEDVectors[lastEdge].Y;
								double lz = (1.0 - t) * LEDVectors[(lastEdge + 3) % 4].Z + t * LEDVectors[lastEdge].Z;
								States[s].Object.Mesh.Vertices[v + 0].Coordinates = new Vector3(lx, ly, lz);
								States[s].Object.Mesh.Vertices[v + 1].Coordinates = LEDVectors[lastEdge % 4];
								v += 2;
							}
						}
					}
				}

				for (int j = v; v < 11; v++)
				{
					States[s].Object.Mesh.Vertices[j].Coordinates = LEDVectors[4];
				}
			}

			// update vertices
			for (int k = 0; k < States[s].Object.Mesh.Vertices.Length; k++)
			{
				// rotate
				if (rotateX)
				{
					internalObject.Mesh.Vertices[k].Coordinates.Rotate(RotateXDirection, cosX, sinX);
				}

				if (rotateY)
				{
					internalObject.Mesh.Vertices[k].Coordinates.Rotate(RotateYDirection, cosY, sinY);
				}

				if (rotateZ)
				{
					internalObject.Mesh.Vertices[k].Coordinates.Rotate(RotateZDirection, cosZ, sinZ);
				}

				// translate
				if (Camera != null && Camera.CurrentRestriction != CameraRestrictionMode.NotAvailable)
				{

					internalObject.Mesh.Vertices[k].Coordinates += States[s].Position - Position;
					/*
					 * HACK: No idea why, but when using dynamic here, we MUST cast the parameters to Vector3 as otherwise it breaks....
					 */
					internalObject.Mesh.Vertices[k].Coordinates.Rotate((Vector3) Camera.AbsoluteDirection, (Vector3) Camera.AbsoluteUp, (Vector3) Camera.AbsoluteSide);
					double dx = -System.Math.Tan(Camera.Alignment.Yaw) - Camera.Alignment.Position.X;
					double dy = -System.Math.Tan(Camera.Alignment.Pitch) - Camera.Alignment.Position.Y;
					double dz = -Camera.Alignment.Position.Z;
					internalObject.Mesh.Vertices[k].Coordinates.X += Camera.AbsolutePosition.X + dx * Camera.AbsoluteSide.X + dy * Camera.AbsoluteUp.X + dz * Camera.AbsoluteDirection.X;
					internalObject.Mesh.Vertices[k].Coordinates.Y += Camera.AbsolutePosition.Y + dx * Camera.AbsoluteSide.Y + dy * Camera.AbsoluteUp.Y + dz * Camera.AbsoluteDirection.Y;
					internalObject.Mesh.Vertices[k].Coordinates.Z += Camera.AbsolutePosition.Z + dx * Camera.AbsoluteSide.Z + dy * Camera.AbsoluteUp.Z + dz * Camera.AbsoluteDirection.Z;
				}
				else
				{
					internalObject.Mesh.Vertices[k].Coordinates += States[s].Position;
					internalObject.Mesh.Vertices[k].Coordinates.Rotate(Direction, Up, Side);
					internalObject.Mesh.Vertices[k].Coordinates += Position;
				}
			}

			// update normals
			for (int k = 0; k < States[s].Object.Mesh.Faces.Length; k++)
			{
				for (int h = 0; h < States[s].Object.Mesh.Faces[k].Vertices.Length; h++)
				{
					internalObject.Mesh.Faces[k].Vertices[h].Normal = States[s].Object.Mesh.Faces[k].Vertices[h].Normal;
					if (!Vector3.IsZero(States[s].Object.Mesh.Faces[k].Vertices[h].Normal))
					{
						if (rotateX)
						{
							internalObject.Mesh.Faces[k].Vertices[h].Normal.Rotate(RotateXDirection, cosX, sinX);
						}

						if (rotateY)
						{
							internalObject.Mesh.Faces[k].Vertices[h].Normal.Rotate(RotateYDirection, cosY, sinY);
						}

						if (rotateZ)
						{
							internalObject.Mesh.Faces[k].Vertices[h].Normal.Rotate(RotateZDirection, cosZ, sinZ);
						}

						internalObject.Mesh.Faces[k].Vertices[h].Normal.Rotate(Direction, Up, Side);
					}
				}

				// visibility changed
				// TouchElement is handled by another function.
				if (!IsTouch)
				{
					if (Show)
					{
						if (Camera != null)
						{
							currentHost.ShowObject(internalObject, ObjectType.Overlay);
						}
						else
						{
							currentHost.ShowObject(internalObject, ObjectType.Dynamic);
						}
					}
					else
					{
						currentHost.HideObject(ref internalObject);
					}
				}
			}
		}
	}
}
