using Edu.Wisc.Forest.Flel.Util;

namespace Landis
{
    /// <summary>
    /// Editable list of information about a group of extensions.
    /// </summary>
    /// <remarks>
    /// The group may be empty, i.e., contain no extensions.
    /// </remarks>
    public class EditableExtensionList
        : ListOfEditable<EditableExtension, ExtensionAndInitFile>
    {
        public EditableExtensionList()
            : base()
        {
        }
    }
}
