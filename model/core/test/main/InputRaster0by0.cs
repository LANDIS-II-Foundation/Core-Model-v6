using Landis.Ecoregions;
using Pixel = Landis.Ecoregions.Pixel;

using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.RasterIO;

namespace Landis.Test.Main
{
    /// <summary>
    /// An input raster of ecoregion pixels with 0 rows and 0 columns.
    /// </summary>
    public class InputRaster0by0
        : InputRaster,
          IInputRaster<Pixel>
    {
        public InputRaster0by0(string    path,
                               IMetadata metadata)
            : base(path)
        {
            this.Dimensions = new Dimensions(0, 0);
            this.Metadata = metadata;
        }

        //---------------------------------------------------------------------

        public Pixel ReadPixel()
        {
            IncrementPixelsRead();
            throw new System.ApplicationException("Expected the IncrementPixelsRead method to throw exception");
        }
    }
}
