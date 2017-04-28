using System.Collections.Generic;

namespace Landis.Utilities
{
    /// <summary>
    /// List of editable items.
    /// </summary>
    public class ListOfEditable<TEditableItem, TItem>
        : List<TEditableItem>, IEditable<TItem[]>
        where TEditableItem : IEditable<TItem>
    {
        /// <summary>
        /// Initializes a new instance with the default capacity.
        /// </summary>
        public ListOfEditable()
            : base()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with a specific capacity.
        /// </summary>
        public ListOfEditable(int capacity)
            : base(capacity)
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with the elements copied from a
        /// collection.
        /// </summary>
        public ListOfEditable(IEnumerable<TEditableItem> collection)
            : base(collection)
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Indicates if the list is complete; it is complete if each item is
        /// complete.
        /// </summary>
        public bool IsComplete
        {
            get {
                foreach (TEditableItem item in this)
                    if (! item.IsComplete)
                        return false;
                return true;
            }
        }

        //---------------------------------------------------------------------

        ///    <summary>
        /// Gets an array of complete items.
        /// </summary>
        public TItem[] GetComplete()
        {
            if (IsComplete) {
                TItem[] completeItems = new TItem[this.Count];
                for (int i = 0; i < this.Count; i++)
                    completeItems[i] = this[i].GetComplete();
                return completeItems;
            }
            else
                return null;
        }
    }
}
