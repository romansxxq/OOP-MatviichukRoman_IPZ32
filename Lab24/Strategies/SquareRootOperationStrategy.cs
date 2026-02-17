using System;
using Lab24.Core;
namespace Lab24.Strategies;

public class SquareRootOperationStrategy : INumericOperationStrategy
{
    public string Name => "Square Root";
    public double Execute(double value) => Math.Sqrt(value);
}
