public class Product
{
    public string Name { get; set; }
    public float Price { get; set; }

    public Product(string name, float price)
    {
        Name = name;
        Price = price;
    }
    public virtual string DisplayInfo()
    {
        return $"Product: {Name}, Price: {Price}";
    }
}