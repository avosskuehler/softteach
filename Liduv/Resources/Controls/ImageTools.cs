using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liduv.Resources.Controls
{
  using System.Windows;
  using System.Windows.Interop;
  using System.Windows.Media.Imaging;

  public class ImageTools
  {
    /// <summary>
    /// The create bitmap from bitmap image.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <returns>
    /// The <see cref="Bitmap"/>.
    /// </returns>
    public static System.Drawing.Bitmap CreateBitmapFromBitmapImage(BitmapSource source)
    {
      var bmp = new System.Drawing.Bitmap(source.PixelWidth, source.PixelHeight);

      // Lock the bitmap's bits.  
      System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
      System.Drawing.Imaging.BitmapData bmpData =
        bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

      // Get the address of the first line
      IntPtr ptr = bmpData.Scan0;
      int bytes = bmpData.Stride * bmp.Height;
      source.CopyPixels(Int32Rect.Empty, ptr, bytes, bmpData.Stride);

      // Unlock the bits.
      bmp.UnlockBits(bmpData);

      return bmp;
    }

    /// <summary>
    /// This methods converts the given bitmap into a BitmapSource
    /// that can be used as an image source for wpf buttons.
    /// </summary>
    /// <param name="bmp">The <see cref="Bitmap"/> to be converted</param>
    /// <returns>A <see cref="BitmapSource"/> containing the given bitmap.</returns>
    public static BitmapSource CreateBitmapSourceFromBitmap(System.Drawing.Bitmap bmp)
    {
      return Imaging.CreateBitmapSourceFromHBitmap(
        bmp.GetHbitmap(),
        IntPtr.Zero,
        Int32Rect.Empty,
        BitmapSizeOptions.FromEmptyOptions());
    }

    /// <summary>
    /// Scales the image.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="maxWidth">The maximum width.</param>
    /// <param name="maxHeight">The maximum height.</param>
    /// <returns></returns>
    public static System.Drawing.Bitmap ScaleImage(System.Drawing.Bitmap image, int maxWidth, int maxHeight)
    {
      var ratioX = (double)maxWidth / image.Width;
      var ratioY = (double)maxHeight / image.Height;
      var ratio = Math.Min(ratioX, ratioY);

      var newWidth = (int)(image.Width * ratio);
      var newHeight = (int)(image.Height * ratio);

      var newImage = new System.Drawing.Bitmap(newWidth, newHeight);
      System.Drawing.Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
      return newImage;
    }
  }
}
