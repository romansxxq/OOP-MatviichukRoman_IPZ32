//Власний виняток
public class MissingRateException : Exception
{
    public MissingRateException(string from, string to) : base($"Курс між {from} і {to} не знайдено.") { }
}

//   Універсальний клас Maybe<T>
public class Maybe<T>
{
    private readonly T? _value;
    public bool HasValue { get; }

    public T Value => HasValue ? _value! : throw new InvalidOperationException("Значення відсутнє.");

    public Maybe() 
    { 
        _value = default;
        HasValue = false; 
    }

    public Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    public override string ToString() => HasValue ? _value?.ToString() ?? "null" : "None";

    public static implicit operator Maybe<T>(T value) => new(value);
}

//   Модель курсів валют
public class Rate
{
    public string From { get; }
    public string To { get; }
    public decimal Value { get; }

    public Rate(string from, string to, decimal value)
    {
        if (value <= 0) throw new ArgumentException("Курс має бути більшим за 0.");

        From = from.ToUpper();
        To = to.ToUpper();
        Value = value;
    }
    public override string ToString() => $"{From} → {To} = {Value}";
}
    // Таблиця курсів (композиція: містить список Rate)
public class RateTable
{
    private readonly List<Rate> _rates = new();
    public void AddRate(Rate rate) => _rates.Add(rate);
    public Maybe<decimal> GetRate(string from, string to)
    {
        var r = _rates.FirstOrDefault(x => x.From == from.ToUpper() && x.To == to.ToUpper());
        return r is null ? new Maybe<decimal>() : new Maybe<decimal>(r.Value);
    }

    public IEnumerable<Rate> All() => _rates;
}

// =============================== //
//   КОНВЕРТЕР ВАЛЮТ
// =============================== //
public class Converter
{
    private readonly RateTable _table;

    public Converter(RateTable table)
    {
        _table = table;
    }

    // Конвертація просто (один курс)
    public decimal Convert(string from, string to, decimal amount)
    {
        var rate = _table.GetRate(from, to);
        if (!rate.HasValue) throw new MissingRateException(from, to);
        return Math.Round(amount * rate.Value, 2);
    }

    // Конвертація за ланцюгом: наприклад, UAH→USD→EUR
    public decimal ConvertChain(decimal amount, params string[] chain)
    {
        if (chain.Length < 2) throw new ArgumentException("Ланцюг повинен містити мінімум дві валюти.");

        decimal result = amount;
        for (int i = 0; i < chain.Length - 1; i++)
        {
            string from = chain[i];
            string to = chain[i + 1];
            var rate = _table.GetRate(from, to);

            if (!rate.HasValue) throw new MissingRateException(from, to);

            result *= rate.Value;
        }

        return Math.Round(result, 2);
    }

    // Ефективний курс між першою і останньою валютою
     public decimal EffectiveRate(params string[] chain)
    {
        if (chain.Length < 2) throw new ArgumentException("Мінімум дві валюти для ефективного курсу.");

        decimal rate = 1m;
        for (int i = 0; i < chain.Length - 1; i++)
        {
            var r = _table.GetRate(chain[i], chain[i + 1]);
            if (!r.HasValue) throw new MissingRateException(chain[i], chain[i + 1]);
            rate *= r.Value;
        }
        return Math.Round(rate, 4);
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("Курси валют:");

        var table = new RateTable();

        // Додаємо курси (UAH, USD, EUR, GBP)
        table.AddRate(new Rate("UAH", "USD", 0.024m));
        table.AddRate(new Rate("USD", "EUR", 0.91m));
        table.AddRate(new Rate("EUR", "GBP", 0.85m));
        table.AddRate(new Rate("USD", "UAH", 41.2m));
        table.AddRate(new Rate("EUR", "USD", 1.1m));

        // Вивід таблиці
        Console.WriteLine("Таблиця курсів:");
        foreach (var r in table.All())
            Console.WriteLine($" • {r}");
        Console.WriteLine();

        var conv = new Converter(table);

        try
        {
            // Просте перетворення
            decimal uahToUsd = conv.Convert("UAH", "USD", 1000m);
            Console.WriteLine($"1000 UAH → USD = {uahToUsd}");

            // Конвертація за ланцюгом: UAH→USD→EUR
            decimal uahToEur = conv.ConvertChain(1000m, "UAH", "USD", "EUR");
            Console.WriteLine($"1000 UAH → USD → EUR = {uahToEur}");

            // Ефективний курс для ланцюга
            decimal effRate = conv.EffectiveRate("UAH", "USD", "EUR");
            Console.WriteLine($"Ефективний курс UAH→EUR через USD = {effRate}");

            // Спроба неіснуючого курсу
            Console.WriteLine();
            conv.Convert("UAH", "BTC", 100); // викличе MissingRateException
        }
        catch (MissingRateException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Неочікувана помилка: {ex.Message}");
        }

        Console.WriteLine("\nРоботу завершено. Натисніть будь-яку клавішу...");
        Console.ReadKey();
    }
}