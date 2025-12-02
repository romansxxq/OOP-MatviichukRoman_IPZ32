class Program
{
    static void Main(string[] args)
    {
        int[] sizes = {1_000_000, 5_000_000, 10_000_000};
        Random rnd = new Random(42);
        foreach (var size in sizes)
        {
            Console.WriteLine($"Data size: {size:N0}");
            var data = GenerateDate(size, rnd);
            WarmUp(data);
            //LINQ
            var sw = System.Diagnostics.Stopwatch.StartNew();
            double sumLinq = data.Where(x => x > 500_000).Select(HeavyMathOperation).Sum();
            sw.Stop();
            long linqMs = sw.ElapsedMilliseconds;

            //PLINQ
            sw.Restart();
            double sumPlinq = data.AsParallel().Where(x => x > 500_000).Select(HeavyMathOperation).Sum();
            sw.Stop();
            long plinqMs = sw.ElapsedMilliseconds;
            Console.WriteLine($" | LINQ time: {linqMs} ms, Sum: {sumLinq:N2} |");
            Console.WriteLine($" | PLINQ time: {plinqMs} ms, Sum: {sumPlinq:N2} |");
            Console.WriteLine($" | Difference: {Math.Abs(linqMs - plinqMs):E6} ms |");
        }
        Console.WriteLine("\nSide Effects Demo:");
        SideEffectsDemo();
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
        
    }
    static List<double> GenerateDate(int size, Random rnd)
    {
        List<double> data = new List<double>(size);
        for (int i = 0; i < size; i++)
        {
            data.Add(rnd.NextDouble() * 1_000_000.0);
        }
        return data;
    }
    static void WarmUp(List<double> data)
    {
        _ = data.Take(1000).Sum();
        _ = data.AsParallel().Take(1000).Sum();
    }
    static double HeavyMathOperation(double x)
    {
        double t = Math.Sqrt(x+1);
        t += Math.Sin(t) * Math.Cos(t/2);
        t = Math.Log(t + 1);
        return t;
    }

    static void SideEffectsDemo()
    {
        var data = Enumerable.Range(1, 2_000_000).ToList();

        int expectedEven = data.Count(x => x % 2 == 0);
        Console.WriteLine($"Expected even numbers: {expectedEven:N0}");

        //інекремент unsafe
        int brokenCounter = 0;

        data.AsParallel().ForAll(x =>
        {
            if (x % 2 == 0)
                brokenCounter++;
        });

        Console.WriteLine($"Broken counter (no sync): {brokenCounter:N0}");

        //Правильно
        int safeCounter = 0;

        data.AsParallel().ForAll(x =>
        {
            if (x % 2 == 0)
                Interlocked.Increment(ref safeCounter);
        });

        Console.WriteLine($"Fixed counter (Interlocked): {safeCounter:N0}");
    }
}