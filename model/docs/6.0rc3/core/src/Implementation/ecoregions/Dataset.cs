using System.Collections;
using System.Collections.Generic;

using Landis.Core;

namespace Landis.Ecoregions
{
    /// <summary>
    /// A read-only collection of ecoregion parameters.
    /// </summary>
    public class Dataset
        : IEcoregionDataset
    {
        private IEcoregion[] ecoregions;

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return ecoregions.Length;
            }
        }

        //---------------------------------------------------------------------

        public IEcoregion this[int index]
        {
            get {
                return ecoregions[index];
            }
        }

        //---------------------------------------------------------------------

        public IEcoregion this[string name]
        {
            get {
                int index = IndexOf(name);
                if (index >= 0)
                    return ecoregions[index];
                else
                    return null;
            }
        }

        //---------------------------------------------------------------------

        public Dataset(List<IEcoregionParameters> parametersList)
        {
            if (parametersList == null)
                ecoregions = new IEcoregion[0];
            else {
                ecoregions = new IEcoregion[parametersList.Count];
                for (int index = 0; index < parametersList.Count; ++index)
                    ecoregions[index] = new Ecoregion(index, parametersList[index]);
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the index of an ecoregion in the dataset.
        /// </summary>
        /// <returns>
        /// -1 if the ecoregion is not in the dataset.
        /// </returns>
        public int IndexOf(string name)
        {
            for (int index = 0; index < ecoregions.Length; ++index)
                if (ecoregions[index].Name == name)
                    return index;
            return -1;
        }

        //---------------------------------------------------------------------

        public IEcoregion Find(ushort mapCode)
        {
            foreach (IEcoregion ecoregion in ecoregions)
                if (ecoregion.MapCode == mapCode)
                    return ecoregion;
            return null;
        }

        //---------------------------------------------------------------------

        IEnumerator<IEcoregion> IEnumerable<IEcoregion>.GetEnumerator()
        {
            foreach (IEcoregion ecoregion in ecoregions)
                yield return ecoregion;
        }
 
        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IEcoregion>) this).GetEnumerator();
        }
    }
}
