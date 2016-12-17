using System.Collections;
using System.Collections.Generic;

using Landis.Core;

namespace Landis.Species
{
    /// <summary>
    /// A read-only collection of species parameters.
    /// </summary>
    public class Dataset
        : ISpeciesDataset
    {
        private ISpecies[] species;

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return species.Length;
            }
        }

        //---------------------------------------------------------------------

        public ISpecies this[int index]
        {
            get {
                return species[index];
            }
        }

        //---------------------------------------------------------------------

        public ISpecies this[string name]
        {
            get {
                int index = IndexOf(name);
                if (index >= 0)
                    return species[index];
                else
                    return null;
            }
        }

        //---------------------------------------------------------------------

        public Dataset(List<ISpeciesParameters> parametersList)
        {
            if (parametersList == null)
                species = new ISpecies[0];
            else {
                species = new ISpecies[parametersList.Count];
                for (int index = 0; index < parametersList.Count; ++index)
                    species[index] = new Species(index, parametersList[index]);
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the index of a species parameters in the dataset.
        /// </summary>
        /// <returns>
        /// -1 if the species is not in the dataset.
        /// </returns>
        public int IndexOf(string name)
        {
            for (int index = 0; index < species.Length; ++index)
                if (species[index].Name == name)
                    return index;
            return -1;
        }

        //---------------------------------------------------------------------

        IEnumerator<ISpecies> IEnumerable<ISpecies>.GetEnumerator()
        {
            foreach (ISpecies sp in species)
                yield return sp;
        }
 
        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ISpecies>) this).GetEnumerator();
        }
    }
}
