using Landis.Utilities;

using Landis.Core;

namespace Landis.Ecoregions
{
    /// <summary>
    /// The parameters for a single ecoregion that can be edited.
    /// </summary>
    public class EditableParameters
        : IEditableParameters
    {
        private InputValue<string> name;
        private InputValue<string> description;
        private InputValue<ushort> mapCode;
        private InputValue<bool> active;

        //---------------------------------------------------------------------

        public InputValue<string> Name
        {
            get {
                return name;
            }

            set {
                if (value != null) {
                    if (value.Actual.Trim() == "")
                        throw new InputValueException(value.String, "Missing name");
                }
                name = value;
            }
        }

        //---------------------------------------------------------------------

        public InputValue<string> Description
        {
            get {
                return description;
            }

            set {
                description = value;
            }
        }

        //---------------------------------------------------------------------

        public InputValue<ushort> MapCode
        {
            get {
                return mapCode;
            }

            set {
                mapCode = value;
            }
        }

        //---------------------------------------------------------------------

        public InputValue<bool> Active
        {
            get {
                return active;
            }

            set {
                active = value;
            }
        }

        //---------------------------------------------------------------------

        public bool IsComplete
        {
            get {
                object[] parameters = new object[]{name, description, mapCode,
                                                   active};
                foreach (object parameter in parameters)
                    if (parameter == null)
                        return false;
                return true;
            }
        }

        //---------------------------------------------------------------------

        public IEcoregionParameters GetComplete()
        {
            if (this.IsComplete)
                return new Parameters(name.Actual,
                                      description.Actual,
                                      mapCode.Actual,
                                      active.Actual);
            else
                return null;
        }
    }
}
