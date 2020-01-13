using OpenBveApi.Interface;
using OpenBveApi.Objects;

namespace OpenBve
{
	/// <summary>The TrainManager is the root class containing functions to load and manage trains within the simulation world.</summary>
	public static partial class TrainManager
	{
		internal class TouchElement
		{
			internal AnimatedObject Element;
			internal int JumpScreenIndex;
			internal int SoundIndex;
			internal Translations.Command Command;
			internal int CommandOption;
		}

		internal class ElementsGroup
		{
			internal AnimatedObject[] Elements;
			internal bool Overlay;
			internal TouchElement[] TouchElements;

			internal void Initialize(bool CurrentlyVisible)
			{
				for (int i = 0; i < Elements.Length; i++)
				{
					for (int j = 0; j < Elements[i].States.Length; j++)
					{
						Elements[i].Initialize(j, Overlay, CurrentlyVisible);
					}
				}
			}
		}

		/// <summary>An animated object attached to a car (Exterior, cab etc.)</summary>
		internal class CarSection
		{
			internal ElementsGroup[] Groups;
			internal int CurrentAdditionalGroup;

			/// <summary>Creates a new empty overlay car section</summary>
			internal CarSection()
			{
				Groups = new ElementsGroup[1];
				Groups[0] = new ElementsGroup
				{
					Elements = new AnimatedObject[] { },
					Overlay = true
				};
			}

			/// <summary>Creates a new car section using the specified object</summary>
			/// <param name="currentObject">The object to use</param>
			internal CarSection(UnifiedObject currentObject)
			{
				Groups = new ElementsGroup[1];
				Groups[0] = new ElementsGroup();
				if (currentObject is StaticObject)
				{
					StaticObject s = (StaticObject)currentObject;
					Groups[0].Elements = new AnimatedObject[1];
					Groups[0].Elements[0] = new AnimatedObject(Program.CurrentHost);
					Groups[0].Elements[0].States = new[] { new ObjectState() };
					Groups[0].Elements[0].States[0].Prototype = s;
					Groups[0].Elements[0].CurrentState = 0;
					Program.CurrentHost.CreateDynamicObject(ref Groups[0].Elements[0].internalObject);
				}
				else if (currentObject is AnimatedObjectCollection)
				{
					AnimatedObjectCollection a = (AnimatedObjectCollection)currentObject;
					Groups[0].Elements = new AnimatedObject[a.Objects.Length];
					for (int h = 0; h < a.Objects.Length; h++)
					{
						Groups[0].Elements[h] = a.Objects[h].Clone();
						Program.CurrentHost.CreateDynamicObject(ref Groups[0].Elements[h].internalObject);
					}
				}
			}
			
			internal void Initialize(bool CurrentlyVisible)
			{
				for (int i = 0; i < Groups.Length; i++)
				{
					Groups[i].Initialize(CurrentlyVisible);
				}
			}

			internal void Show()
			{
				if (Groups.Length > 0)
				{
					for (int i = 0; i < Groups[0].Elements.Length; i++)
					{
						if (Groups[0].Overlay)
						{
							Program.CurrentHost.ShowObject(Groups[0].Elements[i].internalObject, ObjectType.Overlay);
						}
						else
						{
							Program.CurrentHost.ShowObject(Groups[0].Elements[i].internalObject, ObjectType.Dynamic);
						}
					}
				}

				int add = CurrentAdditionalGroup + 1;
				if (add < Groups.Length)
				{
					for (int i = 0; i < Groups[add].Elements.Length; i++)
					{
						if (Groups[add].Overlay)
						{
							Program.CurrentHost.ShowObject(Groups[add].Elements[i].internalObject, ObjectType.Overlay);
						}
						else
						{
							Program.CurrentHost.ShowObject(Groups[add].Elements[i].internalObject, ObjectType.Dynamic);
						}
					}
				}
			}
		}

		internal enum CarSectionType
		{
			NotVisible = -1,
			Interior = 0,
			Exterior = 1
		}

	}
}
