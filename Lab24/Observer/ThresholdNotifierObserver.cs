namespace Lab24.Observers;

public class ThresholdNotifierObserver
{
    private readonly double _threshold;

    public ThresholdNotifierObserver(double threshold)
    {
        _threshold = threshold;
    }

    public void OnResult(double result, string operationName)
    {
        if (result > _threshold)
            Console.WriteLine($"Threshold: {operationName} result {result} > {_threshold}");
    }
}
