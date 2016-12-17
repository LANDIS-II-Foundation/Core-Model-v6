using Landis.Ecoregions;
using Pixel = Landis.Ecoregions.Pixel;

using System;
using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.RasterIO;

namespace Landis.Test.Main
{
    /// <summary>
    /// A raster-driver manager that returns empty input rasters (0 rows by
    /// 0 columns).
    /// </summary>
    public class RasterDriverManager
        : IDriverManager
    {
        public IMetadata RasterMetadata;

        //---------------------------------------------------------------------

        public RasterDriverManager()
        {
        }

        //---------------------------------------------------------------------

        public IInputRaster<TPixel> OpenRaster<TPixel>(string path)
            where TPixel : IPixel, new()
        {
            if (typeof(TPixel) != typeof(Pixel))
                throw new ApplicationException("Only valid pixel type is Landis.Ecoregions.Pixel");

            IInputRaster<Pixel> raster = new InputRaster0by0(path, RasterMetadata);
            return (IInputRaster<TPixel>) raster;
        }

        //---------------------------------------------------------------------

        public IOutputRaster<TPixel> CreateRaster<TPixel>(string     path,
                                                          Dimensions dimensions,
                                                          IMetadata  metadata)
            where TPixel : IPixel, new()
        {
            throw new NotSupportedException(GetType().FullName + " cannot write rasters");
        }
    }
}
