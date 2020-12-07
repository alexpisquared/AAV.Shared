using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AAV.Anime.Views
{
  public partial class ArcPieChart : UserControl
  {
    public ArcPieChart()
    {
      InitializeComponent();
      e1.StrokeDashArray.Clear();
      e1.StrokeDashArray.Add(3.14);
    }
  }
}
