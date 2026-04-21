// public class Program
// {
//     public static void Main(string[] args)
//     {

//         // Create DataContext and DataPublisher
//         DataContext context = new DataContext();
//         DataPublisher publisher = new DataPublisher();

//         // Create observers and subscribe them to DataProcessed event
//         ConsoleLoggerObserver consoleLogger = new ConsoleLoggerObserver();
//         FileLoggerObserver fileLogger = new FileLoggerObserver();
//         AnalyticsSenderObserver analyticsSender = new AnalyticsSenderObserver();

//         publisher.DataProcessed += consoleLogger.OnDataProcessed;
//         publisher.DataProcessed += fileLogger.OnDataProcessed;
//         publisher.DataProcessed += analyticsSender.OnDataProcessed;

//         // Set different strategies and process different data samples
//         List<(IDataProcessorStrategy strategy, string data)> scenarios = new()
//         {
//             (new EncryptDataStrategy(), "User credentials"),
//             (new CompressDataStrategy(), "Large report document"),
//             (new LogDataStrategy(), "System event log")
//         };

//         foreach ((IDataProcessorStrategy strategy, string data) in scenarios)
//         {
//             Console.WriteLine();
//             Console.WriteLine($"> Strategy: {strategy.GetType().Name}");
//             context.SetStrategy(strategy);
//             context.ExecuteStrategy(data);

//             // Notify all observers that processing is complete
//             publisher.PublishDataProcessed(data);
//         }

//         Console.WriteLine();
//         Console.WriteLine("=== Demo completed successfully ===");
//     }
// }
// // Strategies for data processing
// public interface IDataProcessorStrategy
// {
//     void ProcessData(string data);
// }

// public class EncryptDataStrategy : IDataProcessorStrategy
// {
//     public void ProcessData(string data)
//     {
//         //imitation of data encryption
//         Console.WriteLine($"Encrypting data: {data}");
//     }
// }
// public class CompressDataStrategy : IDataProcessorStrategy
// {
//     public void ProcessData(string data)
//     {
//         //imitation of data compression
//         Console.WriteLine($"Compressing data: {data}");
//     }
// }
// public class LogDataStrategy : IDataProcessorStrategy
// {
//     public void ProcessData(string data)
//     {
//         //imitation of data logging
//         Console.WriteLine($"Logging data: {data}");
//     }
// }
// // Context class that uses the strategy
// public class DataContext
// {
//     private IDataProcessorStrategy? _strategy;

//     public void SetStrategy(IDataProcessorStrategy strategy)
//     {
//         _strategy = strategy;
//     }

//     public void ExecuteStrategy(string data)
//     {
//         if (_strategy == null)
//         {
//             Console.WriteLine("No strategy set.");
//             return;
//         }
//         _strategy.ProcessData(data);
//     }
// }
// // Observers
// public class DataPublisher
// {
//     public event Action<string>? DataProcessed;

//     public void PublishDataProcessed(string data)
//     {
//         DataProcessed?.Invoke(data);
//     }
// }
// public class ConsoleLoggerObserver
// {
//     public void OnDataProcessed(string data)
//     {
//         Console.WriteLine($"Console Logger: Data processed - {data}");
//     }
// }
// public class FileLoggerObserver
// {
//     public void OnDataProcessed(string data)
//     {
//         //imitation of logging to a file
//         Console.WriteLine($"File Logger: Data processed - {data}");
//     }
// }
// public class AnalyticsSenderObserver
// {
//     public void OnDataProcessed(string data)
//     {
//         //imitation of sending data to analytics
//         Console.WriteLine($"Analytics Sender: Data processed - {data}");
//     }
// }

class Program {
    static void Main(string[] args)
    {
        // Create Publisher and Observers
        DataPublisher publisher = new DataPublisher();
        CalendarSyncObserver calendarSync = new CalendarSyncObserver();
        NotificationSenderObserver notificationSender = new NotificationSenderObserver();

        // Subscribe observers to the publisher's event
        publisher.DataProcessed += calendarSync.OnDataProcessed;
        publisher.DataProcessed += notificationSender.OnDataProcessed;

        // Create DataContext and set different strategies
        DataContext context = new DataContext(new CreateEventStrategy());
        // Demo: Create Event
        var eventData = "Event: Team Meeting at 10 AM";
        context.ExecuteProcessing(eventData);
        publisher.PublishDataProcessed(eventData);
        Console.WriteLine();
        // Demo 2: Change strategy to UpdateEvent
        context.SetStrategy(new UpdateEventStrategy());
        var eventData2 = "Event: Update Team Meeting to 11 AM";
        context.ExecuteProcessing(eventData2);
        publisher.PublishDataProcessed(eventData2);
        Console.WriteLine();
        // Demo 3: Change strategy to DeleteEvent
        context.SetStrategy(new DeleteEventStrategy());
        var eventData3 = "Event: Delete Team Meeting";
        context.ExecuteProcessing(eventData3);
        publisher.PublishDataProcessed(eventData3);
        Console.WriteLine();

    }
}

public interface IDataProcessorStrategy
{
    void ProcessData(string data);
}

public class CreateEventStrategy : IDataProcessorStrategy
{
    public void ProcessData(string data)
    {
        Console.WriteLine($"[Strategy: CreateEvent] Creating event with data: {data}");
    }
}

public class UpdateEventStrategy : IDataProcessorStrategy
{
    public void ProcessData(string data)
    {
        Console.WriteLine($"[Strategy: UpdateEvent] Updating event with data: {data}");
    }
}

public class DeleteEventStrategy : IDataProcessorStrategy
{
    public void ProcessData(string data)
    {
        Console.WriteLine($"[Strategy: DeleteEvent] Deleting event with data: {data}");
    }
}

public class DataContext
{
    private IDataProcessorStrategy? _strategy;

    public DataContext(IDataProcessorStrategy strategy)
    {
        _strategy = strategy;
    }
    public void SetStrategy(IDataProcessorStrategy strategy)
    {
        _strategy = strategy;
    }

    public void ExecuteProcessing(string data)
    {
        if (_strategy == null)
        {
            Console.WriteLine("No strategy set.");
            return;
        }
        _strategy.ProcessData(data);
    }
}

// Observers
public class DataPublisher
{
    public event Action<string>? DataProcessed;

    public void PublishDataProcessed(string data)
    {
        Console.WriteLine($"[Publisher] Data processed: {data}");
        DataProcessed?.Invoke(data);
    }
}

public class CalendarSyncObserver
{
    public void OnDataProcessed(string data)
    {
        Console.WriteLine($"[Observer: CalendarSync] Syncing calendar with data: {data}");
    }
}

public class NotificationSenderObserver
{
    public void OnDataProcessed(string data)
    {
        Console.WriteLine($"[Observer: NotificationSender] Sending notification with data: {data}");
    }
}

