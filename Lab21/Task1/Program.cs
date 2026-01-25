public static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Type of shipping (Standard, Express, International, Night): ");
        string? input = Console.ReadLine() ?? "";
        // Validate input
        var isValid = Enum.GetValues(typeof(ShippingType))
            .Cast<ShippingType>()
            .Any(t => t.ToString().Equals(input, StringComparison.OrdinalIgnoreCase));

        if (!isValid)
        {
            Console.WriteLine("Invalid shipping type.");
            return;
        }
        // Convert input to ShippingType enum
        ShippingType shippingType = (ShippingType)Enum.Parse(typeof(ShippingType), input, true);

        Console.WriteLine("Distance (in km): ");
        string? distanceInput = Console.ReadLine() ?? "0";
        decimal distance = decimal.Parse(distanceInput);

        Console.WriteLine("Weight (in kg): ");
        string ? weightInput = Console.ReadLine() ?? "0";
        decimal weight = decimal.Parse(weightInput);

        IShippingStrategy strategy = ShippingStretegyFactory.CreateShippingStrategy(shippingType);

        DeliveryService service = new DeliveryService();
        decimal cost = service.CalculateDeliverycost(distance, weight, strategy);
        Console.WriteLine($"The delivery cost is: {cost} USD.");
    }
}
public enum ShippingType
{
    Standard,
    Express,
    International,
    Night
}
public interface IShippingStrategy
{
    decimal CalculateCost(decimal distance, decimal weight);
}
public class StandardShippingStrategy : IShippingStrategy
{
    public decimal CalculateCost(decimal distance, decimal weight) 
        => distance * 2.5m + weight * 0.5m;
}
public class ExpressShippingStrategy : IShippingStrategy
{
    public decimal CalculateCost(decimal distance, decimal weight)
        => distance * 2.5m + weight * 1.0m + 50.0m;
}
public class InternationalShippingStrategy : IShippingStrategy
{
    public decimal CalculateCost(decimal distance, decimal weight)
        => distance * 5.0m + weight * 2.0m + 0.15m;
}
public class NightShippingStrategy : IShippingStrategy
{
    public decimal CalculateCost(decimal distance, decimal weight)
        => distance * 3.0m + weight * 1.5m + 100.0m;
}
public class ShippingStretegyFactory
{
    public static IShippingStrategy CreateShippingStrategy(ShippingType shippingType)
    {
        return shippingType switch
        {
            ShippingType.Standard => new StandardShippingStrategy(),
            ShippingType.Express => new ExpressShippingStrategy(),
            ShippingType.International => new InternationalShippingStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(shippingType), "Invalid shipping type")
        };
    }
}
public class DeliveryService
{
    public decimal CalculateDeliverycost(
    decimal distance,
    decimal weight,
    IShippingStrategy strategy)
    {
        return strategy.CalculateCost(distance, weight);
    }
}
