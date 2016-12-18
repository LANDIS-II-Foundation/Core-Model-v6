using Landis.Core;
using Landis.SpatialModeling;
using System;

namespace Landis.Ecoregions
{
    public class Map
    {
        private string path;
        private IEcoregionDataset ecoregions;
        private IRasterFactory rasterFactory;
        

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="path">
        /// Path to the raster file that represents the map.
        /// </param>
        /// <param name="ecoregions">
        /// The dataset of ecoregions that are in the map.
        /// </param>
        /// <param name="rasterFactory">
        /// The raster factory to use to read the map.
        /// </param>
        public Map(string         path,
                   IEcoregionDataset       ecoregions,
                   IRasterFactory rasterFactory)
        {
            this.path = path;
            this.ecoregions = ecoregions;
            this.rasterFactory = rasterFactory;
            
            
            try
            {
                IInputRaster<EcoregionPixel> map = rasterFactory.OpenRaster<EcoregionPixel>(path);
                using (map)
                {
                    //this.metadata = map.Metadata;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("#### Internal error occurred within the program:");
                Console.WriteLine("  {0}", exc.Message);
                for (Exception innerException = exc.InnerException; innerException != null; innerException = innerException.InnerException)
                {
                    Console.WriteLine("  {0}", innerException.Message);
                }
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Opens the map as an input grid of boolean values.
        /// </summary>
        /// <remarks>
        /// For use in constructing a landscape.
        /// </remarks>
        
        public IInputGrid<bool> OpenAsInputGrid()
        {
            IInputRaster<EcoregionPixel> map = rasterFactory.OpenRaster<EcoregionPixel>(path);
            return new InputGrid(map, ecoregions);
        }
        
        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a site variable with ecoregions.
        /// </summary>
        public ISiteVar<IEcoregion> CreateSiteVar(ILandscape landscape)
        {
            ISiteVar<IEcoregion> siteVar = landscape.NewSiteVar<IEcoregion>();
            IInputRaster<EcoregionPixel> map = rasterFactory.OpenRaster<EcoregionPixel>(path);
            Console.WriteLine("  reading in ecoregion from {0} ", path);
            using (map)
            {
                EcoregionPixel pixel = map.BufferPixel;
                foreach (Site site in landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    ushort mapCode = (ushort)pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        siteVar[site] = ecoregions.Find(mapCode);
                    }
                    if (!site.IsActive && ecoregions.Find(mapCode).Active) 
                    {
                        String msg = String.Format("  Site not active and ecoregion is active.  Ecoreigon = {0}", ecoregions.Find(mapCode).Name);
                        throw new ApplicationException(msg);
                        //Console.WriteLine("  Site not active"); 
                    }
                    if (site.IsActive && !ecoregions.Find(mapCode).Active)
                    {
                        String msg = String.Format("  Site is active and ecoregion is not active.  Ecoreigon = {0}", ecoregions.Find(mapCode).Name);
                        throw new ApplicationException(msg);
                    }
                }
            }
            return siteVar;
        }
    }
}
