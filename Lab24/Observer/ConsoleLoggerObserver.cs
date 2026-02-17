using System;
namespace Lab24.Observers;

public class ConsoleLoggerObserver
{
    public void OnResult(double result, string operationName)
    {
        Console.WriteLine($"[LOG] {operationName}: {result}");
    }
}
