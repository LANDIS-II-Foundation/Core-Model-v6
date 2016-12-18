namespace Landis.Null
{
	public class Output
		: PlugIns.PlugIn
	{
		public Output()
		    : base("Null Output", new PlugIns.PlugInType("output"))
		{
		}

		//---------------------------------------------------------------------

		public override void Initialize(string        dataFile,
		                                PlugIns.ICore modelCore)
		{
		}

		//---------------------------------------------------------------------

		public override void Run()
		{
		}
	}
}
