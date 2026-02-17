using Lab24.Core;
namespace Lab24.Strategies;
public class SquareOperationStrategy : INumericOperationStrategy
{
    public string Name => "Square";
    public double Execute(double value) => value * value;
}
