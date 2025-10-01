class Program
{
    static void Main(string[] args)
    {
        //Колекція продуктів, що містить книги та продукти харчування
        List<Product> products = new List<Product>
        {
            new Book("C# Basics", 300, "John Smith", "TechBooks", 2021),
            new Book("Algorithms", 450, "Robert Martin", "CodePress", 2018),
            new Food("Apple", 25, DateTime.Now.AddDays(7)),
            new Food("Milk", 35, DateTime.Now.AddDays(3))
        };
        //Виведення інформації про всі продукти
        Console.WriteLine("Product List:");
        foreach (var product in products)
        {
            Console.WriteLine(product.DisplayInfo());
        }
        //Виведення загальної вартості всіх продуктів
        double totalPrice = products.Sum(p => p.Price);
        Console.WriteLine($"Total Price: {totalPrice} UAH");
        //Виведення середньої вартості книг
        var avgPriceBook = products.OfType<Book>().Average(b => b.Price);
        Console.WriteLine($"Average price of books: {avgPriceBook} UAH");
        //Виведення середньої вартості продуктів харчування
        var avgPriceFood = products.OfType<Food>().Average(f => f.Price);
        Console.WriteLine($"Average price of book: {avgPriceBook} UAH");
        Console.WriteLine($"Average price of food: {avgPriceFood} UAH");

        
    }
}