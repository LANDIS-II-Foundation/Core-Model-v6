using Landis.Core;
using Landis.Ecoregions;
using Landis.SpatialModeling;

namespace Landis.Test.Ecoregions
{
    /// <summary>
    /// An array-backed input raster of ecoregion pixels.
    /// </summary>
    public class InputRaster<TValue>
        : Landis.RasterIO.InputRaster,
          IInputRaster<EcoregionPixel>
        where TValue : struct
    {
        private TValue[,] data;
        private Location currentPixelLoc;
        private EcoregionPixel pixel;
        public delegate ushort ConvertToUShort<T>(T value);
        private ConvertToUShort<TValue> convertToUShort;

        //---------------------------------------------------------------------

        public EcoregionPixel BufferPixel
        {
            get {
                return pixel;
            }
        }

        //---------------------------------------------------------------------

        public InputRaster(string                  path,
                           TValue[,]               data,
                           ConvertToUShort<TValue> convertToUShort)
            : base(path)
        {
            if (! (typeof(TValue) == typeof(byte) || typeof(TValue) == typeof(ushort)))
                throw new System.ApplicationException("Type parameter TValue is not byte or ushort");
            this.data = data;
            this.Dimensions = new Dimensions(data.GetLength(0),
                                             data.GetLength(1));

            //  Initialize current pixel location so that RowMajor.Next returns
            //  location (1,1).
            this.currentPixelLoc = new Location(1, 0);

            this.pixel = new EcoregionPixel();
            this.convertToUShort = convertToUShort;
        }

        //---------------------------------------------------------------------

        public void ReadBufferPixel()
        {
            IncrementPixelsRead();
            currentPixelLoc = RowMajor.Next(currentPixelLoc, Dimensions.Columns);
            pixel.MapCode.Value = convertToUShort(data[currentPixelLoc.Row - 1,
                                                       currentPixelLoc.Column - 1]);
        }
    }
}
