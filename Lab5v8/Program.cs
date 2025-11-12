//
public class InvalidPackageException : Exception
{
    public InvalidPackageException(string message) : base(message) { }
}

//Models
public enum Zone
{
    Center,
    North,
    South,
    East,
    West
}

// Клас Package — описує одну посилку
public class Package
{
    public string Id { get; }
    public string ProductName { get; }
    public decimal WeightKg { get; }
    public decimal Price { get; }
    public bool DeliveredOnTime { get; }
    public Zone Zone { get; }

    public Package(string id, string productName, decimal weight, decimal price, bool onTime, Zone zone)
    {
        if (string.IsNullOrWhiteSpace(id))throw new InvalidPackageException("Ідентифікатор посилки не може бути порожнім.");
        if (weight <= 0)throw new InvalidPackageException($"Посилка {id}: вага повинна бути більшою за 0.");
        if (price < 0) throw new InvalidPackageException($"Посилка {id}: ціна не може бути від’ємною.");

        Id = id;
        ProductName = productName;
        WeightKg = weight;
        Price = price;
        DeliveredOnTime = onTime;
        Zone = zone;
    }

    public override string ToString()
    {
        return $"{Id} | {ProductName} | {WeightKg} кг | {Price}₴ | {(DeliveredOnTime ? "вчасно" : "запізнилась")} | {Zone}";
    }
}

// Клас Delivery — складається з кількох Package (композиція)
public class Delivery
{
    public string DeliveryId { get; }
    public DateTime CreatedAt { get; }
    private readonly List<Package> _packages = new();

    public Delivery(string id)
    {
        DeliveryId = id;
        CreatedAt = DateTime.Now;
    }

    public void AddPackage(Package package)
    {
        _packages.Add(package);
    }

    public IReadOnlyList<Package> Packages => _packages;

    public decimal TotalWeight() => _packages.Sum(p => p.WeightKg);

    public decimal TotalPrice() => _packages.Sum(p => p.Price);

    public bool IsOnTime() => _packages.All(p => p.DeliveredOnTime);

    public Zone? GetZone()
    {
        var zones = _packages.Select(p => p.Zone).Distinct().ToList();
        return zones.Count == 1 ? zones[0] : null;
    }

    public override string ToString()
    {
        return $"Доставка {DeliveryId}: {_packages.Count} посилок, вага {TotalWeight()} кг, {(IsOnTime() ? "усі вчасно" : "є затримки")}";
    }
}

//   УЗАГАЛЬНЕНИЙ РЕПОЗИТОРІЙ

public interface IRepository<T>
{
    void Add(T item);
    IEnumerable<T> All();
    bool Remove(Predicate<T> predicate);
    T? Find(Func<T, bool> predicate);
}

public class Repository<T> : IRepository<T>
{
    private readonly List<T> _items = new();

    public void Add(T item) => _items.Add(item);

    public IEnumerable<T> All() => _items;

    public bool Remove(Predicate<T> predicate)
    {
        int removed = _items.RemoveAll(predicate);
        return removed > 0;
    }

    public T? Find(Func<T, bool> predicate) => _items.FirstOrDefault(predicate);
}

//   СТАТИСТИКА ТА АНАЛІТИКА
public static class DeliveryStatistics
{
    // Відсоток вчасно доставлених
    public static double SLA(IEnumerable<Delivery> deliveries)
    {
        var list = deliveries.ToList();
        if (!list.Any()) return 0;
        int onTime = list.Count(d => d.IsOnTime());
        return (double)onTime / list.Count * 100.0;
    }

    // Групування за зонами
    public static IEnumerable<(string Key, List<Delivery> Items)> GroupByZone(IEnumerable<Delivery> deliveries)
    {
        return deliveries.GroupBy(d => d.GetZone()?.ToString() ?? "Змішана").Select(g => (g.Key, g.ToList()));
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var repo = new Repository<Delivery>();

        try
        {
            // --- Створюємо доставки ---
            var d1 = new Delivery("D001");
            d1.AddPackage(new Package("P001", "Молоко", 2.0m, 60, true, Zone.Center));
            d1.AddPackage(new Package("P002", "Хліб", 1.0m, 30, true, Zone.Center));

            var d2 = new Delivery("D002");
            d2.AddPackage(new Package("P003", "М’ясо", 5.0m, 250, false, Zone.North));

            var d3 = new Delivery("D003");
            d3.AddPackage(new Package("P004", "Овочі", 3.5m, 100, true, Zone.North));
            d3.AddPackage(new Package("P005", "Фрукти", 2.5m, 120, true, Zone.West));

            repo.Add(d1);
            repo.Add(d2);
            repo.Add(d3);

            // --- Вивід усіх доставок ---
            Console.WriteLine("СПИСОК ДОСТАВОК:");
            foreach (var d in repo.All())
            {
                Console.WriteLine(d);
                foreach (var p in d.Packages)
                    Console.WriteLine($"   • {p}");
                Console.WriteLine();
            }

            // --- Статистика SLA ---
            double sla = DeliveryStatistics.SLA(repo.All());
            Console.WriteLine($"SLA (вчасно доставлених): {sla:F2}%\n");

            // --- Групування за зонами ---
            Console.WriteLine("ГРУПУВАННЯ ЗА ЗОНАМИ:");
            var groups = DeliveryStatistics.GroupByZone(repo.All());
            foreach (var g in groups)
            {
                Console.WriteLine($"Зона: {g.Key}");
                foreach (var d in g.Items)
                    Console.WriteLine($"   - {d.DeliveryId} ({d.Packages.Count} посилок)");
            }

            // --- Приклад пошуку та видалення ---
            Console.WriteLine("Пошук і видалення:");
            var found = repo.Find(d => d.DeliveryId == "D002");
            Console.WriteLine(found != null ? $"Знайдено {found.DeliveryId}" : "Не знайдено.");

            bool removed = repo.Remove(d => d.DeliveryId == "D002");
            Console.WriteLine(removed ? "D002 видалено." : "D002 не знайдено.");

        }
        catch (InvalidPackageException ex)
        {
            Console.WriteLine($"Помилка пакунку: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Неочікувана помилка: {ex.Message}");
        }

        Console.WriteLine("\nРоботу завершено. Натисніть будь-яку клавішу...");
        Console.ReadKey();
    }
}

