public class Program
{
    public static void Main(string[] args)
    {
        // Using Bad SRP
        System.Console.WriteLine("Demonstrating Bad SRP:");
        var badProcessor = new OrderProcessor();
        var order1 = new Order(1, "Alice", 100);
        badProcessor.ProcessOrder(order1);

        // Using Good SRP
        System.Console.WriteLine("\nDemonstrating Good SRP:");
        IOrderValidator validator = new OrderValidator();
        IOrderRepository repository = new InMemoryOrderRepository();
        IEmailService emailService = new ConsoleEmailService();
        OrderService orderService = new OrderService(validator, repository, emailService);
        //Valid order
        Order validOrder = new Order(2, "Bob", 150);
        orderService.ProcessOrder(validOrder);
        //Invalid order
        Order invalidOrder = new Order(3, "Charlie", -50);
        orderService.ProcessOrder(invalidOrder);

        Console.ReadKey();

    }
}
public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Order(int id, string customerName, decimal totalAmount)
    {
        Id = id;
        CustomerName = customerName;
        TotalAmount = totalAmount;
        Status = OrderStatus.New;
    }
}
public enum OrderStatus
{
    New,
    PendingValidation,
    Processed,
    Shipped,
    Delivered,
    Cancelled
}

// Bad SRP
public class OrderProcessor {
    public void ProcessOrder(Order order)
    {
        if (order.TotalAmount <= 0)
        {
            Console.WriteLine("Order validation failed.");
            return;
        }
        //Save
        Console.WriteLine($"Order {order.Id} saved to database.");
        //Email
        Console.WriteLine($"Email sent to {order.CustomerName}.");
        //Update status
        order.Status = OrderStatus.Processed;
        Console.WriteLine($"Order {order.Id} processed.");
    }
}

public interface IOrderValidator
{
    bool IsValid(Order order);
}
public interface IOrderRepository
{
    void Save(Order order);
    Order? GetById(int id);
}
public interface IEmailService
{
    void SendOrderConfirmation(Order order);
}

public class OrderValidator : IOrderValidator
{
    public bool IsValid(Order order)
    {
        return order.TotalAmount > 0;
    }
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _orders = new();

    public void Save(Order order)
    {
        _orders[order.Id] = order;
        Console.WriteLine($"Order {order.Id} saved to in-memory database.");
    }

    public Order? GetById(int id)
    {
        return _orders.ContainsKey(id) ? _orders[id] : null;
    }
}
public class ConsoleEmailService : IEmailService
{
    public void SendOrderConfirmation(Order order)
    {
        Console.WriteLine($"Email sent to {order.CustomerName} for order {order.Id}.");
    }
}
public class OrderService {
    private readonly IOrderValidator _validator;
    private readonly IOrderRepository _repository;
    private readonly IEmailService _emailService;

    public OrderService(
        IOrderValidator validator, 
        IOrderRepository repository, 
        IEmailService emailService)
    {
        _validator = validator;
        _repository = repository;
        _emailService = emailService;
    }

    public void ProcessOrder(Order order)
    {
        Console.WriteLine($"Starting order processing {order.Id}...");
        if (!_validator.IsValid(order))
        {
            Console.WriteLine("Order validation failed.");
            order.Status = OrderStatus.Cancelled;
            return;
        }
        order.Status = OrderStatus.Processed;
        _repository.Save(order);
        _emailService.SendOrderConfirmation(order);
        Console.WriteLine($"Order {order.Id} processed successfully.");
    }
}
