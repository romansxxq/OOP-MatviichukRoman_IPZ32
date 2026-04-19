public class Program
{
    static void Main(string[] args)
    {
        var converterManager = ConverterManager.Instance;

        // Set and use DocToPdfFactory
        converterManager.SetConverterFactory(new DocToPdfFactory());
        converterManager.ConvertFile("example.doc");

        // Set and use JpgToPngFactory
        converterManager.SetConverterFactory(new JpgToPngFactory());
        converterManager.ConvertFile("photo.jpg");

        // Set and use CsvToJsonFactory
        converterManager.SetConverterFactory(new CsvToJsonFactory());
        converterManager.ConvertFile("data.csv");
        
        Console.WriteLine("All conversions completed.");
        Console.ReadLine();
    }
}
// Interfaces
public interface IFileConverter
{
    void Convert(string fileName);
}

// Converters 
public class DocToPdfConverter : IFileConverter
{
    public void Convert(string fileName) 
        => Console.WriteLine($"[DocToPdfConverter] Converting {fileName} from DOC to PDF...");
}
public class JpgToPngConverter  : IFileConverter
{
    public void Convert(string fileName)
    {
        Console.WriteLine($"[JpgToPngConverter] Converting {fileName} from JPG to PNG...");
    }
}
public class CsvToJsonConverter  : IFileConverter
{
    public void Convert(string fileName)
    {
        Console.WriteLine($"[CsvToJsonConverter] Converting {fileName} from CSV to JSON...");
    }
}

public abstract class FileConverterFactory
{
// Factory Method
    protected abstract IFileConverter CreateConverter();
    public void ProcessConversion(string fileName)
    {
        var converter = CreateConverter();
        converter.Convert(fileName);
    }
}
public class DocToPdfFactory : FileConverterFactory
{
    protected override IFileConverter CreateConverter() => new DocToPdfConverter();
}
public class JpgToPngFactory : FileConverterFactory
{
    protected override IFileConverter CreateConverter() => new JpgToPngConverter();
}
public class CsvToJsonFactory : FileConverterFactory
{
    protected override IFileConverter CreateConverter() => new CsvToJsonConverter();
}
// Singleton 
public class ConverterManager
{
    private static ConverterManager _instance;
    private static readonly object _lock = new object(); // for thread safety
    private FileConverterFactory _currentFactory;
    private ConverterManager() { }
    public static ConverterManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ConverterManager();
                    }
                }
            }
            return _instance;
        }
    }
    public void SetConverterFactory(FileConverterFactory factory)
    {
        _currentFactory = factory;
    }
    public void ConvertFile(string fileName)
    {
        if (_currentFactory == null)
        {
            Console.WriteLine("No converter factory set. Please set a factory before converting.");
            return;
        }
        _currentFactory.ProcessConversion(fileName);
    }
}