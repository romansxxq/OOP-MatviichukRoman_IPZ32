namespace Lab24.Core;
public interface INumericOperationStrategy
{
    double Execute(double value);
    string Name { get; }
}
