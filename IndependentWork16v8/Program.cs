//Bad SRP

// public class ConfigurationManager
// {
//     public void LoadConfig(string filePath)
//     {
//         Console.WriteLine($"Loading configuration...");
//         Thread.Sleep(2000);
//         Console.WriteLine("Configuration loaded.");
//     }
//     public bool ValidateConfig()
//     {
//         Console.WriteLine($"Validating configuration...");
//         Thread.Sleep(1000);
//         Console.WriteLine("Configuration is valid.");
//         return true;
//     }
//     public void SaveConfig(string filePath)
//     {
//         Console.WriteLine($"Saving configuration...");
//         Thread.Sleep(500);
//         Console.WriteLine("Configuration saved.");
//     }
//     public void NotifyChanges()
//     {
//         Console.WriteLine($"Notifying about configuration changes...");
//         Thread.Sleep(200);
//         Console.WriteLine("Notification sent.");
//     }
//     public void ConfigurationService()
//     {
//         LoadConfig("config.json");
//         if (ValidateConfig())
//         {
//             SaveConfig("config.json");
//             NotifyChanges();
//         }
//     }
// }
// class Program
// {
//     static void Main(string[] args)
//     {
//         ConfigurationManager configManager = new ConfigurationManager();
//         configManager.ConfigurationService();
//     }
// }

// Refactored SRP

class Program 
{
    static void Main(string[] args)
    {
        //Connecting all the services together
        IConfigLoader loader = new ConfigLoader();
        IConfigValidator validator = new ConfigValidator();
        IConfigSaver saver = new ConfigSaver();
        IChangeNotifier notifier = new ChangeNotifier();
        //Creating the main service
        ConfigurationService configService = new ConfigurationService(loader, validator, saver, notifier);
        configService.ProcessConfiguration();
    }
}

public interface IConfigLoader
{
    public void Load();
}
public interface IConfigValidator
{
    public bool IsValid();
}
public interface IConfigSaver
{
    public void Save();
}
public interface IChangeNotifier
{
    public void Notify();
}
public class ConfigLoader : IConfigLoader
{
    public void Load()
    {
        Console.WriteLine($"Loading configuration...");
        Thread.Sleep(2000);
        Console.WriteLine("Configuration loaded.");
    }
}
public class ConfigValidator : IConfigValidator
{
    public bool IsValid()
    {
        Console.WriteLine($"Validating configuration...");
        Thread.Sleep(1000);
        Console.WriteLine("Configuration is valid.");
        return true;
    }
}
public class ConfigSaver : IConfigSaver
{
    public void Save()
    {
        Console.WriteLine($"Saving configuration...");
        Thread.Sleep(500);
        Console.WriteLine("Configuration saved.");
    }
}
public class ChangeNotifier : IChangeNotifier
{
    public void Notify()
    {
        Console.WriteLine($"Notifying about configuration changes...");
        Thread.Sleep(200);
        Console.WriteLine("Notification sent.");
    }
}
public class ConfigurationService
{
    private readonly IConfigLoader _loader;
    private readonly IConfigValidator _validator;
    private readonly IConfigSaver _saver;
    private readonly IChangeNotifier _notifier;

    public ConfigurationService(IConfigLoader loader, IConfigValidator validator, IConfigSaver saver, IChangeNotifier notifier)
    {
        _loader = loader;
        _validator = validator;
        _saver = saver;
        _notifier = notifier;
    }

    public void ProcessConfiguration()
    {
        _loader.Load();
        if (_validator.IsValid())
        {
            _saver.Save();
            _notifier.Notify();
        }
    }
}