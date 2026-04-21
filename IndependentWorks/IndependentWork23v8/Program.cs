class Program
{
    static void Main()
    {
        Console.WriteLine("=== Adapter Pattern ===");
        OldSensorDevice oldDevice = new OldSensorDevice();
        ISensorReader adapter = new OldSensorAdapter(oldDevice);
        double temp = adapter.ReadData();
        Console.WriteLine($"Temperature from adapter: {temp}C\n");

        Console.WriteLine("=== Facade Pattern ===");
        SensorDataFacade facade = new SensorDataFacade();
        facade.ProcessSensorData();
        Console.WriteLine();

        Console.WriteLine("=== Proxy Pattern ===");
        ISensorData sensorProxy = new CachedSensorDataProxy();

        Console.WriteLine("Call 1:");
        Console.WriteLine($"Sensor Value: {sensorProxy.GetValue()}C\n");
        Console.WriteLine("Call 2:");
        Console.WriteLine($"Sensor Value: {sensorProxy.GetValue()}C\n");
        Console.ReadKey();
        
    }
}
// Adapter pattern
public interface ISensorReader
{
    double ReadData();
}
public class OldSensorDevice
{
    public string ReadRawData()
    {
        Console.WriteLine("[OldSensorDevice] Reading raw data...");
        return "24.5";
    }
}
public class OldSensorAdapter : ISensorReader
{
    private readonly OldSensorDevice _oldDevice;

    public OldSensorAdapter(OldSensorDevice oldDevice)
    {
        _oldDevice = oldDevice;
    }

    public double ReadData()
    {
        string rawData = _oldDevice.ReadRawData();
        Console.WriteLine($"[OldSensorAdapter] Converting raw data: {rawData}");
        return double.Parse(rawData, System.Globalization.CultureInfo.InvariantCulture);
    }
}

// Facade pattern
// Subsystem 1
public class DataCollector
{
    public string Collect()
    {
        Console.WriteLine("[DataCollector] Collecting data...");
        return "Raw Sensor Data";
    }
}
// Subsystem 2
public class DataProcessor
{
    public string Process(string data)
    {
        Console.WriteLine($"[DataProcessor] Processing data: {data}");
        return $"Processed [{data}]";
    }
}
// Subsystem 3
public class DataStorage
{
    public void Save(string data)
    {
        Console.WriteLine($"[DataStorage] Storing data to Db: {data}");
    }
}

public class SensorDataFacade
{
    private readonly DataCollector _collector = new DataCollector();
    private readonly DataProcessor _processor = new DataProcessor();
    private readonly DataStorage _storage = new DataStorage();

    public void ProcessSensorData()
    {
        Console.WriteLine("[SensorDataFacade] Starting sensor data processing...");
        string raw = _collector.Collect();
        string processed = _processor.Process(raw);
        _storage.Save(processed);
    }
}
// Proxy pattern
public interface ISensorData
{
    double GetValue();
}
public class RealSensorData : ISensorData
{
    public double GetValue()
    {
        Console.WriteLine("[RealSensorData] Reading sensor value...");
        Thread.Sleep(1000); // Simulate delay
        return 99.9; // Simulated sensor value
    }
}

public class CachedSensorDataProxy : ISensorData
{
    private readonly RealSensorData _realData = new RealSensorData();
    private double? _cachedValue = null;

    public double GetValue()
    {
        if (_cachedValue == null)
        {
            Console.WriteLine("[CachedSensorDataProxy] Cache miss. Fetching real data...");
            _cachedValue = _realData.GetValue();
        }
        else
        {
            Console.WriteLine("[CachedSensorDataProxy] Cache hit. Returning cached value...");
        }
        return _cachedValue.Value;
    }
}