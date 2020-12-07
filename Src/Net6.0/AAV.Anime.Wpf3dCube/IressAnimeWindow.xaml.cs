using System.Windows;

namespace AAV.Anime.Wpf3dCube
{
  public partial class IressAnimeWindow : Window
  {
    public IressAnimeWindow() => InitializeComponent();

    private void Button_Click(object sender, RoutedEventArgs e) => Close();
  }
}
