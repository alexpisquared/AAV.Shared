using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AAV.Common
{
  public class HighlightableTextBlock : Control // latest + multi-word feature
  {
    readonly char[] _delim = new char[] { '·', '|', '&' };

    static HighlightableTextBlock() => DefaultStyleKeyProperty.OverrideMetadata(typeof(HighlightableTextBlock), new FrameworkPropertyMetadata(typeof(HighlightableTextBlock)));

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(HighlightableTextBlock), new UIPropertyMetadata("", updateControlCallBack)); public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public static readonly DependencyProperty HighlightBackgroundProperty = DependencyProperty.Register("HighlightBackground", typeof(Brush), typeof(HighlightableTextBlock), new UIPropertyMetadata(Brushes.Yellow, updateControlCallBack)); public Brush HighlightBackground { get => (Brush)GetValue(HighlightBackgroundProperty); set => SetValue(HighlightBackgroundProperty, value); }
    public static readonly DependencyProperty HighlightForegroundProperty = DependencyProperty.Register("HighlightForeground", typeof(Brush), typeof(HighlightableTextBlock), new UIPropertyMetadata(Brushes.DarkRed, updateControlCallBack)); public Brush HighlightForeground { get => (Brush)GetValue(HighlightForegroundProperty); set => SetValue(HighlightForegroundProperty, value); }
    public static readonly DependencyProperty IsMatchCaseProperty = DependencyProperty.Register("IsMatchCase", typeof(bool), typeof(HighlightableTextBlock), new UIPropertyMetadata(false, updateControlCallBack)); public bool IsMatchCase { get => (bool)GetValue(IsMatchCaseProperty); set => SetValue(IsMatchCaseProperty, value); }
    public static readonly DependencyProperty IsHighlightProperty = DependencyProperty.Register("IsHighlight", typeof(bool), typeof(HighlightableTextBlock), new UIPropertyMetadata(true, updateControlCallBack)); public bool IsHighlight { get => (bool)GetValue(IsHighlightProperty); set => SetValue(IsHighlightProperty, value); }
    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string), typeof(HighlightableTextBlock), new UIPropertyMetadata("", updateControlCallBack)); public string SearchText { get => (string)GetValue(SearchTextProperty); set => SetValue(SearchTextProperty, value); }
    public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(HighlightableTextBlock), new UIPropertyMetadata(TextWrapping.WrapWithOverflow, updateControlCallBack)); public TextWrapping TextWrapping { get => (TextWrapping)GetValue(TextWrappingProperty); set => SetValue(TextWrappingProperty, value); }

    static void updateControlCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as HighlightableTextBlock)?.InvalidateVisual();

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (Template == null) return;

      if (string.IsNullOrEmpty(Text))
      {
        base.OnRender(drawingContext);
        return;
      }

      var textBlock = Template.FindName("PART_TEXT", this) as TextBlock; // Define a TextBlock to hold the search result.
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

      Run run;

      //foreach (var searchstring in searchstrings.Split(_delim, System.StringSplitOptions.RemoveEmptyEntries))
      {
        while (posOfNearestMatch(compareText, searchstrings.Split(_delim, System.StringSplitOptions.RemoveEmptyEntries), out var searchstring) >= 0)
        {
          if (string.IsNullOrEmpty(searchstring)) break;

          var position = compareText.IndexOf(searchstring);

          run = generateRun(displayText.Substring(0, position), false);
          if (run != null)
            textBlock.Inlines.Add(run);

          run = generateRun(displayText.Substring(position, searchstring.Length), true);
          if (run != null)
            textBlock.Inlines.Add(run);

          compareText = compareText.Substring(position + searchstring.Length);
          displayText = displayText.Substring(position + searchstring.Length);
        }
      }

      run = generateRun(displayText, false);
      if (run != null)
        textBlock.Inlines.Add(run);

      base.OnRender(drawingContext);
    }

    Run generateRun(string searchedString, bool isHighlight)
    {
      if (string.IsNullOrEmpty(searchedString))
        return null;

      return isHighlight ?
        new Run(searchedString) { Background = HighlightBackground, Foreground = HighlightForeground, FontWeight = FontWeights.SemiBold } :
        new Run(searchedString) { Background = Background, Foreground = Foreground };
    }
    static int posOfNearestMatch(string compareText, string[] searchstrings, out string searchstring)
    {
      searchstring = null;
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
}