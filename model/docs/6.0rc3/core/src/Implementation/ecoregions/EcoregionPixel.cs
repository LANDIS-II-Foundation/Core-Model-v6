using Landis.SpatialModeling;

namespace Landis.Ecoregions
{
    public class EcoregionPixel : Pixel
    {
        public Band<int> MapCode  = "The numeric code for each ecoregion";

        public EcoregionPixel() 
        {
            SetBands(MapCode);
        }
    }
}
