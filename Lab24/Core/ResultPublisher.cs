namespace Lab24.Core;

public class ResultPublisher
{
    public event Action<double, string>? ResultCalculated;

    public void PublishResult(double result, string operationName)
    {
        ResultCalculated?.Invoke(result, operationName);
    }
}