namespace StandardLib.Helpers;

public class SizeWeightedRandomPicker
{
  readonly Random _random = new(Guid.NewGuid().GetHashCode());
  List<FileInfo> _files;
  readonly long _totalSize;

  public SizeWeightedRandomPicker(string directoryPath)
  {
#if DEBUG_
    var searchPattern = "*.mts";
#else
    var searchPattern = "*";
#endif

    var start = Stopwatch.GetTimestamp();
    _files = new List<FileInfo>(new DirectoryInfo(directoryPath).GetFiles(searchPattern, SearchOption.AllDirectories));
    _totalSize = _files.Sum(file => file.Length);
    Count = _files.Count;

    WriteLine($"{DateTime.Now} ■ ■ ■ loaded {_files.Count:N0} {searchPattern} files of total {_totalSize * .000000001:N0} Gb in {Stopwatch.GetElapsedTime(start).TotalSeconds:N2} s.");
  }

  public int Count { get; private set; }
  //public List<FileInfo> Files { get => _files; private set => _files = value; }

  public FileInfo PickRandomFile()
  {
    var randomNumber = _random.NextInt64(_totalSize);
    long sum = 0;

    foreach (var file in _files)
    {
      sum += file.Length;
      if (randomNumber < sum)
        return file;
    }

    throw new ArgumentNullException("▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄"); //tmi: WriteLine($"{nameof(PickRandomFile)} execution time: {Stopwatch.GetElapsedTime(start).TotalSeconds:N2} s.");
  }

  public void Serialize(string filePath = _allFiles) => File.WriteAllText(filePath, JsonSerializer.Serialize(this));

  public static SizeWeightedRandomPicker Deserialize(string filePath = _allFiles) => JsonSerializer.Deserialize<SizeWeightedRandomPicker>(File.ReadAllText(filePath)) ?? throw new ArgumentNullException("▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄▀▄");

  const string _allFiles = (@"C:\g\Microsoft-Graph\Src\msgraph-training-uwp\DemoApp\Stuff\AllFiles.json");
}