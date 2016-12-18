using System.Collections.Generic;

namespace Landis.Test.Main
{
    public class PlugInDataset
        : PlugIns.IDataset
    {
        private Dictionary<string, PlugIns.PlugInInfo> plugIns;

        //---------------------------------------------------------------------

        public PlugInDataset()
        {
            plugIns = new Dictionary<string, PlugIns.PlugInInfo>();
        }

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return plugIns.Count;
            }
        }

        //---------------------------------------------------------------------

        public PlugIns.PlugInInfo this[string name]
        {
            get {
                PlugIns.PlugInInfo plugIn;
                plugIns.TryGetValue(name, out plugIn);
                return plugIn;
            }
        }

        //---------------------------------------------------------------------

        public void AddPlugIn(string name,
                              string type,
                              string implementationName)
        {
            plugIns[name] = new PlugIns.PlugInInfo(name,
                                                   new PlugIns.PlugInType(type),
                                                   implementationName);
        }
    }
}
