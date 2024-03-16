# .NET Standard 2.0 class library. 

Why not 2.1? What is the difference?

The difference between .NET Standard 2.0 and 2.1 lies in their compatibility and feature sets.
.NET Standard is a specification that defines a set of APIs that are available across different .NET implementations, such as .NET Framework, .NET Core, and Xamarin. It provides a common set of APIs that developers can rely on when building libraries and applications that target multiple .NET platforms.
.NET Standard 2.0 was introduced earlier and has broader compatibility with various .NET implementations. It supports a wide range of platforms, including .NET Framework 4.6.1 and later, .NET Core 2.0 and later, and Xamarin.iOS and Xamarin.Android.
On the other hand, .NET Standard 2.1 was introduced later and includes additional APIs and features. It requires more recent versions of the .NET implementations, such as .NET Core 3.0 and later, and Xamarin.iOS 12.16 and Xamarin.Android 10.0.
The decision to use .NET Standard 2.0 or 2.1 depends on the target platforms you need to support. If you need to target older versions of .NET Framework or .NET Core, or if you need to support Xamarin platforms with older versions, then .NET Standard 2.0 is a better choice. However, if you are targeting newer versions of .NET Core or Xamarin platforms, and you can take advantage of the additional APIs and features provided by .NET Standard 2.1, then you can consider using it.
It's important to note that .NET Standard 2.1 is not backward compatible with .NET Standard 2.0. So, if you choose to upgrade from 2.0 to 2.1, you may need to make some changes to your code to accommodate the new APIs and features.
In summary, the difference between .NET Standard 2.0 and 2.1 lies in their compatibility and the additional APIs and features provided by 2.1. The choice between them depends on the target platforms you need to support and the specific requirements of your project.


.NET Standard 2.1 introduces several additional APIs and features compared to .NET Standard 2.0. Some of the notable additions include:
1.	Span<T> and Memory<T>: These types provide efficient and safe ways to work with contiguous blocks of memory, enabling high-performance scenarios such as data processing and network communication.
2.	ValueTask: ValueTask is a lightweight alternative to Task for representing asynchronous operations. It reduces memory allocations and improves performance in scenarios where the result is often available synchronously.
3.	IAsyncEnumerable<T>: This interface allows you to work with asynchronous streams of data. It enables efficient processing of large datasets without loading everything into memory at once.
4.	System.Text.Json: .NET Standard 2.1 includes the System.Text.Json namespace, which provides a high-performance JSON serialization and deserialization API. It is a lightweight alternative to Newtonsoft.Json and is optimized for performance.
5.	Support for new platforms: .NET Standard 2.1 requires more recent versions of the .NET implementations, such as .NET Core 3.0 and later, and Xamarin.iOS 12.16 and Xamarin.Android 10.0. This means that you can take advantage of the latest features and improvements available in these platforms.
These are just a few examples of the additional APIs and features provided by .NET Standard 2.1. The new APIs and features are designed to improve performance, enable new scenarios, and enhance developer productivity. When considering whether to use .NET Standard 2.1, it's important to evaluate whether these additional APIs and features align with the requirements of your project and the target platforms you need to support.
