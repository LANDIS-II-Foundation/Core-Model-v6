using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;

namespace Landis
{
    /// <summary>
    /// Editable list of information about a group of plug-ins.
    /// </summary>
    /// <remarks>
    /// The group may be empty, i.e., contain no plug-ins.
    /// </remarks>
    public class EditablePlugInList
        : ListOfEditable<EditablePlugIn, PlugInAndInitFile>
    {
        public EditablePlugInList()
            : base()
        {
        }
    }
}
