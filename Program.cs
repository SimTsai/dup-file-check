﻿string? inputPath = null;
string? outputPath = null;

if (args.Length > 0)
{
    foreach (var arg in args)
    {
        if (arg is null) { ShowUsage(); return; }
        var param = arg.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (param.Length != 2) { ShowUsage(); return; }
        var pName = param[0];
        var pValue = param[1];
        if (pName == "-p")
        {
            inputPath = pValue;
            continue;
        }
        else if (pName == "-o")
        {
            if (pValue.StartsWith("~/"))
            {
                pValue = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), pValue.Substring(2));

            }
            outputPath = Path.Combine(pValue, $"{DateTime.Now:yyyyMMddHHmmss}.json");
            continue;
        }
    }
}
else { ShowUsage(); return; }

if (inputPath is null || outputPath is null) { ShowUsage(); return; }

Console.WriteLine($"dup file check start check path: {inputPath}");
DirectoryInfo directoryInfo = new DirectoryInfo(inputPath);
var allFiles = directoryInfo.GetFiles("*", new EnumerationOptions
{
    RecurseSubdirectories = true
});

using var stdOut = Console.OpenStandardOutput();

System.Collections.Concurrent.ConcurrentDictionary<string, List<string>> dict = new System.Collections.Concurrent.ConcurrentDictionary<string, List<string>>();
System.Collections.Concurrent.ConcurrentDictionary<string, string> exDict = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();

await Parallel.ForEachAsync(allFiles, async (fileInfo, ct) =>
{
    try
    {
        using var fs = fileInfo.Open(FileMode.Open);
        await stdOut.WriteAsync(new byte[] { 0x2E });
        await stdOut.FlushAsync();
        var md5 = System.Security.Cryptography.MD5.Create();
        var hash = await md5.ComputeHashAsync(fs);
        var key = BitConverter.ToString(hash).Replace("-", string.Empty);
        dict.AddOrUpdate(key, k => new List<string> { fileInfo.FullName }, (key, old) => { old.Add(fileInfo.FullName); return old; });
    }
    catch (Exception ex)
    {
        exDict.TryAdd(fileInfo.FullName, ex.ToString());
    }
});
stdOut.Close();
var dup_files = dict.Where(p => p.Value.Count > 1);

using var fs = new FileStream(outputPath, FileMode.CreateNew);
await System.Text.Json.JsonSerializer.SerializeAsync(fs, new { dup_files, exception = exDict });
Console.WriteLine($"dup file check end result store: {outputPath}");


void ShowUsage()
{
    Console.WriteLine($"version {typeof(Program).Assembly.GetName().Version?.ToString(3)}");
    Console.WriteLine("dup-file-check -p check-path -o output-path");
}