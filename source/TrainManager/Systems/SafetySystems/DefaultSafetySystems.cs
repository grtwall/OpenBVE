using System;

namespace TrainManager.Systems
{
	/// <summary>The default safety-systems supported by the inbuilt plugin</summary>
	/// <remarks>These are the default Japanese safety systems as supported by BVE1 ==> 4</remarks>
	[Flags]
	public enum DefaultSafetySystems
	{
		/// <summary>No safety systems</summary>
		None = 0,
		/// <summary>ATS-SN</summary>
		AtsSn = 1,
		/// <summary>AST-P</summary>
		AtsP = 2,
		/// <summary>ATC</summary>
		Atc = 4,
		/// <summary>EB</summary>
		Eb = 8,
		/// <summary>The safety systems are controlled by a runtime plugin</summary>
		Plugin = 16

	}
}
