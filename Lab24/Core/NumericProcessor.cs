namespace Lab24.Core;
public class NumericProcessor
{
    private INumericOperationStrategy _strategy;

    public NumericProcessor(INumericOperationStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(INumericOperationStrategy strategy)
    {
        _strategy = strategy;
    }
    public double Process(double input) => _strategy.Execute(input);
    public string CurrentOperation => _strategy.Name;
}