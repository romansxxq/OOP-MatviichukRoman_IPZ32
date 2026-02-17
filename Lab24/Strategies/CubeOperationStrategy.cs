using Lab24.Core;

namespace Lab24.Strategies;

public class CubeOperationStrategy : INumericOperationStrategy
{
    public string Name => "Cube";
    public double Execute(double value) => value * value * value;
}
