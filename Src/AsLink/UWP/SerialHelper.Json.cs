using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AsLink
{
    public static class JsonHelper // Simple JSON serializer / deserializer for passing messages between processes
	{
		public static string ToJson<T>(T data)
		{
			try
			{
				var serializer = new DataContractJsonSerializer(typeof(T));
				using (MemoryStream ms = new MemoryStream())
				{
					serializer.WriteObject(ms, data);
					var jsonArray = ms.ToArray();
					return Encoding.UTF8.GetString(jsonArray, 0, jsonArray.Length);
				}
			}
			catch (Exception ex) { Debug.WriteLine($"$#~>{ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); /*else throw;*/ }

			return null;
		}

		public static T FromJson<T>(string json)
		{
			var deserializer = new DataContractJsonSerializer(typeof(T));
			try
			{
				using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
					return (T)deserializer.ReadObject(ms);
			}
			catch (SerializationException ex) { throw new SerializationException("Unable to deserialize JSON: " + json, ex); } // If the string could not be deserialized to an object from JSON then add the original string to the exception chain for debugging.
			catch (Exception ex) { Debug.WriteLine($"$#~>{ex.Message}"); if (Debugger.IsAttached) Debugger.Break(); /*else throw;*/ }

			return (T)Activator.CreateInstance(typeof(T)); //tu: Oct2016
		}
	}
}
