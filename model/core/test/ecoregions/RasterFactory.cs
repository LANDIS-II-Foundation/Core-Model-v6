using Landis.Core;
using Landis.Ecoregions;
using Landis.SpatialModeling;
using System;
using System.Collections.Generic;

namespace Landis.Test.Ecoregions
{
    /// <summary>
    /// A mock raster factory that handles just array-backed input rasters
    /// of ecoregion pixels.
    /// </summary>
    public class RasterFactory
        : IRasterFactory
    {
        private Dictionary<string, object> dataArrays;

        //---------------------------------------------------------------------

        public RasterFactory()
        {
            this.dataArrays = new Dictionary<string, object>();
        }

        //---------------------------------------------------------------------

        public void SetData(string  path,
                            byte[,] data)
        {
            dataArrays[path] = data;
        }

        //---------------------------------------------------------------------

        public void SetData(string    path,
                            ushort[,] data)
        {
            dataArrays[path] = data;
        }

        //---------------------------------------------------------------------

        public IInputRaster<TPixel> OpenRaster<TPixel>(string path)
            where TPixel : Pixel, new()
        {
            if (typeof(TPixel) != typeof(EcoregionPixel))
                throw new ApplicationException("Only valid pixel type is Landis.Ecoregions.EcoregionPixel");

            object data;
            if (! dataArrays.TryGetValue(path, out data))
                throw new ApplicationException("Unknown path: " + path);

            IInputRaster<EcoregionPixel> raster;
            if (data is byte[,])
                raster = new InputRaster<byte>(path,
                                               (byte[,]) data,
                                               Convert.ToUInt16);
            else
                raster = new InputRaster<ushort>(path,
                                                 (ushort[,]) data,
                                                 Convert.ToUInt16);
            return (IInputRaster<TPixel>) raster;
        }

        //---------------------------------------------------------------------

        public IOutputRaster<TPixel> CreateRaster<TPixel>(string     path,
                                                          Dimensions dimensions)
            where TPixel : Pixel, new()
        {
            throw new NotSupportedException(GetType().FullName + " cannot write rasters");
        }
    }
}
