using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace SoftTeach.Resources.FontAwesome
{
  public class Icon : MarkupExtension
  {
    private readonly IconBlock _iconBlock;

    public Icon(IconChar icon)
    {
      _iconBlock = new IconBlock
      {
        FontSize = 24,
        Icon = icon
      };
    }

    public Brush Foreground
    {
      get => _iconBlock.Foreground;
      set => _iconBlock.Foreground = value;
    }

    public AwesomeFontType FontType
    {
      get => _iconBlock.FontType;
      set => _iconBlock.FontType = value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return _iconBlock;
    }
  }
}
