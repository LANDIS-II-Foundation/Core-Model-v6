using Landis.PlugIns;

namespace Landis.Test.PlugIns.Admin
{
	public class FooBarPlugIn
		: Landis.PlugIns.PlugIn
	{
		public FooBarPlugIn()
			: base("Foo Bar", new PlugInType("sample"))
		{
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes the plug-in with a data file.
		/// </summary>
		/// <param name="dataFile">
		/// Path to the file with initialization data.
		/// </param>
		/// <param name="modelCore">
		/// The model's core framework.
		/// </param>
		public override void Initialize(string dataFile,
		                                ICore  modelCore)
		{
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Runs the plug-in for the current timestep.
		/// </summary>
		public override void Run()
		{
		}
	}
}
