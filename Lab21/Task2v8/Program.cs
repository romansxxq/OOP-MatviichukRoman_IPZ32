
public static class Program
{
    public static void Main(string[] args)
    {
        System.Console.WriteLine("Type of insurance (Basic, Extended, FullCoverage, PremiumCoverage): ");
        string? input = Console.ReadLine() ?? "";

        var isValid = Enum.GetValues(typeof(InsuranceType))
            .Cast<InsuranceType>()
            .Any(t => t.ToString().Equals(input, StringComparison.OrdinalIgnoreCase));
        
        if (!isValid)
        {
            Console.WriteLine("Invalid insurance type.");
            return;
        }

        InsuranceType insuranceType = (InsuranceType)Enum.Parse(typeof(InsuranceType), input, true);

        Console.WriteLine("Driver Age: ");
        string? ageInput = Console.ReadLine() ?? "0";

        System.Console.WriteLine("Driver Experience (in years): ");
        string? experienceInput = Console.ReadLine() ?? "0";

        Console.WriteLine("Price of the car (in USD): ");
        string? priceCarInput = Console.ReadLine() ?? "0";

        IInsuranceStrategy strategy = InsuranceStrategyFactory.CreateInsuranceStrategy(insuranceType);
        InsuranceCalculator calculator = new InsuranceCalculator();

        decimal insuranceCost = calculator.CalculateInsurance(
            int.Parse(ageInput),
            int.Parse(experienceInput),
            decimal.Parse(priceCarInput),
            strategy);
        
        Console.WriteLine($"The insurance cost is: {insuranceCost} USD.");
    }
}
public enum InsuranceType
{
    Basic,
    Extended,
    FullCoverage,
    PremiumCoverage
}
public interface IInsuranceStrategy
{
    decimal CalculateInsurance(int driverAge, int driverExperience, decimal priceAuto);
}

public class BasicInsuranceStrategy : IInsuranceStrategy
{
    public decimal CalculateInsurance(int driverAge, int driverExperience, decimal priceCar)
    {
        decimal ageFactor = driverAge < 22 ? 1.5m : 1.0m;
        decimal experienceFactor = driverExperience < 2 ? 1.2m : 1.0m;
        decimal baseRate = 0.02m;
        return priceCar * baseRate * ageFactor * experienceFactor;
    }
}
public class ExtendedInsuranceStrategy : IInsuranceStrategy
{
    public decimal CalculateInsurance(int driverAge, int driverExperience, decimal priceCar)
    {
        decimal ageFactor = driverAge < 25 ? 1.0m : 1.0m;
        decimal experienceFactor = driverExperience < 2 ? 1.3m : 1.0m;
        decimal baseRate = 0.03m;
        return priceCar * baseRate * ageFactor * experienceFactor;
    }
}
public class FullCoverageStrategy : IInsuranceStrategy
{
    public decimal CalculateInsurance(int driverAge, int driverExperience, decimal priceCar)
    {
        decimal ageFactor = driverAge < 30 ? 1.7m : 1.0m;
        decimal experienceFactor = driverExperience < 5 ? 1.5m : 1.0m;
        decimal baseRate = 0.05m;
        return priceCar * baseRate * ageFactor * experienceFactor;
    }
}
public class PremiumCoverageStrategy : IInsuranceStrategy
{
    public decimal CalculateInsurance(int driverAge, int driverExperience, decimal priceCar)
    {
        decimal ageFactor = driverAge < 35 ? 2.0m : 1.0m;
        decimal experienceFactor = driverExperience < 10 ? 1.7m : 1.0m;
        decimal baseRate = 0.07m;
        return priceCar * baseRate * ageFactor * experienceFactor;
    }
}

public static class InsuranceStrategyFactory
{
    public static IInsuranceStrategy CreateInsuranceStrategy(InsuranceType insuranceType)
    {
        return insuranceType switch
        {
            InsuranceType.Basic => new BasicInsuranceStrategy(),
            InsuranceType.Extended => new ExtendedInsuranceStrategy(),
            InsuranceType.FullCoverage => new FullCoverageStrategy(),
            InsuranceType.PremiumCoverage => new PremiumCoverageStrategy(),
            _ => throw new ArgumentException("Invalid insurance type")
        };
    }
}

public class InsuranceCalculator
{
    public decimal CalculateInsurance(
        int driverAge,
        int driverExperience,
        decimal priceCar,
        IInsuranceStrategy strategy)
        => strategy.CalculateInsurance(driverAge, driverExperience, priceCar);
}