using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AsLink
{
    public partial class WebSerialHelper /// requires C:\c\AsLink\UWP\WebHelper.uwp.cs
	{
        public static async Task<T> UrlToInstnace<T>(string url, TimeSpan? expiryPeriod = null)
        {
            try
            {
                // Debug.WriteLine($"══{url}");
                var webStr = await WebHelper.GetWebStr(url, expiryPeriod); // Debug.WriteLine($"{url}:\r\n{txt}");     
                if (webStr.StartsWith("<")) return (T)new XmlSerializer(typeof(T)).Deserialize(new StringReader(webStr));
                if (webStr.StartsWith("[") || webStr.StartsWith("{")) return (T)JsonStringSerializer.Load<T>(webStr); //needs what? (check BusCatch) ... probably C:\c\AsLink\PlatformNeutral\JsonHelper.cs
                else
                {
                    Debug.WriteLine(/*ApplicationData.Current.RoamingSettings.Values["Exn"] = */$"\r\nFAILED deserial-n: \r\n{url}\r\n{webStr}");
                }
            }
            catch //(Exception ex)
            {
                //ex.Pop(url);
                //ApplicationData.Current.RoamingSettings.Values["Exn"] = $"\r\nFAILED deserial-n: \r\n{url}";
                throw;
            }

            return default(T); // null 
        }
    }

#if !!TempFix // use instead: C:\c\AsLink\PlatformNeutral\JsonHelper.cs
	public class JsonHelper 	{		public static T FromJson<T>(string txt)		{			return default(T);		}	}
#endif
}
