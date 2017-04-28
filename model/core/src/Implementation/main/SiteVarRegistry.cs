using Landis.Utilities;
using System;
using System.Collections.Generic;
// using Wisc.Flel.GeospatialModeling.Landscapes;

using Landis.SpatialModeling;

namespace Landis
{
    /// <summary>
    /// The site-variable registry for the model.
    /// </summary>
    public class SiteVarRegistry
    {
        private Dictionary<string, ISiteVariable> registeredVars;

        //-----------------------------------------------------------------

        public SiteVarRegistry()
        {
            registeredVars = new Dictionary<string, ISiteVariable>();
        }

        //-----------------------------------------------------------------

        /// <summary>
        /// Registers a site variable under a specific name.
        /// </summary>
        /// <param name="siteVar">
        /// The site variable being registered.
        /// </param>
        /// <param name="name">
        /// The name under which the site variable is to be registered.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// At least one of the parameters is null.
        /// </exception>
        /// <exception cref="System.ApplicationException">
        /// Another site variable has been previously registered with the same
        /// name.
        /// </exception>
        public void RegisterVar(ISiteVariable siteVar,
                                string        name)
        {
            Require.ArgumentNotNull(siteVar);
            Require.ArgumentNotNull(name);
            if (registeredVars.ContainsKey(name))
                throw new ApplicationException(string.Format("A site variable has already been registered with the name \"{0}\"",
                                                             name));

            registeredVars[name] = siteVar;
        }

        //-----------------------------------------------------------------

        /// <summary>
        /// Gets a site variable with a specific name and data type.
        /// </summary>
        /// <param name="name">
        /// The name under which the site variable is to be registered.
        /// </param>
        /// <returns>
        /// The site variable that was registered under the given name, or null
        /// if no site variable has been registered under the name.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// name is null.
        /// </exception>
        /// <exception cref="System.ApplicationException">
        /// The site variable registered with the given name has a different
        /// data type.
        /// </exception>
        /*
        public ISiteVar<T> GetVar<T>(string name)
        {
            
            Require.ArgumentNotNull(name);
            ISiteVariable siteVar;
            ISiteVar<T> tempVar;
            
            
            if (registeredVars.TryGetValue(name, out siteVar)) 
            {
                //if (siteVar is ISiteVar<T>)
                    return (ISiteVar<T>) siteVar;
                //throw new ApplicationException(string.Format("The data type of site variable \"{0}\" is {1}, not {2}",
                //                                              // name, siteVar.DataType.Name, typeof(T).Name));
                //                                              name, siteVar.DataType.ToString(), typeof(T).ToString()));
            }
            
            
            
            if (registeredVars.TryGetValue(name, out siteVar)) 
            {
                if (siteVar is ISiteVar<T>)
                    return (ISiteVar<T>)siteVar;
                else
                {
                    tempVar = siteVar as ISiteVar<T>;
                    return tempVar;
                }

                //throw new ApplicationException(string.Format("The data type of site variable \"{0}\" is {1}, not {2}",
                //                                              name, siteVar.DataType.Name, typeof(T).Name));
            }

            Console.Out.WriteLine("*************** ALLLLLLLLLLLLLLLOOOOOOOOOHAAAAAAA 3");

            return null;
         }
            */ 
           
        public ISiteVar<T> GetVar<T>(string name)
        {
            Require.ArgumentNotNull(name);
            ISiteVariable siteVar;
            if (registeredVars.TryGetValue(name, out siteVar)) {
                if (siteVar is ISiteVar<T>)
                    return (ISiteVar<T>) siteVar;
                throw new ApplicationException(string.Format("The data type of site variable \"{0}\" is {1}, not {2}",
                                                             // name, siteVar.DataType.Name, typeof(T).Name));
                                                             name, siteVar.DataType.FullName, typeof(T).FullName));
            }
            return null;
        }
        
        //-----------------------------------------------------------------

        /// <summary>
        /// Removes all the registered variables.
        /// </summary>
        public void Clear()
        {
            registeredVars.Clear();
        }
    }
}
