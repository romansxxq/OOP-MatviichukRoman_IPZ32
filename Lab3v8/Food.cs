public class Food : Product
{
    public DateTime ExpirationDate { get; set; }
    public Food(string name, float price, DateTime expirationDate) : base(name, price)
    {
        ExpirationDate = expirationDate;
    }
    //Деструктор класу Food
    ~Food()
    {
        Console.WriteLine($"Food {Name} is being destroyed");
    }
    public override string DisplayInfo()
    {
        return $"Food: {Name}, Price: {Price}, Expiration Date: {ExpirationDate.ToShortDateString()}";
    }
}