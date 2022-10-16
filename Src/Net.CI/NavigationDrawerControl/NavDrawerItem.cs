using System.Windows;
using System.Windows.Controls;

namespace NavigationDrawerControl;

public class NavDrawerItem : RadioButton
{
  static NavDrawerItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(NavDrawerItem), new FrameworkPropertyMetadata(typeof(NavDrawerItem)));
}