// Interface defining power usage methods
public interface IPowerUsage
{
    // Calculate power usage per day in kWh
    double CalculatePowerUsagePerDay();
    // Calculate power usage per week in kWh
    double CalculatePowerUsagePerWeek();
    string ShowInfo();
}
// Abstract base class for devices
public abstract class Device : IPowerUsage
{
    protected string Name { get; }
    protected double PowerRating { get; } // in watts
    protected double HoursUsedPerDay { get; }
    // Constructor
    public Device(string name, double powerRating, double hoursUsedPerDay)
    {
        Name = name;
        PowerRating = powerRating;
        HoursUsedPerDay = hoursUsedPerDay;
    }
    // Abstract method to calculate daily power usage
    public abstract double CalculatePowerUsagePerDay();
    // Default implementation for weekly usage
    public double CalculatePowerUsagePerWeek()
    {
        return CalculatePowerUsagePerDay() * 7;
    }
    public virtual string ShowInfo()
    {
        return $"Device: {Name}\nPower Rating: {PowerRating}W\nDaily Usage: {CalculatePowerUsagePerDay()}Wh";
    }

}
// Laptop class inheriting from Device
public class Laptop : Device
{
// Indicates if the laptop is a gaming laptop
    public bool IsGaming { get; }
    public Laptop(string name, double powerRating, double hoursUsedPerDay, bool isGaming)
        : base(name, powerRating, hoursUsedPerDay)
    {
        IsGaming = isGaming;
    }
    public override double CalculatePowerUsagePerDay()
    {
    // Gaming laptops consume more power
        double gamingMultiplier = IsGaming ? 1.5 : 1.0;
        return (PowerRating * HoursUsedPerDay * gamingMultiplier) / 1000.0; // Convert to kWh
    }
    public override string ShowInfo()
    {
        return base.ShowInfo() + $"\nType: {(IsGaming ? "Gaming Laptop" : "Regular Laptop")}";
    }
}
public class Lamp : Device
{
    // Type of lamp (e.g., LED, Incandescent)
    public string LampType { get; }
    public Lamp(string name, double powerRating, double hoursUsedPerDay, string lampType)
        : base(name, powerRating, hoursUsedPerDay)
    {
        LampType = lampType;
    }
    public override double CalculatePowerUsagePerDay()
    {
        return (PowerRating * HoursUsedPerDay) / 1000.0; // Convert to kWh
    }
    public override string ShowInfo()
    {
        return base.ShowInfo() + $"\nLamp Type: {LampType}";
    }
}
// class PowerCalculator (compostion)
public class PowerCaluculator
{
    private readonly List<IPowerUsage> devices = new();
    public void AddDevice(IPowerUsage device)
    {
        devices.Add(device);
    }
    public double CalculateTotalWeeklyUsage()
    {
        double total = 0;
        foreach (var device in devices)
        {
            total += device.CalculatePowerUsagePerWeek();
        }
        return total;
    }
    public double CalculateTotalDailyUsage()
    {
        double total = 0;
        foreach (var device in devices)
        {
            total += device.CalculatePowerUsagePerDay();
        }
        return total;
    }
    public void ShowReport()
    {
        Console.WriteLine("Power Usage Report:");
        foreach (var device in devices)
        {
        // Add an extra line break for Device types
            if (device is Device)
            {
                Console.WriteLine();
            }
            Console.WriteLine(device.ShowInfo());
            Console.WriteLine();
        }
        Console.WriteLine($"Total Daily Usage: {CalculateTotalDailyUsage()} kWh");
        Console.WriteLine($"Total Weekly Usage: {CalculateTotalWeeklyUsage()} kWh");
    }
}
internal class Program
{
    private static void Main(string[] args)
    {
    // Create instances of devices
        var laptop = new Laptop("Lenovo", 150, 5, true);
        var lamp = new Lamp("Desk Lamp", 60, 8, "LED");

        var powerCalculator = new PowerCaluculator();
        powerCalculator.AddDevice(laptop);
        powerCalculator.AddDevice(lamp);

        powerCalculator.ShowReport();

        Console.ReadLine();
    }
}