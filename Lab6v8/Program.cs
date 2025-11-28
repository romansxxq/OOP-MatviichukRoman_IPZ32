class Program
{
    
    static void Main(string[] args)
    {
        // Колекція для демонстрації
        List<int> numbers = new() { 2, 3, 4, 5, 10, 13, 17, 18, 19, 20, 23, 25 };

        //Func<int, int, bool> – перевірка простого числа
        Func<int, int, bool> isPrimeFunc = (num, _) =>
        {
            if (num < 2) return false;
            for (int i = 2; i * i <= num; i++)
            {
                if (num % i == 0) return false;
            }
            return true;
        };
        //Action<List<int>> – друк списку
        Action<List<int>> printListAction = list =>
        {
            Console.WriteLine("List elements:");
            list.ForEach(n => Console.Write(n + " "));
            Console.WriteLine("\n");
        };
        //Predicate<int> – правило видалення "непотрібних"
        Predicate<int> removeUnwantedPredicate = n => n > 20;
        // Друкуємо початковий список
        printListAction(numbers);
        // Фільтруємо прості числа через Func
        var primes = numbers.Where(n => isPrimeFunc(n, 0)).ToList();
        Console.WriteLine("Prime numbers:");
        printListAction(primes);
        //Видаляємо непотрібні числа через Predicate
        numbers.RemoveAll(n => removeUnwantedPredicate(n));
        Console.WriteLine("After removing unwanted (>20):");
        printListAction(numbers);
        //Використання LINQ Aggregate (бонус із критеріїв) – добуток всіх чисел
        int multiplyAll = numbers.Aggregate(1, (acc, n) => acc * n);
        Console.WriteLine($"Multiply all remaining numbers = {multiplyAll}");
        Console.WriteLine("Done!");
    }
}
