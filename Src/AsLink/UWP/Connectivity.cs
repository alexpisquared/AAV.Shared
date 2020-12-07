using Windows.Networking.Connectivity;

namespace AsLink
{
	public sealed class Connectivity
	{
		public static bool IsInternet() //more at: https://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh452991.aspx
		{
			ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
			bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
			return internet;
		}
	}
}
