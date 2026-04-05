using System.Diagnostics;

public class Program
{
    public static async Task Main()
    {
        var processor = new IpFileProcessor();
        string inputFilePath = "ips.txt";
        string outputFilePath = "unique_ips.txt";

        //Generate file with random IP addresses
        processor.GenerateIpFile(inputFilePath, 1_000_000);

        //async 
        var swAsync = Stopwatch.StartNew();
        var uniqueIpsAsync = await processor.ProcessIpFileAsync(inputFilePath);
        await processor.SaveToFileAsync(uniqueIpsAsync, outputFilePath);
        swAsync.Stop();
        Console.WriteLine($"Unique IPs (async) saved to {outputFilePath}");
        Console.WriteLine($"Time taken (async): {swAsync.ElapsedMilliseconds} ms");

        //sync
        var swSync = Stopwatch.StartNew();
        var uniqueIpsSync = processor.ProcessIpFile(inputFilePath);
        await processor.SaveToFileAsync(uniqueIpsSync, "unique_ips_sync.txt");
        swSync.Stop();
        Console.WriteLine($"Unique IPs (sync) saved to unique_ips_sync.txt");
        Console.WriteLine($"Time taken (sync): {swSync.ElapsedMilliseconds} ms");
    }
}
public class IpRecord
{
    public string IpAdress { get; set; }
}
public class IpFileProcessor
{
    // Generating file with random IP addresses
    public void GenerateIpFile(string filePath, int lines)
    {
        var random = new Random();
        using (var writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < lines; i++)
            {
                string ipAddress = $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(1, 255)}";
                writer.WriteLine(ipAddress);
            }
        }
    }
    // async version
    public async Task<HashSet<string>> ProcessIpFileAsync(string filePath)
    {
        var UniqueIps = new HashSet<string>();
        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                UniqueIps.Add(line);
            }
        }
        return UniqueIps;
    }
    // saving unique IPs to file
    public async Task SaveToFileAsync(HashSet<string> uniqueIps, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var ip in uniqueIps)
            {
                await writer.WriteLineAsync(ip);
            }
        }
    }
    // sync version
    public HashSet<string> ProcessIpFile(string filePath)
    {
        var UniqueIps = new HashSet<string>();
        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                UniqueIps.Add(line);
            }
        }
        return UniqueIps;
    }
}