using System.Reflection;

namespace WindowsFormsLib;
class SystemInformationAdapter
{
  public bool IsLeftHandedMouse()
  {
    var propname = "MouseButtonsSwapped";

    var t = typeof(System.Windows.Forms.SystemInformation);
    var pi = t.GetProperties();
    PropertyInfo? prop = null;
    for (var i = 0; i < pi.Length; i++)
      if (pi[i].Name == propname)
      {
        prop = pi[i];
        break;
      }

    ArgumentNullException.ThrowIfNull(prop);
    var propval = prop.GetValue(null, null);
    WriteLine($"The value of the {propname} property is: {propval}");

    return true;
  }
}
