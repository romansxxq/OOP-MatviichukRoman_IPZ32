using System;
using Lab24.Core;
using Lab24.Observers;
using Lab24.Strategies;

namespace Lab24;

class Program
{
    static void Main()
    {
        var publisher = new ResultPublisher();

        var consoleObs = new ConsoleLoggerObserver();
        var historyObs = new HistoryLoggerObserver();
        var thresholdObs = new ThresholdNotifierObserver(20);

        publisher.ResultCalculated += consoleObs.OnResult;
        publisher.ResultCalculated += historyObs.OnResult;
        publisher.ResultCalculated += thresholdObs.OnResult;

        var processor = new NumericProcessor(new SquareOperationStrategy());

        double input = 5;

        Run(processor, publisher, input);

        processor.SetStrategy(new CubeOperationStrategy());
        Run(processor, publisher, input);

        processor.SetStrategy(new SquareRootOperationStrategy());
        Run(processor, publisher, input);

        Console.WriteLine("\nHistory:");
        foreach (var item in historyObs.History)
            Console.WriteLine(item);
    }

    static void Run(NumericProcessor processor, ResultPublisher publisher, double input)
    {
        var result = processor.Process(input);
        publisher.PublishResult(result, processor.CurrentOperation);
    }
}
