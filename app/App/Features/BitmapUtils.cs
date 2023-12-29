using ImageMagick;
using IronOcr;
using System;
using System.Drawing;

namespace IOApp.Features
{
    public class BitmapUtils
    {
        public static OcrInput CropImage(Tuple<int, int, int, int> rectInit, string inputPath)
        {
            Bitmap originalImage = new Bitmap(inputPath);

            Rectangle cropRect = new Rectangle(
                rectInit.Item1, 
                rectInit.Item2, 
                rectInit.Item3, 
                rectInit.Item4);

            Bitmap croppedImage = originalImage.Clone(cropRect, originalImage.PixelFormat);
            return new OcrInput(croppedImage);
        }
    }
}