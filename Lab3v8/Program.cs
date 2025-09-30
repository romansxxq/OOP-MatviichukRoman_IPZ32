class Program
{
    static void Main(string[] args)
    {
        List<Product> products = new List<Product>
        {
            new Book("C# Basics", 300, "John Smith", "TechBooks"),
            new Book("Algorithms", 450, "Robert Martin", "CodePress"),
            new Food("Apple", 25, DateTime.Now.AddDays(7)),
            new Food("Milk", 35, DateTime.Now.AddDays(3))
        };
        Console.WriteLine("Product List:");
        foreach (var product in products)
        {
            Console.WriteLine(product.DisplayInfo());
        }

        double totalPrice = products.Sum(p => p.Price);
        Console.WriteLine($"Total Price: {totalPrice} UAH");

        var avgPriceBook = products.OfType<Book>().Average(b => b.Price);
        Console.WriteLine($"Average price of books: {avgPriceBook} UAH");

        var avgPriceFood = products.OfType<Food>().Average(f => f.Price);
        Console.WriteLine($"Average price of food: {avgPriceBook} UAH");

        
    }
}