class Program
{
    
    static void Main(string[] args)
    {
        Func<int, int, bool> isPrimeNumCheck = (num, divisor) =>
        {
            if (divisor <= 1) return true;
            if (num % divisor == 0) return false;
            return isPrimeNumCheck(num, divisor - 1);
        };
        Console.WriteLine("Hello, World!");
    }
}