using SoftTeach.ExceptionHandling;
using SoftTeach.Properties;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace SoftTeach.Resources.FontAwesome
{
  // cf.: 
  // * http://stackoverflow.com/questions/23108181/changing-font-icon-in-wpf-using-font-awesome
  // * http://www.codeproject.com/Tips/634540/Using-Font-Icons
  public static class IconHelper
  {
    public const double DefaultSize = 200.0;

    public static readonly Brush DefaultBrush = new SolidColorBrush(Settings.Default.Basisfarbe);

    //public static FontFamily FontAwesomeLight;
    //public static FontFamily FontAwesomeRegular;
    //public static FontFamily FontAwesomeSolid;
    //public static FontFamily FontAwesomeBrands;

    private static readonly GlyphTypeface GlyphTypefaceLight;
    private static readonly GlyphTypeface GlyphTypefaceRegular;
    private static readonly GlyphTypeface GlyphTypefaceSolid;
    private static readonly GlyphTypeface GlyphTypefaceBrands;
    private static readonly GlyphTypeface GlyphTypefaceThin;
    private static readonly GlyphTypeface GlyphTypefaceDuotone;

    private static readonly int Dpi = GetDpi();

    static IconHelper()
    {
      var FontAwesomeLight = Application.Current.FindResource("FontAwesomeLight") as FontFamily;
      var FontAwesomeRegular = Application.Current.FindResource("FontAwesomeRegular") as FontFamily;
      var FontAwesomeSolid = Application.Current.FindResource("FontAwesomeSolid") as FontFamily;
      var FontAwesomeBrands = Application.Current.FindResource("FontAwesomeBrands") as FontFamily;
      var FontAwesomeThin = Application.Current.FindResource("FontAwesomeThin") as FontFamily;
      var FontAwesomeDuotone = Application.Current.FindResource("FontAwesomeDuotone") as FontFamily;
      //var FontAwesomeLight = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Font Awesome 5 Pro Light");
      //var FontAwesomeRegular = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Font Awesome 5 Pro Regular");
      //var FontAwesomeSolid = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Font Awesome 5 Pro Solid");
      //var FontAwesomeBrands = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Font Awesome 5 Brands Regular");
      //FontAwesomeLight = new FontFamily("/FontAwesome.Sharp;component/fonts/FontAwesome5Pro-Light-300.otf#Font Awesome 5 Pro Light");
      //FontAwesomeRegular = new FontFamily("/FontAwesome.Sharp;component/fonts/FontAwesome5Pro-Regular-400.otf#Font Awesome 5 Pro Regular");
      //FontAwesomeSolid = new FontFamily("pack://application:,,,/FontAwesome.Sharp;component/fonts/FontAwesome5Pro-Solid-900.otf#Font Awesome 5 Pro Solid");
      //FontAwesomeBrands = new FontFamily("pack://application:,,,/FontAwesome.Sharp;component/fonts/FontAwesome5Brands-Regular-400.otf#Font Awesome 5 Brands Regular");

      var typeface = new Typeface(FontAwesomeLight, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceLight))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeLight.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceLight))
          Log.ProcessErrorMessage("No glyphtypeface found");
      }

      typeface = new Typeface(FontAwesomeRegular, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceRegular))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeRegular.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceRegular))
          Log.ProcessErrorMessage("No glyphtypeface found");
      }

      typeface = new Typeface(FontAwesomeSolid, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceSolid))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeSolid.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceSolid))
          Log.ProcessErrorMessage("No glyphtypeface found");
      }

      typeface = new Typeface(FontAwesomeBrands, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceBrands))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeBrands.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceBrands))
          Log.ProcessErrorMessage("No glyphtypeface found");
      }

      typeface = new Typeface(FontAwesomeThin, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceThin))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeThin.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceThin))
          Log.ProcessErrorMessage("No glyphtypeface found");
      }

      typeface = new Typeface(FontAwesomeDuotone, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
      if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceDuotone))
      {
        typeface = new Typeface(new FontFamily(new Uri("pack://application:,,,"), FontAwesomeDuotone.Source),
            FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        if (!typeface.TryGetGlyphTypeface(out GlyphTypefaceDuotone))
          Log.ProcessErrorMessage("No glyphtypeface found");
      }
    }

    public static ImageSource ToImageSource(this IconChar iconChar, AwesomeFontType fontType = AwesomeFontType.Regular, Brush foregroundBrush = null, double size = DefaultSize)
    {
      var text = char.ConvertFromUtf32((int)iconChar);
      return ToImageSource(text, fontType, foregroundBrush ?? DefaultBrush, size);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public static ImageSource ToImageSource(string text, AwesomeFontType fontType = AwesomeFontType.Regular, Brush foregroundBrush = null, double size = DefaultSize)
    {
      if (string.IsNullOrWhiteSpace(text)) return null;

      var glyphIndexes = new ushort[text.Length];
      var advanceWidths = new double[text.Length];
      GlyphTypeface glyphtypeface;

      switch (fontType)
      {
        case AwesomeFontType.Light:
          glyphtypeface = GlyphTypefaceLight;
          break;
        default:
        case AwesomeFontType.Regular:
          glyphtypeface = GlyphTypefaceRegular;
          break;
        case AwesomeFontType.Solid:
          glyphtypeface = GlyphTypefaceSolid;
          break;
        case AwesomeFontType.Brands:
          glyphtypeface = GlyphTypefaceBrands;
          break;
        case AwesomeFontType.Thin:
          glyphtypeface = GlyphTypefaceThin;
          break;
        case AwesomeFontType.Duotone:
          glyphtypeface = GlyphTypefaceDuotone;
          break;
      }

      for (var n = 0; n < text.Length; n++)
      {
        ushort glyphIndex;
        try
        {
          glyphIndex = glyphtypeface.CharacterToGlyphMap[text[n]];
        }
        catch (Exception)
        {
          glyphIndex = 42;
        }
        glyphIndexes[n] = glyphIndex;

        var width = glyphtypeface.AdvanceWidths[glyphIndex] * 1.0;
        advanceWidths[n] = width;
      }

      try
      {
        var fontSize = PixelsToPoints(size);
        var glyphRun = new GlyphRun(
          glyphtypeface,
          0, 
          false, 
          fontSize,
          Dpi/96f,
          glyphIndexes,
          new Point(0, 0), advanceWidths, null, null, null, null, null, null);

        var glyphRunDrawing = new GlyphRunDrawing(foregroundBrush ?? DefaultBrush, glyphRun);
        return new DrawingImage(glyphRunDrawing);
      }
      catch (Exception ex)
      {
        Trace.TraceError($"Error generating GlyphRun : {ex.Message}");
      }
      return null;
    }

    private static double PixelsToPoints(double size)
    {
      // pixels to points, cf.: http://stackoverflow.com/a/139712/2592915
      return size * (72.0 / Dpi);
    }

    private static int GetDpi()
    {
      // How can I get the DPI in WPF?, cf.: http://stackoverflow.com/a/12487917/2592915
      var dpiProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
      return (int)dpiProperty.GetValue(null, null);
    }
  }
}
