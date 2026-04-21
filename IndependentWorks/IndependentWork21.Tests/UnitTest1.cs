using System;
using System.Collections.Generic;
using Xunit;

namespace IndependentWork21.Tests;

// Strategy
public interface IEventStrategy
{
    string Process(string data);
}

public class CreateStrategy : IEventStrategy
{
    public string Process(string data) =>
        $"[Create] Event created with data: {data}";
}
public class UpdateStrategy : IEventStrategy
{
    public string Process(string data) =>
        $"[Update] Event updated with data: {data}";
}

// Observer (виправлена опечатка в інтерфейсі ILoggerObserver)
public interface ILoggerObserver
{
    List<string> Logs { get; }
    void OnEventProcessed(string message);
}

public class MemoryLoggerObserver : ILoggerObserver
{
    public List<string> Logs { get; } = new List<string>();
    public void OnEventProcessed(string message)
    {
        Logs.Add(message);
    }
}

// Factory Method
public static class LoggerFactory
{
    public static ILoggerObserver CreateLogger() => new MemoryLoggerObserver();
}

// Singleton + Context + Publisher
public class EventManager
{
    private static EventManager? _instance;
    private IEventStrategy? _strategy;

    // Singleton instance
    public static EventManager Instance => _instance ??= new EventManager();

    private EventManager() { }

    public static void ResetInstance() => _instance = new EventManager();

    // Observer event
    public event Action<string>? EventProcessed;

    public void SetStrategy(IEventStrategy strategy)
    {
        _strategy = strategy;
    }
    
    public void ProcessEvent(string data)
    {
        if (_strategy == null)
            throw new InvalidOperationException("Strategy not set.");
            
        // Використовуємо IsNullOrWhiteSpace, щоб покрити тест з пробілами "   "
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Data cannot be null or empty.");

        var result = _strategy.Process(data);
        EventProcessed?.Invoke(result); // Виправлено назву події
    }
}

// Integration tests
public class PatternIntegrationTests : IDisposable
{
    public PatternIntegrationTests()
    {
        // Reset singleton instance before each test
        EventManager.ResetInstance();
    }

    // ДОДАНО: реалізація IDisposable
    public void Dispose()
    {
        EventManager.ResetInstance();
    }

    // Positive scenarios
    [Fact]
    public void Scenario1_FullIntegration_CreateEventAndNotifiesObserver()
    {
        // Arrange
        var manager = EventManager.Instance;
        var logger = LoggerFactory.CreateLogger();
        manager.EventProcessed += logger.OnEventProcessed;
        manager.SetStrategy(new CreateStrategy());

        // Act
        manager.ProcessEvent("EventData1");

        // Assert
        Assert.Single(logger.Logs);
        Assert.Equal("[Create] Event created with data: EventData1", logger.Logs[0]);
    }
    
    [Fact]
    public void Scenario2_RuntimeStrategyChange_UpdatesBehaviorCorrectly()
    {
        // Arrange
        var manager = EventManager.Instance;
        var logger = LoggerFactory.CreateLogger(); // Виправлено назву методу
        manager.EventProcessed += logger.OnEventProcessed; // Виправлено назву події

        // Act 1 - Create
        manager.SetStrategy(new CreateStrategy());
        manager.ProcessEvent("Event A");
        
        // Act 2 - Update (зміна стратегії в рантаймі)
        manager.SetStrategy(new UpdateStrategy());
        manager.ProcessEvent("Event A");
        
        // Assert
        Assert.Equal(2, logger.Logs.Count);
        Assert.Equal("[Create] Event created with data: Event A", logger.Logs[0]);
        Assert.Equal("[Update] Event updated with data: Event A", logger.Logs[1]);
    }
    
    [Fact]
    public void Scenario3_MultipleObservers_AllReceiveNotifications()
    {
        // Arrange
        var manager = EventManager.Instance;
        var logger1 = LoggerFactory.CreateLogger(); 
        var logger2 = LoggerFactory.CreateLogger(); 

        manager.EventProcessed += logger1.OnEventProcessed; 
        manager.EventProcessed += logger2.OnEventProcessed;
        manager.SetStrategy(new CreateStrategy());
        
        // Act
        manager.ProcessEvent("Global Event");
        
        // Assert
        Assert.Single(logger1.Logs);
        Assert.Single(logger2.Logs);
        Assert.Equal("[Create] Event created with data: Global Event", logger1.Logs[0]);
        Assert.Equal("[Create] Event created with data: Global Event", logger2.Logs[0]);
    }

    // Negative scenarios
    [Fact]
    public void Scenario4_ProcessWithoutStrategy_ThrowsInvalidOperationException()
    {
        // Arrange
        var manager = EventManager.Instance;
        
        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => manager.ProcessEvent("Test"));
        Assert.Equal("Strategy not set.", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Scenario5_ProcessInvalidData_ThrowsArgumentException(string invalidData)
    {
        // Arrange
        var manager = EventManager.Instance;
        manager.SetStrategy(new CreateStrategy());
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => manager.ProcessEvent(invalidData));
        Assert.Equal("Data cannot be null or empty.", ex.Message);
    }
}