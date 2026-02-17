using System.Collections.Generic;

namespace Lab24.Observers;

public class HistoryLoggerObserver
{
    public List<string> History { get; } = new();

    public void OnResult(double result, string operationName)
    {
        History.Add($"{operationName}: {result}");
    }
}
