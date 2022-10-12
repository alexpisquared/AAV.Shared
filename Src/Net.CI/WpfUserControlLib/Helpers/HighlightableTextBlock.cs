namespace CI.Visual.Lib.Helpers;

public class HighlightableTextBlock : Control // latest + multi-word feature + IGNORE THE OTHER ONES!!!!!!!
{
  readonly char[] _delim = new char[] { '·', '|', '&' };

  static HighlightableTextBlock() => DefaultStyleKeyProperty.OverrideMetadata(typeof(HighlightableTextBlock), new FrameworkPropertyMetadata(typeof(HighlightableTextBlock)));

  public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(HighlightableTextBlock), new UIPropertyMetadata("", UpdateControlCallBack)); public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
  public static readonly DependencyProperty HighlightBackgroundProperty = DependencyProperty.Register("HighlightBackground", typeof(Brush), typeof(HighlightableTextBlock), new UIPropertyMetadata(Brushes.Yellow, UpdateControlCallBack)); public Brush HighlightBackground { get => (Brush)GetValue(HighlightBackgroundProperty); set => SetValue(HighlightBackgroundProperty, value); }
  public static readonly DependencyProperty HighlightForegroundProperty = DependencyProperty.Register("HighlightForeground", typeof(Brush), typeof(HighlightableTextBlock), new UIPropertyMetadata(Brushes.DarkRed, UpdateControlCallBack)); public Brush HighlightForeground { get => (Brush)GetValue(HighlightForegroundProperty); set => SetValue(HighlightForegroundProperty, value); }
  public static readonly DependencyProperty IsMatchCaseProperty = DependencyProperty.Register("IsMatchCase", typeof(bool), typeof(HighlightableTextBlock), new UIPropertyMetadata(false, UpdateControlCallBack)); public bool IsMatchCase { get => (bool)GetValue(IsMatchCaseProperty); set => SetValue(IsMatchCaseProperty, value); }
  public static readonly DependencyProperty IsHighlightProperty = DependencyProperty.Register("IsHighlight", typeof(bool), typeof(HighlightableTextBlock), new UIPropertyMetadata(true, UpdateControlCallBack)); public bool IsHighlight { get => (bool)GetValue(IsHighlightProperty); set => SetValue(IsHighlightProperty, value); }
  public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string), typeof(HighlightableTextBlock), new UIPropertyMetadata("", UpdateControlCallBack)); public string SearchText { get => (string)GetValue(SearchTextProperty); set => SetValue(SearchTextProperty, value); }
  public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(HighlightableTextBlock), new UIPropertyMetadata(TextWrapping.WrapWithOverflow, UpdateControlCallBack)); public TextWrapping TextWrapping { get => (TextWrapping)GetValue(TextWrappingProperty); set => SetValue(TextWrappingProperty, value); }

  static void UpdateControlCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as HighlightableTextBlock)?.InvalidateVisual();

  protected override void OnRender(DrawingContext drawingContext)
  {
    if (Template == null) return;

    if (string.IsNullOrEmpty(Text))
    {
      base.OnRender(drawingContext);
      return;
    }

    if (Template.FindName("PART_TEXT", this) is not TextBlock textBlock) // Define a TextBlock to hold the search result.
      throw new ArgumentException("Hey! PART_TEXT is not defined");

    textBlock.TextWrapping = TextWrapping;

    if (!IsHighlight)
    {
      textBlock.Text = Text;

      base.OnRender(drawingContext);
      return;
    }

    textBlock.Inlines.Clear();
    var searchstrings = IsMatchCase ? SearchText : (SearchText ?? "").ToUpperInvariant();
    var compareText = IsMatchCase ? Text : Text.ToUpperInvariant();
    var displayText = Text;

    Run? run;

    //foreach (var searchstring in searchstrings.Split(_delim, System.StringSplitOptions.RemoveEmptyEntries))
    {
      while (PosOfNearestMatch(compareText, searchstrings.Split(_delim, System.StringSplitOptions.RemoveEmptyEntries), out var searchstring) >= 0)
      {
        if (string.IsNullOrEmpty(searchstring)) break;

        var position = compareText.IndexOf(searchstring);

        run = GenerateRun(displayText[..position], false);
        if (run != null)
          textBlock.Inlines.Add(run);

        run = GenerateRun(displayText.Substring(position, searchstring.Length), true);
        if (run != null)
          textBlock.Inlines.Add(run);

        compareText = compareText[(position + searchstring.Length)..];
        displayText = displayText[(position + searchstring.Length)..];
      }
    }

    run = GenerateRun(displayText, false);
    if (run != null)
      textBlock.Inlines.Add(run);

    base.OnRender(drawingContext);
  }

  Run? GenerateRun(string searchedString, bool isHighlight)
  {
    if (string.IsNullOrEmpty(searchedString))
      return null;

    return isHighlight ?
      new Run(searchedString) { Background = HighlightBackground, Foreground = HighlightForeground, FontWeight = FontWeights.SemiBold } :
      new Run(searchedString) { Background = Background, Foreground = Foreground };
  }
  static int PosOfNearestMatch(string compareText, string[] searchstrings, out string searchstring)
  {
    searchstring = "";
    var pos = compareText.Length;
    foreach (var ss in searchstrings)
    {
      var p = compareText.IndexOf(ss);
      if (pos > p && p > -1)
      {
        pos = p;
        searchstring = ss;
      }
    }

    return pos == compareText.Length ? -1 : pos;
  }
}
