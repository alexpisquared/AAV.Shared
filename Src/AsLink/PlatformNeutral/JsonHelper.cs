using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json; // requires ref: System.Runtime.Serialization
using System.Text;
using Newtonsoft.Json;

namespace AsLink
{
  public static class JsonStringSerializer // _4_uwp // for UWP; for win32 use C:\c\AsLink\UniSerializer.cs - it has the same and more ..much more.
  {
    public static string Save<T>(T data) => JsonHelper.ToJson<T>(data);
    public static T Load<T>(string json) => JsonHelper.FromJson<T>(json);
  }

  public static class JsonHelper // Last edit 2016-10-28
  {
    public static string ToJson<T>(T data)
    {
      try
      {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings { MaxDepth = 128 }; // Dec 2022: as per https://github.com/alexpisquared/BusCatch/security/dependabot/1

        var serializer = new DataContractJsonSerializer(typeof(T));
        using (var ms = new MemoryStream())
        {
          serializer.WriteObject(ms, data);
          var jsonArray = ms.ToArray();
          return Encoding.UTF8.GetString(jsonArray/*, 0, jsonArray.Length*/);
        }
      }
      catch (Exception ex) { Debug.WriteLine(ex.Message); throw; /*ex.Pop(); */} // SerializationCodeIsMissingForType, System.Collections.IEnumerable
    }

    public static T FromJson<T>(string json) //catch ... : lets the caller handle the failure: it knows better what to do
    {
      JsonConvert.DefaultSettings = () => new JsonSerializerSettings { MaxDepth = 128 }; // Dec 2022: as per https://github.com/alexpisquared/BusCatch/security/dependabot/1

      if (!string.IsNullOrEmpty(json))
        try
        {

          var deserializer = new DataContractJsonSerializer(typeof(T));
          using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
          {
            return (T)deserializer.ReadObject(ms);
          }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); throw; /*ex.Pop(json);*/ }

      return (T)Activator.CreateInstance(typeof(T));
    }
  }
}
